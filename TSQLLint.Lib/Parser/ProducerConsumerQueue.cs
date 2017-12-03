using System;
using System.Collections.Generic;
using System.Threading;

namespace TSQLLint.Lib.Parser
{
    public class ProducerConsumerQueue
    {
        private readonly object _locker = new object();
        private readonly Thread[] _workers;
        private readonly Queue<Action> _itemQ = new Queue<Action>();

        public ProducerConsumerQueue(int workerCount)
        {
            _workers = new Thread[workerCount];

            // Create and start a separate thread for each worker
            for (var i = 0; i < workerCount; i++)
            {
                (_workers[i] = new Thread(Consume, 2000000)).Start();
            }
        }

        public void Shutdown(bool waitForWorkers)
        {
            // Enqueue one null item per worker to make each exit.
            for (var index = 0; index < _workers.Length; index++)
            {
                EnqueueItem(null);
            }

            // Wait for workers to finish
            if (waitForWorkers)
            {
                for (var index = 0; index < _workers.Length; index++)
                {
                    var worker = _workers[index];
                    worker.Join();
                }
            }
        }

        public void EnqueueItem(Action item)
        {
            lock (_locker)
            {
                _itemQ.Enqueue(item);           // We must pulse because we're
                Monitor.Pulse(_locker);         // changing a blocking condition.
            }
        }

        private void Consume()
        {
            while (true)                        // Keep consuming until
            {                                   // told otherwise.
                Action item;
                lock (_locker)
                {
                    while (_itemQ.Count == 0) Monitor.Wait(_locker);
                    item = _itemQ.Dequeue();
                }
                if (item == null) return;         // This signals our exit.
                item();                           // Execute item.
            }
        }
    }
}
