using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    class LockSync
    {
        private readonly int _readCount;

        private readonly List<Item> _items = new List<Item>();
        private int _id = 1;
        private int _count = 30;

        private readonly object locker = new object();

        public LockSync(int readCount)
        {
            _readCount = readCount;
        }

        public void StartSync()
        {
            var writeThreads = new List<Thread>();
            var readThreads = new List<Thread>();

            for (var i = 0; i < 2; i++)
            {
                var thread = new Thread(WriteItems)
                {
                    Name = $"Thread {i}"
                };
                writeThreads.Add(thread);
                thread.Start();
            }

            for (var i = 2; i < _readCount + 2; i++)
            {
                var thread = new Thread(ReadItems)
                {
                    Name = $"Thread {i}"
                };
                readThreads.Add(thread);
                thread.Start();
            }

            foreach (var thread in writeThreads)
            {
                thread.Join();
                Console.WriteLine($"{thread.Name} joined");
            }

            foreach (var thread in readThreads)
            {
                thread.Join();
                Console.WriteLine($"{thread.Name} joined");
            }
            Console.WriteLine("Done");
        }

        private void WriteItems()
        {
            while (true)
            {
                lock (locker)
                {
                    if (_count == 0) break;
                    var item = new Item { Id = _id, Name = $"I {_id} {Thread.CurrentThread.Name}" };
                    _items.Add(item);
                    
                    _id++;
                    _count--;
                }

                Thread.Sleep(10);
            }
        }

        private void ReadItems()
        {
            while (true)
            {
                lock (locker)
                {
                    if (_items.Count == 0) break;
                    var item = _items.First();

                    Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, item);

                    _items.RemoveAt(0);
                }

                Thread.Sleep(10);
            }
        }
    }
}
