using System;
using System.Collections.Concurrent;
using System.Threading;

namespace TSQLLint.Infrastructure.Reporters
{
    public static class NonBlockingConsole
    {
        public static BlockingCollection<string> messageQueue = new BlockingCollection<string>();

        static NonBlockingConsole()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    Console.WriteLine(messageQueue.Take());
                }
            }).Start();
        }

        public static void WriteLine(string value)
        {
            messageQueue.Add(value);
        }
    }
}
