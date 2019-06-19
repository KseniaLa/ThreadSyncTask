using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
     class ReaderWriterLockSlimSync : Sync
     {
          private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

          private int _readIndex = -1;

          public ReaderWriterLockSlimSync(int readCount) : base(readCount)
          {
          }

          protected override void WriteItems()
          {
               while (true)
               {
                    try
                    {
                         _lock.EnterWriteLock();

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
                         _lock.ExitWriteLock();
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
                         _lock.EnterReadLock();

                         var currIndex = 0;

                         Interlocked.CompareExchange(ref currIndex, Interlocked.Increment(ref _readIndex), 0);

                         if (_id >= _totalCount + 1 && currIndex >= _totalCount)
                         {
                              break;
                         }

                         var exit = false;

                         if (_items.Count - 1 < currIndex && currIndex < _totalCount)
                         {
                              _lock.ExitReadLock();
                              exit = true;
                         }

                         while (_items.Count - 1 < currIndex && currIndex < _totalCount) { }
                         
                         if (exit)
                              _lock.EnterReadLock();

                         if (_items.Count - 1 >= currIndex)
                         {
                              item = _items[currIndex];
                         }
                    }
                    finally
                    {
                         _lock.ExitReadLock();
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
