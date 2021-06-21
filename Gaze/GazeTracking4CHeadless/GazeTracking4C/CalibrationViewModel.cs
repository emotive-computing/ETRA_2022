using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTracking4C
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Tobii.Research;
    using Tobii.Eyetracking.Sdk;
    

    /// <summary>
    /// View model for the calibration window.
    /// </summary>
    internal sealed class CalibrationViewModel : ICalibrationViewModel, INotifyPropertyChanged
    {
       // public bool? DialogResult;
        public event CalibrationDoneEventHandler CalibrationDone;

        private const double CalibrationNearLimit = 0.3;
        private const double CalibrationFarLimit = 0.7;

        /// <summary>
        /// Calibration points specified in the ADCS coordinate system. (0, 0) means top left of the display, (1, 1) is bottom right.
        /// </summary>
        private static readonly Point[] CalibrationPoints = new Point[]
        {
            new Point(0.5, 0.5),
            new Point(0.9, 0.1),
            new Point(0.9, 0.9),
            new Point(0.1, 0.9),
            new Point(0.1, 0.1)
        };

        private Dispatcher _dispatcher;
        private IEyeTracker _tracker;
        private ScreenBasedCalibration s;
        private int _currentCalibrationPoint;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dispatcher">Dispatcher used for marshaling operations to the main thread.</param>
        /// <param name="eyeTrackerUrl">Eye tracker URL.</param>
        /// <param name="exitAction">Delegate invoked to exit the application.</param>
        public CalibrationViewModel(Dispatcher dispatcher, IEyeTracker eyeTracker)
        {
            this.s = new ScreenBasedCalibration(eyeTracker);
            _dispatcher = dispatcher;
            this._tracker = eyeTracker;
            Stage = Stage.Initializing;
            ContinueCommand = new ActionCommand(Continue);
            EyePositions = new ObservableCollection<Point>();

     
            _tracker.EventErrorOccurred += OnEyeTrackerError;
            _tracker.GazeDataReceived += OnGazeData;
            s.EnterCalibrationModeAsync();
            OnStartCalibrationCompleted();
        }

        public void onWindowClosing(object sender, CancelEventArgs e)
        {
            _tracker.EventErrorOccurred -= OnEyeTrackerError;
            _tracker.GazeDataReceived -= OnGazeData;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the stage the view is currently in.
        /// </summary>
        public Stage Stage { get; private set; }

        /// <summary>
        /// Gets a command used for moving on to the next stage if possible.
        /// (Typically invoked when the user presses space.)
        /// </summary>
        public ICommand ContinueCommand { get; private set; }

        /// <summary>
        /// Gets a command used for moving to the Exiting stage. Do not pass go, do not collect $200. 
        /// (Typically invoked when the user presses escape.)
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// Gets an error message that describes what went wrong in the Error stage.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets the positions of the detected eyes in the PositioningGuide stage.
        /// </summary>
        public ObservableCollection<Point> EyePositions { get; private set; }

        /// <summary>
        /// Gets the current positioning status in the PositioningGuide stage.
        /// </summary>
        public PositioningStatus PositioningStatus { get; private set; }

        /// <summary>
        /// Gets the position of the calibration dot in the Calibration stage.
        /// </summary>
        public Point CalibrationDotPosition
        {
            get
            {
                return CalibrationPoints[_currentCalibrationPoint];
            }
        }

        /// <summary>
        /// Starts collecting data for a calibration point. Call this method when the animation is finished.
        /// </summary>
        public void CalibrationDotAnimationCompleted()
        {
            Trace.WriteLine(string.Format("Adding calibration point {0}", _currentCalibrationPoint + 1));

            // when the animation has completed, we call calibration_add_point (once), which will in turn 
            // call OnAddCalibrationPointCompleted.
            s.CollectDataAsync(ToPoint2D(CalibrationDotPosition));
            OnAddCalibrationPointCompleted();
        }

        private static NormalizedPoint2D ToPoint2D(Point p)
        {
            return new NormalizedPoint2D((float)p.X, (float)p.Y);
        }

        private static Point ToPoint(Point2D p)
        {
            return new Point(p.X, p.Y);
        }

        private void Continue()
        {
            switch (Stage)
            {
                case Stage.PositioningGuide:
                    // proceed to start calibration
                    StartCalibration();
                    break;

                case Stage.CalibrationFailed:
                    // go back to the positioning guide
                    StartPositioningGuide();
                    break;

                case Stage.Finished:
                case Stage.Error:
                    // continue from terminal states means exit the application
                    Dispose();
                    break;

                default:
                    // ignore.
                    break;
            }
        }

        private void StartPositioningGuide()
        {
            Trace.WriteLine("Starting positioning guide.");

            Stage = Stage.PositioningGuide;
            OnPropertyChanged("Stage");
        }

        private void StartCalibration()
        {
            Trace.WriteLine("Starting calibration.");

            Stage = Stage.Calibration;
            _currentCalibrationPoint = 0;
            OnPropertyChanged("Stage");
            OnPropertyChanged("CalibrationDotPosition"); // triggers the animation -- the view should call CalibrationDotAnimationCompleted when it finishes.
        }

        private void OnGazeData(object sender, Tobii.Research.GazeDataEventArgs e)
        {
            // mirror the x coordinate to make the visualization make sense.
            var left = new Point2D(1 - e.LeftEye.GazeOrigin.PositionInTrackBoxCoordinates.X, e.LeftEye.GazeOrigin.PositionInTrackBoxCoordinates.Y);
            var right = new Point2D(1 - e.RightEye.GazeOrigin.PositionInTrackBoxCoordinates.X, e.RightEye.GazeOrigin.PositionInTrackBoxCoordinates.Y);
            var z = 1.1;

            if(e.LeftEye.GazeOrigin.Validity == Validity.Valid && e.RightEye.GazeOrigin.Validity == Validity.Valid)
            {
                z = (e.LeftEye.GazeOrigin.PositionInTrackBoxCoordinates.Z + e.RightEye.GazeOrigin.PositionInTrackBoxCoordinates.Z) / 2;
            }else if(e.LeftEye.GazeOrigin.Validity == Validity.Valid)
            {
                z = e.LeftEye.GazeOrigin.PositionInTrackBoxCoordinates.Z;
                right = new Point2D(double.NaN, double.NaN);
            }else if (e.RightEye.GazeOrigin.Validity == Validity.Valid)
            {
                z = e.RightEye.GazeOrigin.PositionInTrackBoxCoordinates.Z;
                left = new Point2D(double.NaN, double.NaN);
            }
            else
            {
                left = right = new Point2D(double.NaN, double.NaN);
            }

            

            _dispatcher.BeginInvoke(new Action(() =>
            {
                SetEyePositions(left, right, z);
            }));
        }

        private void SetEyePositions(Point2D left, Point2D right, double z)
        {
            EyePositions.Clear();

            if (!double.IsNaN(left.X))
            {
                EyePositions.Add(ToPoint(left));
            }

            if (!double.IsNaN(right.X))
            {
                EyePositions.Add(ToPoint(right));
            }

            if (z < CalibrationNearLimit)
            {
                PositioningStatus = GazeTracking4C.PositioningStatus.TooClose;
            }
            else if (z <= CalibrationFarLimit)
            {
                PositioningStatus = GazeTracking4C.PositioningStatus.PositionOk;
            }
            else
            {
                PositioningStatus = GazeTracking4C.PositioningStatus.TooFarOrNotDetected;
            }

            OnPropertyChanged("PositioningStatus");
        }

        private void OnEyeTrackerError(object sender, EventErrorEventArgs e)
        {
            Trace.WriteLine("The eye tracker reported a spurious error.");
            HandleError(e.Message);
        }

        private void OnStartCalibrationCompleted()
        {
            
            Trace.WriteLine("Start calibration completed.");

            _dispatcher.Invoke(new Action(StartPositioningGuide));

            //_tracker.StartTrackingAsync(OnGenericOperationCompleted);
        }

        private void OnAddCalibrationPointCompleted()
        {
            Trace.WriteLine("Add calibration point completed.");


            if (_currentCalibrationPoint + 1 < CalibrationPoints.Length)
            {
                // next point, please
                _dispatcher.Invoke(new Action(() =>
                {
                    _currentCalibrationPoint++;
                    OnPropertyChanged("CalibrationDotPosition");
                }));
            }
            else
            {
                // done: move to the next stage.
                _dispatcher.Invoke(new Action(() =>
                {
                    Stage = GazeTracking4C.Stage.ComputingCalibration;
                    OnPropertyChanged("Stage");
                }));

                Trace.WriteLine("Computing and setting calibration.");
                s.ComputeAndApplyAsync();
                OnComputeAndSetCalibrationCompleted();
            }
        }

        private void OnComputeAndSetCalibrationCompleted()
        {
            Trace.WriteLine("Compute and set calibration completed.");

            

            s.LeaveCalibrationModeAsync();
            OnStopCalibrationCompleted();
        }

        private void OnStopCalibrationCompleted()
        {
            Trace.WriteLine("Stop calibration completed.");

            _dispatcher.Invoke(new Action(() =>
            {
                Stage = Stage.Finished;
                OnPropertyChanged("Stage");
            }));
        }

        private void OnGenericOperationCompleted()
        {
            Trace.WriteLine("Operation completed.");

        }

        private void HandleError(string message)
        {
            Trace.WriteLine("Error: " + message);

            var action = new Action(() =>
            {
                Stage = Stage.Error;
                ErrorMessage = message;
                OnPropertyChanged("Stage");
            });

            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.BeginInvoke(action);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            CalibrationDone(this, new EventArgs());
        }
    }
}
