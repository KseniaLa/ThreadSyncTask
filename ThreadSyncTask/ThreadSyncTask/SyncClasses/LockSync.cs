using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    class LockSync : Sync
    {
        private readonly object locker = new object();

        public LockSync(int readCount) : base(readCount)
        {

        }

        protected override void WriteItems()
        {
            while (true)
            {
                lock (locker)
                {
                    if (_count == 0)
                    {
                        Monitor.PulseAll(locker);
                        break;
                    }

                    var item = new Item { Id = _id, Name = $"I {_id} {Thread.CurrentThread.Name}" };
                    _items.Add(item);
                    Console.WriteLine(_id);
                    _id++;
                    _count--;
                    Monitor.PulseAll(locker);
                }

                Thread.Sleep(5);
            }
        }

        protected override void ReadItems()
        {
            while (true)
            {
                lock (locker)
                {
                    if (_id >= _totalCount + 1 && _items.Count == 0)
                    {
                        break;
                    }

                    while (true)
                    {
                        if (_items.Count != 0)
                        {
                            var item = _items.First();

                            Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, item);

                            _items.RemoveAt(0);

                            Monitor.PulseAll(locker);

                            break;
                        }

                        if (_id >= _totalCount + 1 && _items.Count == 0)
                        {
                            break;
                        }

                        Monitor.Wait(locker);
                    }
                }

                Thread.Sleep(5);
            }

            Console.WriteLine($"{Thread.CurrentThread.Name} finished");
        }
    }
}
