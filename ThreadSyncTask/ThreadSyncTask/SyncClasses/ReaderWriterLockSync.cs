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

          public ReaderWriterLockSync(int readCount) : base(readCount)
          {
          }

          protected override void WriteItems()
          {
               while (true)
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
          }

          protected override void ReadItems()
          {
               while (true)
               {
                    try
                    {
                         _rwlock.AcquireWriterLock(100);

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
                         _rwlock.ReleaseWriterLock();
                    }

                    Thread.Sleep(5);
               }
          }
     }
}
