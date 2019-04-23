using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    abstract class Sync
    {
        protected readonly int _readCount;

        protected readonly List<Item> _items = new List<Item>();
        protected int _id = 1;
        protected const int _totalCount = 50;
        protected int _count = 50;

        protected Sync(int readCount)
        {
            _readCount = readCount;
            _count = _totalCount;
        }

        public virtual void StartSync()
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

        protected abstract void WriteItems();

        protected abstract void ReadItems();
    }
}
