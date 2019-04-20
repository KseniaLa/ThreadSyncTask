using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSyncTask.SyncClasses
{
     class ManualResetEventSync : Sync
     {
          public ManualResetEventSync(int readCount) : base(readCount)
          {
          }

          protected override void WriteItems()
          {
               throw new NotImplementedException();
          }

          protected override void ReadItems()
          {
               throw new NotImplementedException();
          }
     }
}
