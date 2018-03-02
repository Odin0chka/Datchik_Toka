using System;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace Datchik_Toka
{
    public partial class Form1 : Form
    {
        private double SelectionStart = double.NaN;

        public Form1()
        {
            InitializeComponent();
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Avoid dialog processing of arrow keys
            if (keyData == Keys.Left || keyData == Keys.Right)
            {
                return false;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void chart1_Click(object sender, System.EventArgs e)
        {
            // Set input focus to the chart control
            chart1.Focus();

            // Set the selection start variable to that of the current position
            this.SelectionStart = chart1.ChartAreas["Default"].CursorX.Position;
        }

        private void ProcessSelect(System.Windows.Forms.KeyEventArgs e)
        {
            // Process keyboard keys
            if (e.KeyCode == Keys.Right)
            {
                // Make sure the selection start value is assigned
                if (this.SelectionStart == double.NaN)
                    this.SelectionStart = chart1.ChartAreas["Default"].CursorX.Position;

                // Set the new cursor position 
                chart1.ChartAreas["Default"].CursorX.Position += chart1.ChartAreas["Default"].CursorX.Interval;
            }
            else if (e.KeyCode == Keys.Left)
            {
                // Make sure the selection start value is assigned
                if (this.SelectionStart == double.NaN)
                    this.SelectionStart = chart1.ChartAreas["Default"].CursorX.Position;

                // Set the new cursor position 
                chart1.ChartAreas["Default"].CursorX.Position -= chart1.ChartAreas["Default"].CursorX.Interval;
            }

            // If the cursor is outside the view, set the view
            // so that the cursor can be seen
            SetView();


            chart1.ChartAreas["Default"].CursorX.SelectionStart = this.SelectionStart;
            chart1.ChartAreas["Default"].CursorX.SelectionEnd = chart1.ChartAreas["Default"].CursorX.Position;
        }

        private void SetView()
        {
            // Keep the cursor from leaving the max and min axis points
            if (chart1.ChartAreas["Default"].CursorX.Position < 0)
                chart1.ChartAreas["Default"].CursorX.Position = 0;

            else if (chart1.ChartAreas["Default"].CursorX.Position > 75)
                chart1.ChartAreas["Default"].CursorX.Position = 75;


            // Move the view to keep the cursor visible
            if (chart1.ChartAreas["Default"].CursorX.Position < chart1.ChartAreas["Default"].AxisX.ScaleView.Position)
                chart1.ChartAreas["Default"].AxisX.ScaleView.Position = chart1.ChartAreas["Default"].CursorX.Position;

            else if ((chart1.ChartAreas["Default"].CursorX.Position >
                (chart1.ChartAreas["Default"].AxisX.ScaleView.Position + chart1.ChartAreas["Default"].AxisX.ScaleView.Size)))
            {
                chart1.ChartAreas["Default"].AxisX.ScaleView.Position =
                    (chart1.ChartAreas["Default"].CursorX.Position - chart1.ChartAreas["Default"].AxisX.ScaleView.Size);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] kek = new int[200];
            Random r = new Random();
            int[] x = new int[200];
            for (int i = 0; i < 200; i++)
            {
                r.Next(1, 200);
                x[i] = i;
            }
            chart1.Series[0].Points.DataBindXY(x, kek);
        }
    }
}
