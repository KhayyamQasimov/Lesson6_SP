using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson6_SP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new ThreadManager(3);
            var ui = new ConsoleUI(manager);
            ui.Run();
        }
    }
}
