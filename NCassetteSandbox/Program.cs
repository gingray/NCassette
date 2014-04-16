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
            var url = "http://google.com";
            var result = NCassette.Record(() =>
            {
                //some very heavy manipulation here
                var networkRespone = NetworkResponse();
                var someClass = new CustomClass { SomeStringProperty = networkRespone };
                return someClass;
            })
                .SerializeWayJson()
                .WorkInReleaseMode()
                .DependsOn(url)
                .StorageInTempFiles()
                .Execute();


            result = NCassette.Record(() =>
            {
                //some very heavy manipulation here
                var networkRespone = NetworkResponse();
                var someClass = new CustomClass { SomeStringProperty = networkRespone };
                return someClass;
            })
                .SerializeWayJson()
                .DependsOn(url)
                .DuplicateStorageWithKey(string.Format("{0}.html", new Uri(url).Host))
                .StorageInFolder("CacheData")
                .Execute();

            Console.WriteLine("{0}", result.SomeStringProperty);
            Console.WriteLine("Done");
            Console.ReadLine();

        }

        private static string NetworkResponse()
        {
            return "Some network response";
        }
    }
}
