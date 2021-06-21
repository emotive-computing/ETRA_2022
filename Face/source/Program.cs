using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceLauncher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("hello world.");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Main form = new FaceLauncher.Main(args);
            Application.Run();
        }
    }
}
