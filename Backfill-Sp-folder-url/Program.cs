using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backfill_Sp_folder_url
{
    class Program
    {
        static void Main(string[] args)
        {
            var h = new Backfill();
            var t = h.Do();
            t.GetAwaiter().GetResult();
            
            Console.WriteLine("That's all folks");
            Console.ReadKey();
        }
    }
}
