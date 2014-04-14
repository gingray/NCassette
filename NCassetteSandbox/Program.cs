using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCassetteLib;
using NCassetteLib.Common;

namespace NCassetteSandbox
{
    class Program
    {
        /// <summary>
        ///Expression in lambda chaching in ur file system or anywhere u specify
        ///using this u can easyly pass heavy or unstable parts of code like fetching data from some api
        /// or from database
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var x = "1231";
            var result = NCassette.Record(() => new StoreItem<CustomClass>(new CustomClass(), DateTime.Now))
                .SerializeWayJson()
                .WorkInReleaseMode()
                .DependsOn(x)
                .SetLifeTime(c => c.DateTime < DateTime.Now)
                .StorageInTempFiles()
                .Execute();

            Console.WriteLine("{0} {1} now: {2}", result.MainObject, result.DateTime, DateTime.Now);
            Console.WriteLine("Done");
            Console.ReadLine();

        }
    }
}
