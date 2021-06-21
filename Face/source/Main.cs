using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using OpenCvSharp;
using System.IO;
using System.Timers;
using System.Runtime.InteropServices;

namespace FaceLauncher
{
    public partial class Main : Form
    {
        private Process clmf;
        private CvCapture capture = null;
        private Graphics previewGraphics = null;
        //private static StringBuilder serverLogBuilder = new StringBuilder();
        private string outputPath; 
        private string currentID; // ID of this particular session
        private string userID =  ""; // userID read in from user input
        private string camera;
        private int minsToRecord = 5;
        private bool perpetual = true;
        private bool displaySetup = false;
        private System.Timers.Timer timer;
        private const int SW_HIDE = 0;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);


        public Main(string[] args)
        {
            Console.WriteLine("top of main");
            string dirName = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("FaceLauncher.exe", "");
            Directory.SetCurrentDirectory(dirName);
            this.outputPath = Environment.CurrentDirectory + "/../../../Output/";

            if (args[0] != "-1" && args[0] != "")
            {
                bool correctlyParsed = Int32.TryParse(args[0], out this.minsToRecord);
                perpetual = false;
                if (!correctlyParsed)
                {
                    Console.WriteLine("Unable to correctly parse the command line arg for minutes to record.");
                    System.Environment.Exit(0);
                }
            }


            InitializeComponent();
            if (displaySetup)
            {
                this.Visible = true;
            }
                
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.previewGraphics = panelCamera.CreateGraphics();
            Utilities.TimeUtil.updateNTP(true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("You must enter the correct ID");
                return;
            }

            // Kill the camera so CLM-framework can start using it.
            this.previewGraphics.Dispose();
            timerCamera.Stop();
            this.capture.Dispose();

            userID = txtId.Text;
            camera = cmbCamera.Text;
            writeConfig();

            timerStart();
            startCLM();

            // blow everything up
            this.btnOk.Click -= new System.EventHandler(this.btnOk_Click);
            this.cmbCamera.SelectedIndexChanged -= new System.EventHandler(this.cmbCamera_SelectedIndexChanged);
            this.timerCamera.Tick -= new System.EventHandler(this.timerCamera_Tick);
            this.Controls.Remove(this.lblCamera);
            this.Controls.Remove(this.cmbCamera);
            this.Controls.Remove(this.panelCamera);
            this.Controls.Remove(this.btnOk);
            this.Controls.Remove(this.txtId);
            this.Controls.Remove(this.lblInstructions);

            this.lblCamera.Dispose();
            this.cmbCamera.Dispose();
            this.panelCamera.Dispose();
            this.btnOk.Dispose();
            this.txtId.Dispose();
            this.lblInstructions.Dispose();
            this.components.Dispose();
            this.Hide();
        }


        // return true if config file exists and read in properly
        // false otherwise
        private bool readConfig()
        {
            string usernameFile = Environment.CurrentDirectory + "/../../../../Face_Config.txt";
            if (File.Exists(usernameFile))
            {
                using (StreamReader reader = new StreamReader(usernameFile))
                {
                    reader.ReadLine(); // the first line is just the header
                    string[] configLine = reader.ReadLine().Trim('\n').Split(',');
                    // config file should contain userName (blank for lab)
                    // 0 or 1 indicating if should delete output file
                    // # indciating device # of camera, this is ignored for lab software
                    // # of lines to skip before sending output data (i.e. send 1 out of every n lines)
                    if (configLine.Length == 4)
                    {
                        if (configLine[0] != "-1" && configLine[0] != "")
                        {
                            userID = configLine[0];
                        }

                        else
                            displaySetup = true;

                        if (configLine[2] != "-1" && configLine[2] != "")
                        {
                            camera = configLine[2];
                        }

                        else
                            displaySetup = true;


                        if (!displaySetup)
                        {
                            timerStart();
                            startCLM();
                            return true;
                        }
                            
                    }

                }
            }

            displaySetup = true;
            return false;
        }

        private void writeConfig()
        {
            string usernameFile = Environment.CurrentDirectory + "/../../../../Face_Config.txt";
            string header;
            string[] configData;

            // rewriting only parts of config that should be rewritten
            using (StreamReader reader = new StreamReader(usernameFile))
            {
                header = reader.ReadLine();
                configData = reader.ReadLine().Split(',');
            }

            using (StreamWriter writer = new StreamWriter(usernameFile))
            {
                writer.WriteLine(header);
                configData[0] = userID;
                configData[2] = camera;
                writer.WriteLine(string.Join(",", configData));
            }

        }


        private void startCLM()
        {
            // Find the gap between local time and UTC NTP time servers.
            long ntpOffsetMS = (long)(Utilities.TimeUtil.getCurrentTime() - DateTime.Now)
                .TotalMilliseconds;


            currentID = userID + "." + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Hour + "." + DateTime.Now.Minute;

            string userOutputPath = outputPath + currentID;

            System.IO.Directory.CreateDirectory(userOutputPath); // Does nothing if already extant.

            // Record features using CLM-Framework.
            this.clmf = new Process();
            this.clmf.StartInfo.FileName = Environment.CurrentDirectory +
                "/../../CLM-framework/Release/FeatureExtraction.exe";
            // -q suppresses the output window
            this.clmf.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            this.clmf.StartInfo.Arguments =
                "-q -device " + camera + " -op \"" + userOutputPath + "/" + currentID + "-pose.txt\"" +
                " -ogaze \"" + userOutputPath + "/" + currentID + "-clmgaze.txt\"" +
                " -oaus \"" + userOutputPath + "/" + currentID + "-aus.txt\"" +
                " -NTPoffsetMS " + ntpOffsetMS;
            
            this.clmf.Start();
            this.clmf.PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
        }

        private void stopCLM()
        {
            try
            {
                this.clmf.Kill();
            }

            catch (System.InvalidOperationException)
            {
                // just keep going
            }

            System.Threading.Thread.Sleep(500);
            using (StreamWriter writer = File.AppendText(Environment.CurrentDirectory + "/../../../../Post_To_ND_Server/bin/Face_closed.txt"))
            {
                writer.WriteLine(currentID);
            }
        }

        private void timerStart()
        {
            timer = new System.Timers.Timer(minsToRecord * 60 * 1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            stopCLM();

            if (perpetual)
                startCLM();
            else
            {
                Process serverPostProcess = new Process();
                serverPostProcess.StartInfo.FileName = Environment.CurrentDirectory + "/../../../../Post_To_ND_Server/Python/Python3/python.exe";
                serverPostProcess.StartInfo.UseShellExecute = false;
                serverPostProcess.StartInfo.CreateNoWindow = true;
                serverPostProcess.StartInfo.Arguments = "\"" + Environment.CurrentDirectory + "/../../../../Post_To_ND_Server/Python/Post_To_ND_Server_Face.py\" " + currentID;
                serverPostProcess.Start();
                System.Environment.Exit(0);

            }
        }




        private void timerCamera_Tick(object sender, EventArgs e)
        {
            if (this.capture == null)
                try
                {
                    this.capture = new CvCapture(int.Parse(cmbCamera.Text));
                    Cv.SetCaptureProperty(this.capture, CaptureProperty.FrameWidth, 640);
                    Cv.SetCaptureProperty(this.capture, CaptureProperty.FrameHeight, 480);
                }
                catch (Exception ex) { }
            else
            {
                IplImage frame = this.capture.QueryFrame();
                if (frame != null && frame.Size.Height > 0 && frame.Size.Width > 0)
                {
                    using (IplImage resized = new IplImage(
                        new CvSize(panelCamera.Width, panelCamera.Height),
                        frame.Depth, frame.NChannels))
                    {
                        OpenCvSharp.Cv.Resize(frame, resized);
                        OpenCvSharp.BitmapConverter.DrawToGraphics(
                            resized, this.previewGraphics, 0, 0,
                            panelCamera.Width, panelCamera.Height, 0, 0);
                    }
                }
            }
        }

        private void cmbCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.capture = null;
        }
    }
}
