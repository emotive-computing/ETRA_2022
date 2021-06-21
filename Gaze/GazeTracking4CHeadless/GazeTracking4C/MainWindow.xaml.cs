﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GazeTracking4C
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static readonly DependencyProperty CalibrationDotRadiusProperty =
            DependencyProperty.Register("CalibrationDotRadius", typeof(double), typeof(MainWindow));

        public MainWindow(ICalibrationViewModel dataContext)
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            Closing += dataContext.onWindowClosing;
            DataContext = dataContext;
            dataContext.CalibrationDone += OnCalibrationDone;
        }

        private void OnCalibrationDone(object sender, EventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        public double CalibrationDotRadius
        {
            get { return (double)GetValue(CalibrationDotRadiusProperty); }
            set { SetValue(CalibrationDotRadiusProperty, value); }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((ICalibrationViewModel)DataContext).PropertyChanged += (s, e1) =>
            {
                if (string.Equals(e1.PropertyName, "CalibrationDotPosition"))
                {
                    StartAnimation();
                }
            };
        }

        private void StartAnimation()
        {
            var animation = (DoubleAnimation)FindResource("ShrinkingCalibrationDotAnimation");
            this.BeginAnimation(CalibrationDotRadiusProperty, animation);
        }

        private void CalibrationDotAnimation_Completed(object sender, EventArgs e)
        {
            ((ICalibrationViewModel)DataContext).CalibrationDotAnimationCompleted();
        }


    }
}
