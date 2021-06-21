using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Snooze
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (Process process in Process.GetProcessesByName("FaceLauncher"))
            {
                process.Kill();
            }

            foreach (Process process in Process.GetProcessesByName("FeatureExtraction"))
            {
                process.Kill();
            }

            System.Threading.Thread.Sleep(7200000); // 7200000
            Directory.SetCurrentDirectory(Environment.CurrentDirectory + "/../../../../bin/Debug");
            Process launcher = new Process();
            launcher.StartInfo.FileName = "FaceLauncher.exe";
            launcher.Start();

        }
    }
}
