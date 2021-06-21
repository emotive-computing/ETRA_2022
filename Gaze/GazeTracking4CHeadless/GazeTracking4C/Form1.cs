using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;
using System.Timers;
using Tobii.Eyetracking.Sdk;
using Timer = System.Windows.Forms.Timer;

namespace GazeTracking4C
{
    public partial class Form1 : Form
    {
        private Tracker4C tracker;
        private string PID;
        private int binWidth;
        private string expName;
        private int recordTime;
        string path = "C:/projects/4C_Gaze/Output/";
        string outPath = "";
        static System.Windows.Forms.Timer t;
        Dictionary<string, string> config = new Dictionary<string, string>();


        public void readConfig()
        {
            string file = "C:/projects/4C_Gaze/4C_config.txt";

            using (System.IO.StreamReader sr = new System.IO.StreamReader(file))
            {
                while (!sr.EndOfStream) // Keep reading until we get to the end
                {
                    string splitMe = sr.ReadLine();
                    string[] bananaSplits = splitMe.Split(new char[] { '\t' }); //Split at the tabs

                    if (bananaSplits.Length < 2) // If we get less than 2 results, discard them
                        continue;
                    else if (bananaSplits.Length == 2) // Easy part. If there are 2 results, add them to the dictionary
                        config.Add(bananaSplits[0].Trim(), bananaSplits[1].Trim());
                   
                }
            }

            PIDLab.Text = config["PID"];
        }

        public void ssTick(object source, EventArgs e)
        {
            // code here will run every second
            CaptureMyScreen();
        }

        private void StartRecording()
        {

            using (StreamWriter w = File.AppendText("CurrentDirectoryLog.txt"))
            {
                w.WriteLine(Environment.CurrentDirectory);
            }

            this.PID = config["PID"] + "." + DateTime.UtcNow.Day + "." + DateTime.UtcNow.Month + "." +
                       DateTime.UtcNow.Hour + "." + DateTime.UtcNow.Minute;
            Directory.CreateDirectory(path + PID);
            outPath = path + PID + "/";
            //MessageBox.Show(Screen.PrimaryScreen.Bounds.Width.ToString());
            tracker = new Tracker4C(PID, outPath, Screen.PrimaryScreen.Bounds.Height,
                Screen.PrimaryScreen.Bounds.Width);

            if (this.tracker.status == 0)
            {
                Console.WriteLine("Trigger");
                this.Close();
                Application.Exit();
                return;
            }

            try
            {
                tracker.LoadLocalCalibration();
            }
            catch
            {
                MessageBox.Show("EyeTracker License Error");
            }

            Timer ssTimer = new Timer();
            ssTimer.Tick += ssTick;
            ssTimer.Interval = 1000; // 1000 ms is one second
            ssTimer.Start();


            tracker.Start();
            t = new Timer();
            //MessageBox.Show(config["BinWindow"]);
            if (this.recordTime < 0)
            {
                t.Interval = int.Parse(config["BinWindow"]) * 60000;
                //MessageBox.Show(t.Interval.ToString());
                t.Tick += _timer_Tick;
            }
            else
            {
                t.Interval = this.recordTime * 60000;
                //MessageBox.Show(t.Interval.ToString());
                t.Tick += runToCompletion;
            }


            double thresh = 0.25;
            Random rand = new Random();
            double r = rand.NextDouble();
            if (r < thresh)
            {
                checkCalib(t);
            }
            else
            {
                t.Start();
            }

            
            
            RunningLAB.Text = "True";
        }



        void checkCalib(Timer after)
        {
            MessageBox.Show(
                "We will now begin a quick calibration check, please fixate on each cross in turn. This window will dissappear when the check is compete. When you are ready click okay. ");
            int h = Screen.PrimaryScreen.Bounds.Height;
            int w = Screen.PrimaryScreen.Bounds.Width;
            Utilities.Logger calibCheckerLog = new Utilities.Logger(this.PID, "", this.outPath + this.PID + "_calibCheck.txt",
                "PointX\t" + "PointY\t" + "Status\t"+
                "Width\t" +
                "Height\t");



            Color col = Color.White;
            int size = 25;


            Queue<FixationPoint> q = new Queue<FixationPoint>();
            q.Enqueue(new FixationPoint(new Point2D(0.5, 0.5), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.1, 0.1), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.1, 0.9), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.5, 0.9), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.5, 0.1), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.9, 0.1), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.9, 0.9), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.9, 0.5), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.1, 0.5), col, size));
            q.Enqueue(new FixationPoint(new Point2D(0.5, 0.5), col, size));

            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1500;
            CalibrationCheckForm c = new CalibrationCheckForm();
            c.Show();
            timer1.Enabled = true;
            timer1.Start();
            FixationPoint dispPoint = null;

            timer1.Tick += (s, e) =>
            {
                c.ClearPoints();
                if (dispPoint != null)
                {
                    calibCheckerLog.log(dispPoint.Center.X, dispPoint.Center.Y, "Cleared",w,h);
                }
                
                if (q.Count == 0)
                {
                    timer1.Enabled = false;
                    timer1.Stop();
                    c.Close();
                    after.Start();
                }
                else
                {
                    dispPoint = q.Dequeue();
                    
                    c.AddFixationPoint(dispPoint);
                    calibCheckerLog.log(dispPoint.Center.X, dispPoint.Center.Y, "Displayed", w, h);
                    Thread.Sleep(1000);
                }
                
                //Console.WriteLine("stop wait timer");
            };


            
            
        
        }
        


        void _timer_Tick(object sender, EventArgs e)
        {
            tracker.Stop();
            t.Stop();
            StartRecording();
        }

        void runToCompletion(object sender, EventArgs e)
        {

            
            tracker.Stop();
            t.Stop();
            //TransferFile();
            this.Close();
            Application.Exit();
            return;
        }

        void TransferFile()
        {


            Process serverPostProcess = new Process();
            serverPostProcess.StartInfo.FileName = Environment.CurrentDirectory + "/transferFile.bat";
            serverPostProcess.StartInfo.CreateNoWindow = true;
            serverPostProcess.StartInfo.Arguments = PID;
            serverPostProcess.Start();
        }

        public Form1(int numMin = -1)
        {
            
            //MessageBox.Show("Loaded");
            this.ShowInTaskbar = false;
            this.recordTime = numMin;
            
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = true;
            TrayIcon.Icon = SystemIcons.Application;
            this.WindowState = FormWindowState.Minimized;
        }


        /**
        private void LoadButton_Click(object sender, EventArgs e)
        {
            tracker.LoadLocalCalibration();
            Record.Enabled = true;
        }

        private void Record_Click(object sender, EventArgs e)
        {
            tracker.Start();
            LoadButton.Enabled = false;
            Record.Enabled = false;
            Record.Visible = false;
            StopButton.Visible = true;
            StopButton.Enabled = true;
            InitButton.Enabled = false;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            tracker.Stop();
            StopButton.Enabled = false;
            Record.Enabled = true;
            StopButton.Visible = false;
            Record.Visible = true;
        }

        private void PID_Box_TextChanged(object sender, EventArgs e)
        {

        }

        private void RunCalibration_Click(object sender, EventArgs e)
        {
            tracker.Calibrate();
        }
    **/

        private void Form1_Shown(object sender, EventArgs e)
        {
            readConfig();
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            StartRecording();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void CaptureMyScreen()
        {
            try
            {
               //Creating a new Bitmap object
                Bitmap captureBitmap = new Bitmap(1024, 768, PixelFormat.Format32bppArgb);
                //Bitmap captureBitmap = new Bitmap(int width, int height, PixelFormat);
                //Creating a Rectangle object which will  
                //capture our Current Screen
                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                //Creating a New Graphics Object
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                //Copying Image from The Screen
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                //Saving the Image File (I am here Saving it in My E drive).
                string filename = DateTime.UtcNow.Day + "." + DateTime.UtcNow.Month + "." + DateTime.UtcNow.Hour + "." + DateTime.UtcNow.Minute+ "." + DateTime.UtcNow.Second + ".png";
                //captureBitmap.Save(outPath + filename, ImageFormat.Jpeg);
                //Displaying the Successfull Result
                //MessageBox.Show("Screen Captured");

                

                var dimensions = Screen.FromControl(this).Bounds;

                

                string imageName =outPath + filename;
                using (Bitmap bm = new Bitmap(captureRectangle.Width, captureRectangle.Height))
                {
                    using (Graphics g = Graphics.FromImage(bm))
                        g.CopyFromScreen(new Point(0, 0), Point.Empty, dimensions.Size);

                    bm.Save(imageName,
                        System.Drawing.Imaging.ImageFormat.Png);
                }

            }

            catch (Exception ex)

            {

                MessageBox.Show(ex.Message);

            }

        }
    }
}
