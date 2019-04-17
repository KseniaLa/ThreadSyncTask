using System;
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
            var lockSync = new LockSync(3);
            lockSync.StartSync();

            Console.ReadLine();
        }
    }
}
