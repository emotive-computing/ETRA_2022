using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tobii.Eyetracking.Sdk;

namespace GazeTracking4C
{
    public class FixationPoint
    {

        public Point2D Center { get; set; }

        public Color Color { get; set; }

        public int Size { get; set; }

        public FixationPoint(Point2D center, Color color, int size)
        {
            this.Center = center;
            this.Color = color;
            this.Size = size;
        }
    }

    public partial class CalibrationCheckForm : Form
    {
        private Size monitorSize = Screen.PrimaryScreen.Bounds.Size;
        private Size testAreaSize;
        private Form canvas;
        private double marginX, marginY;
        private List<FixationPoint> fixationPoints = new List<FixationPoint>();

        public CalibrationCheckForm(int areaWidth = -1, int areaHeight = -1)
        {
            canvas = new Form();
            if (areaWidth > 0 && areaHeight > 0)
            {
                testAreaSize = new Size(areaWidth, areaHeight);
            }
            else
            {
                testAreaSize = monitorSize;
            }

            marginX = (monitorSize.Width - testAreaSize.Width) / 2.0;
            marginY = (monitorSize.Height - testAreaSize.Height) / 2.0;
            InitializeComponent();
        }

        public void AddFixationPoint(FixationPoint fixationPoint)
        {
            fixationPoints.Add(fixationPoint);
            Invalidate();
        }

        public void RemoveFixationPoint(int index)
        {
            if (this.fixationPoints.Count > index)
            {
                this.fixationPoints.RemoveAt(index);
            }
            Invalidate();
        }

        public FixationPoint GetFixationPoint(int index)
        {
            if (this.fixationPoints.Count > index)
            {
                return this.fixationPoints[index];
            }
            else
            {
                return null;
            }
        }

        public void ClearPoints()
        {
            this.fixationPoints.Clear();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                int leftBoundary = (this.monitorSize.Width - this.testAreaSize.Width) / 2;
                int rightBoundary = (this.monitorSize.Width - this.testAreaSize.Width) / 2 + this.testAreaSize.Width;
                int topBoundary = (this.monitorSize.Height - this.testAreaSize.Height) / 2;
                int bottomBoundary = (this.monitorSize.Height - this.testAreaSize.Height) / 2 + this.testAreaSize.Height;

                Rectangle leftBar = new Rectangle(0, 0, leftBoundary, this.monitorSize.Height);
                Rectangle rightBar = new Rectangle(rightBoundary, 0, this.monitorSize.Width - rightBoundary, this.monitorSize.Height);

                Rectangle topBar = new Rectangle(leftBoundary, 0, this.testAreaSize.Width, topBoundary);
                Rectangle bottomBar = new Rectangle(leftBoundary, bottomBoundary, this.testAreaSize.Width, this.monitorSize.Height - bottomBoundary);
                e.Graphics.FillRectangles(brush, new Rectangle[] { leftBar, rightBar, topBar, bottomBar });
            }
            foreach (FixationPoint fixationPoint in fixationPoints)
            {
                // Draw calibration circle
                Rectangle circleBounds = new Rectangle();
                circleBounds.X = (int)((testAreaSize.Width * fixationPoint.Center.X) + marginX - fixationPoint.Size / 2);
                circleBounds.Y = (int)((testAreaSize.Height * fixationPoint.Center.Y) + marginY - fixationPoint.Size / 2);
                circleBounds.Width = fixationPoint.Size;
                circleBounds.Height = fixationPoint.Size;

                //Draw fixation cross
                Rectangle crossVert = new Rectangle();
                crossVert.X = (circleBounds.X + (circleBounds.Width / 2));
                crossVert.Y = circleBounds.Y + (circleBounds.Height * 3 / 8);
                crossVert.Width = 1;
                crossVert.Height = circleBounds.Height / 4 + 1;
                Rectangle crossHorz = new Rectangle();
                crossHorz.X = circleBounds.X + (circleBounds.Width * 3 / 8);
                crossHorz.Y = (circleBounds.Y + (circleBounds.Height / 2));
                crossHorz.Width = circleBounds.Width / 4 + 1;
                crossHorz.Height = 1;
                using (SolidBrush brush = new SolidBrush(fixationPoint.Color))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    Console.WriteLine("Drawing circle " + circleBounds + " color " + fixationPoint.Color);
                    e.Graphics.FillEllipse(brush, circleBounds);

                    brush.Color = fixationPoint.Color == Color.Transparent ? Color.Transparent : Color.Black;
                    e.Graphics.FillRectangles(brush, new Rectangle[] { crossVert, crossHorz });
                }
            }
        }

        private void CalibrationForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true; // Really force it to be on top.
        }
    }
}
