using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTracking4C
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    
    using System.Windows;
    using System.Windows.Input;

    public delegate void CalibrationDoneEventHandler(object sender, EventArgs e);

    /// <summary>
    /// View model interface for the calibration window.
    /// The view model decides what should be displayed in the calibration window 
    /// and acts on input from the calibration window.
    /// </summary>
    public interface ICalibrationViewModel : INotifyPropertyChanged, IDisposable
    {
        event CalibrationDoneEventHandler CalibrationDone;
        /// <summary>
        /// Gets the stage the view is currently in.
        /// </summary>
        Stage Stage { get; }

        /// <summary>
        /// Gets a command used for moving on to the next stage if possible.
        /// (Typically invoked when the user presses space.)
        /// </summary>
        ICommand ContinueCommand { get; }

        /// <summary>
        /// Gets a command used for moving to the Exiting stage. Do not pass go, do not collect $200. 
        /// (Typically invoked when the user presses escape.)
        /// </summary>
        ICommand ExitCommand { get; }

        /// <summary>
        /// Gets an error message that describes what went wrong in the Error stage.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets the positions of the detected eyes in the PositioningGuide stage.
        /// </summary>
        ObservableCollection<Point> EyePositions { get; }

        /// <summary>
        /// Gets the current positioning status in the PositioningGuide stage.
        /// </summary>
        PositioningStatus PositioningStatus { get; }

        /// <summary>
        /// Gets the position of the calibration dot in the Calibration stage.
        /// </summary>
        Point CalibrationDotPosition { get; }

        /// <summary>
        /// Starts collecting data for a calibration point. Call this method when the animation is finished.
        /// </summary>
        void CalibrationDotAnimationCompleted();

        void onWindowClosing(object sender, CancelEventArgs e);
    }

    /// <summary>
    /// Modes that the view may be in.
    /// </summary>
    public enum Stage
    {
        Initializing,
        PositioningGuide,
        Calibration,
        ComputingCalibration,
        CalibrationFailed,
        Finished,
        Error
    }

    /// <summary>
    /// Possible states for the positioning guide.
    /// </summary>
    public enum PositioningStatus
    {
        TooClose,
        TooFarOrNotDetected,
        PositionOk
    }
}

