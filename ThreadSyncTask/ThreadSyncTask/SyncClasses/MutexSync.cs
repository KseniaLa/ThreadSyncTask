using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    class MutexSync : Sync
    {

        private readonly Mutex _mutex = new Mutex();

        public MutexSync(int readCount) : base(readCount)
        {

        }

        protected override void WriteItems()
        {
            while (true)
            {
                try
                {
                    _mutex.WaitOne();

                    if (_count == 0)
                    {
                        break;
                    }

                    var item = new Item { Id = _id, Name = $"Added by {Thread.CurrentThread.Name}" };
                    _items.Add(item);

                    Console.WriteLine(_id);

                    _id++;
                    _count--;
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }

                Thread.Sleep(5);
            }
        }

        protected override void ReadItems()
        {
            Item item = null;

            while (true)
            {
                try
                {
                    _mutex.WaitOne();

                    // item list is empty and writing threads don't add new items
                    if (_id >= _totalCount + 1 && _items.Count == 0)
                    {
                        break;
                    }

                    if (_items.Count != 0)
                    {
                        item = _items.First();

                        _items.RemoveAt(0);
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }

                if (item != null)
                {
                    Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, item);
                    item = null;
                }
                
                Thread.Sleep(5);
            }

            Console.WriteLine($"{Thread.CurrentThread.Name} finished");
        }
    }
}
