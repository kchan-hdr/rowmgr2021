using com.hdr.rowmgr.Relocation;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relocation.Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            var c = new RelocationContext();
            var r = new RelocationRepository(c);
            IRelocationCaseOps h = new RelocationCaseOps(c, r);


            Console.WriteLine("That's all folks.");
            Console.ReadKey();
        }
    }
}
