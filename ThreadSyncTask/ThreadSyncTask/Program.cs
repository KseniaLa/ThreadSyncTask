﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadSyncTask.SyncClasses;

namespace ThreadSyncTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var syncMethod = new MutexSync(3);
            syncMethod.StartSync();

            Console.ReadLine();
        }
    }
}
