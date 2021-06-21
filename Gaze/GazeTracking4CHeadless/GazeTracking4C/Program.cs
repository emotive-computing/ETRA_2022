using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GazeTracking4C
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int n = -1;
            if (args.Length > 0)  // Warning : Index was out of the bounds of the array
            {
                try
                {
                    n = int.Parse(args[0]);
                    //MessageBox.Show("" +    n);
                }catch
                {

                }

            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(n));
            //new Form1();
        }
    }
}
