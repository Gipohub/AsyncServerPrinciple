using System.Collections.Concurrent;
using System.Threading;

namespace AsyncServerPrinciple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            tasks.Add(Task.Run(() => {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine($"{Server.GetCounter()} {DateTime.Now.Millisecond}");
                    Thread.Sleep(1);
                }
            }));
            tasks.Add(Task.Run(() => {
                for (int i = 0; i < 100; i++)
                {
                    int r = new Random().Next(1, 1000);
                    Console.WriteLine($"New counter is {r} {DateTime.Now.Millisecond}");
                    Server.SetCounter(r);
                    Thread.Sleep(1);
                }
            }));
            Task.WhenAll(tasks).Wait();
        }
    }
    static public class Server
    {
        static private int counter = 0;
        static private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public static int GetCounter()
        {
            cacheLock.EnterReadLock();
            try
            {
                return counter;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }
        public static void SetCounter(int counterIn)
        {
            cacheLock.EnterWriteLock();
            try
            {
                counter = counterIn;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }

            //Thread.Sleep(500);
        }
    }
}
