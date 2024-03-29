﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadSyncTask.Entities;

namespace ThreadSyncTask.SyncClasses
{
    class AutoResetEventSync : Sync
    {
        private readonly AutoResetEvent _waitHandler = new AutoResetEvent(true);

        public AutoResetEventSync(int readCount) : base(readCount)
        {
        }

        protected override void WriteItems()
        {
            while (true)
            {
                try
                {
                    _waitHandler.WaitOne();

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
                    _waitHandler.Set();
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
                    _waitHandler.WaitOne();

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
                    _waitHandler.Set();
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
