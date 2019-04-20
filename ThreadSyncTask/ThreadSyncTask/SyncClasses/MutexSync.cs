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

        private readonly Mutex mutex = new Mutex();

        public MutexSync(int readCount) : base(readCount)
        {

        }

        protected override void WriteItems()
        {
            while (true)
            {
                try
                {
                    mutex.WaitOne();

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
                    mutex.ReleaseMutex();
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
                    mutex.WaitOne();

                    if (_items.Count == 0)
                    {
                        break;
                    }

                    var item = _items.First();

                    Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, item);

                    _items.RemoveAt(0);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
                
                Thread.Sleep(5);
            }
        }
    }
}
