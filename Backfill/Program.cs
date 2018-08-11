using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backfill
{
    class Program
    {
        static void Main(string[] args)
        {

            var f = new B2hFeature();
            var c = f.GetAllFeatures().GetAwaiter().GetResult();

            Console.WriteLine($"read {c} parcels");
            Console.WriteLine("That's all folks");
            Console.ReadKey();
        }
    }
}
