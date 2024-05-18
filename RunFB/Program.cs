using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FullBomHoum;

namespace RunFB
{
    class Program
    {
        static void Main(string[] args)
        {
            GetAssemblyID ass = new GetAssemblyID();
            ass.OnCmd();
        }
    }
}
