
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tobii.Research;



namespace GazeTracking4C
{
    class Tracker4C
    {

        private string calibrationQuality;
        private double calibrationAccuracy;
        private bool calibrated;
        private string outdir = "";
        private IEyeTracker tracker;
        private SimpleLogger logger;
        private String PID;
        private Utilities.Logger gazeLog;
        private int counter = 0;
        public int status = 0;
        int height;
        int width;
        public Tracker4C(String Particpant, String path, int h, int w)
        {
            logger = new SimpleLogger(path + Particpant + "_" + "gazeInfoLog.log", true);
            logger.Info("New Class Instance");
            logger.Info("Searching for Trackers");
            outdir = path;
            this.PID = Particpant;
            this.height = h;
            this.width = w;
            logger.Info("Participant Set to :" + this.PID);
            logger.Info("Screen Height :" + this.height);
            logger.Info("Screen Width  :" + this.width);
            EyeTrackerCollection T = Tobii.Research.EyeTrackingOperations.FindAllEyeTrackers();
            Console.WriteLine(T.Count);

            logger.Info("Trackers found: " + T.Count);
            try
            {
                tracker = T[0];
                this.status = 1;
            }catch
            {
                this.status = 0;
                return;
            }
            
            logger.Info("Tracker Serial: " + tracker.SerialNumber);
        }

        public void LoadLocalCalibration()
        {
            CalibrationData c = tracker.RetrieveCalibrationData();
            tracker.ApplyCalibrationData(c);
            

            Console.WriteLine(this.tracker.GetDisplayArea().Width.ToString() + " " + this.tracker.GetDisplayArea().Height.ToString());
            Console.WriteLine(this.tracker.GetEyeTrackingMode());

            DisplayArea displayArea = tracker.GetDisplayArea();

            Console.WriteLine("Bottom Left: ({0}, {1}, {2})",
            displayArea.BottomLeft.X, displayArea.BottomLeft.Y, displayArea.BottomLeft.Z);
            Console.WriteLine("Bottom Right: ({0}, {1}, {2})",
                displayArea.BottomRight.X, displayArea.BottomRight.Y, displayArea.BottomRight.Z);
            Console.WriteLine("Top Left: ({0}, {1}, {2})",
                displayArea.TopLeft.X, displayArea.TopLeft.Y, displayArea.TopLeft.Z);
            Console.WriteLine("Top Right: ({0}, {1}, {2})",
                displayArea.TopRight.X, displayArea.TopRight.Y, displayArea.TopRight.Z);

            var allEyeTrackingModes = tracker.GetAllEyeTrackingModes();
            foreach (var eyeTrackingMode in allEyeTrackingModes)
            {
                tracker.SetEyeTrackingMode(eyeTrackingMode);
                Console.WriteLine("New eye tracking mode is: {0}.", eyeTrackingMode.ToString());
            }
        }

        public bool Start()
        {
            gazeLog = new Utilities.Logger(this.PID, "", this.outdir + this.PID + "_GazeLog4C.txt",
                        "LeftGazePointX\t" +
                        "LeftGazePointY\t" +
                        "RightGazePointX\t" +
                        "RightGazePointY\t" +
                        "LeftGazePointXUser\t" +
                        "LeftGazePointYUser\t" +
                        "RightGazePointXUser\t" +
                        "RightGazePointYUser\t" +
                        "LeftGazePointXMonitor\t" +
                        "LeftGazePointYMonitor\t" +
                        "RightGazePointXMonitor\t" +
                        "RightGazePointYMonitor\t" +
                        "LeftPupilDiameter\t" +
                        "RightPupilDiameter\t" +
                        "LeftEyeValidity\t" +
                        "RightEyeValidity\t" +
                        "LeftPupilValidity\t" +
                        "RightPupilValidity\t" +
                        "SystemTime\t" +
                        "DeviceTime\t" +
                        "Width\t" +
                        "Height\t"+
                        "Count");
            logger.Debug("Gaze Log Init");
            tracker.GazeDataReceived += EyeTrackerGazeData;
            logger.Debug("Event Handler Added");

            return true;


        }

        private void EyeTrackerGazeData(object sender, GazeDataEventArgs e)
        {
            if (counter == 0)
            {
                logger.Debug("First Sample Recieved");
            }

            

            counter++;
            
            gazeLog.log(e.LeftEye.GazePoint.PositionOnDisplayArea.X,
                e.LeftEye.GazePoint.PositionOnDisplayArea.Y,
                e.RightEye.GazePoint.PositionOnDisplayArea.X,
                e.RightEye.GazePoint.PositionOnDisplayArea.Y,
                e.LeftEye.GazePoint.PositionInUserCoordinates.X,
                e.LeftEye.GazePoint.PositionInUserCoordinates.Y,
                e.RightEye.GazePoint.PositionInUserCoordinates.X,
                e.RightEye.GazePoint.PositionInUserCoordinates.Y,
                e.LeftEye.GazePoint.PositionOnDisplayArea.X * this.width,
                e.LeftEye.GazePoint.PositionOnDisplayArea.Y * this.height,
                e.RightEye.GazePoint.PositionOnDisplayArea.X * this.width,
                e.RightEye.GazePoint.PositionOnDisplayArea.Y * this.height,
                e.LeftEye.Pupil.PupilDiameter,
                e.RightEye.Pupil.PupilDiameter,
                e.LeftEye.GazePoint.Validity,
                e.RightEye.GazePoint.Validity,
                e.LeftEye.Pupil.Validity,
                e.RightEye.Pupil.Validity,
                e.SystemTimeStamp,
                e.DeviceTimeStamp,
                this.width,
                this.height,
                
                counter.ToString()
                );
            if (counter == 0)
            {
                logger.Debug("First Sample Processed");
            }

        }
        public bool Calibrate()
        {
            //Implement calibration
            logger.Debug("Entering Calibration Mode");
            CalibrationViewModel cvm = new CalibrationViewModel(Dispatcher.CurrentDispatcher, this.tracker);
            MainWindow mw = new MainWindow(cvm);
            this.calibrated = (bool)mw.ShowDialog();
            logger.Debug("Leaving Calibration Mode");
            //this.calibrationQuality = this.RatingFunction(this.tracker.RetrieveCalibrationData());
            //this.OnPropertyChanged("Calibrated");
           // Console.WriteLine(();
            return this.calibrated;
            
        }

        public void Stop()
        {
            tracker.GazeDataReceived -= EyeTrackerGazeData;
            gazeLog.close();
        }
        /**
                public string RatingFunction(CalibrationData calibration)
                {
                    if (calibration == null)
                        return "";

                    List<CalibrationPlotItem> calibrationPoints = new List<CalibrationPlotItem>();
                    foreach (CalibrationPlotItem data in calibration)
                    {
                        calibrationPoints.Add(data);
                    }

                    List<Double> distList = new List<Double>();
                    foreach (CalibrationPlotItem data in calibrationPoints)
                    {
                        if (data.LeftStatus.Equals(CalibrationPointStatus.CalibrationPointValidAndUsedInCalibration)
                            && data.RightStatus.Equals(CalibrationPointStatus.CalibrationPointValidAndUsedInCalibration))
                        {
                            double truX = data.TruePosition.X;
                            double truY = data.TruePosition.Y;
                            double leftX = data.LeftMapPosition.X;
                            double leftY = data.LeftMapPosition.Y;
                            double leftDist = Math.Sqrt(Math.Pow(leftX - truX, 2) + Math.Pow(leftY - truY, 2));
                            double rightX = data.RightMapPosition.X;
                            double rightY = data.RightMapPosition.Y;
                            double rightDist = Math.Sqrt(Math.Pow(rightX - truX, 2) + Math.Pow(rightY - truY, 2));
                            distList.Add(leftDist);
                            distList.Add(rightDist);
                        }
                    }

                    double accuracy = 0;
                    if (distList.Count > 0)
                    {

                        foreach (double d in distList)
                        {
                            accuracy += (d / distList.Count);
                        }
                    }
                    else
                    {
                        accuracy = 1;
                    }

                    this.calibrationAccuracy = accuracy;

                    if (accuracy < 0.015)
                        return "Calibration Quality: PERFECT";

                    if (accuracy < 0.03)
                        return "Calibration Quality: GOOD";

                    if (accuracy < 0.06)
                        return "Calibration Quality: MODERATE";

                    if (accuracy < 0.1)
                        return "Calibration Quality: POOR";

                    return "Calibration Quality: REDO";
                }
            }
            **/
    }
}
