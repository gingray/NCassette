using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCassette.Common;
using NCassette.Serialize;
using NCassette.Storage;

namespace NCassetteSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = "1231";
            var result = NCassette.NCassette.Record(() => new StoreItem<CustomClass>(new CustomClass(), DateTime.Now))
                .SerializeWayJson()
                .PleaseWorkInRealeseMode()
                .SetLifeTime(c => c.DateTime < DateTime.Now)
                .StorageInTempFiles()
                .Execute();

            Console.WriteLine("{0} {1} now: {2}", result.MainObject, result.DateTime, DateTime.Now);
            Console.WriteLine("Done");
            Console.ReadLine();

        }
    }
}
