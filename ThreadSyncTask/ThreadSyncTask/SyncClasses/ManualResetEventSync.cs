using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    class ManualResetEventSync : Sync
    {
        private readonly ManualResetEvent _waitHandler = new ManualResetEvent(true);

        public ManualResetEventSync(int readCount) : base(readCount)
        {
        }

        protected override void WriteItems()
        {
            while (true)
            {
                try
                {
                    _waitHandler.WaitOne();
                    _waitHandler.Reset();

                    if (_count == 0)
                    {
                        break;
                    }

                    var item = new Item { Id = _id, Name = $"I {_id} {Thread.CurrentThread.Name}" };
                    _items.Add(item);
                    Console.WriteLine(_id);
                    _id++;
                    _count--;
                }
                finally
                {
                    _waitHandler.Set();
                }

                Thread.Sleep(5);
            }
        }

        protected override void ReadItems()
        {
            while (true)
            {
                try
                {
                    _waitHandler.WaitOne();
                    _waitHandler.Reset();

                    if (_id >= _totalCount + 1 && _items.Count == 0)
                    {
                        break;
                    }

                    if (_items.Count != 0)
                    {
                        var item = _items.First();

                        Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, item);

                        _items.RemoveAt(0);
                    }
                }
                finally
                {
                    _waitHandler.Set();
                }

                Thread.Sleep(5);
            }

            Console.WriteLine($"{Thread.CurrentThread.Name} finished");
        }
    }
}
