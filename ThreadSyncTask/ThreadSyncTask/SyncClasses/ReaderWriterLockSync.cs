using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    class ReaderWriterLockSync : Sync
    {
        private ReaderWriterLock _rwlock = new ReaderWriterLock();
        private int _index = 0;

        public ReaderWriterLockSync(int readCount) : base(readCount)
        {
        }

        protected override void WriteItems()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        _rwlock.AcquireWriterLock(100);

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
                        _rwlock.ReleaseWriterLock();
                    }

                    Thread.Sleep(5);
                }
                catch (ApplicationException)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} - Timeout");
                }
            }
        }

        protected override void ReadItems()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        _rwlock.AcquireWriterLock(100);

                        if (_id >= _totalCount + 1 && _items.Count == 0)
                        {
                            break;
                        }

                        if (_items.Count != 0)
                        {
                            var item = _items.First();

                            Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, item);

                            //Interlocked.Increment(ref _index);
                            _items.RemoveAt(0);
                        }
                    }
                    finally
                    {
                        _rwlock.ReleaseReaderLock();
                    }

                    Thread.Sleep(5);
                }
                catch (ApplicationException)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} - Timeout");
                }
            }

            Console.WriteLine($"{Thread.CurrentThread.Name} finished");
        }
    }
}
