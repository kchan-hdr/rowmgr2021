using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tectra
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new DocuController();

            var t = c.DoWrite();
            t.GetAwaiter().GetResult();

            Console.WriteLine("That's all folks");
            Console.ReadKey();
        }
    }
}
