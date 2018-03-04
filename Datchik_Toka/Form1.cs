using System;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Collections.Generic;
using ZedGraph;
using System.IO.Ports;
using System.Threading;

namespace Datchik_Toka
{
    public partial class Form1 : Form
    {
        private int _cx;
        private delegate void SetTextDeleg(string text);
        private string _data;
        private double _am;
        private double _avr;

        public Form1()
        {
            InitializeComponent();
            _cx = 1;
            _avr = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 20;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Title = "t, c";
            chart1.ChartAreas[0].AxisY.Title = "I, A";
            chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 16);
            chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 16);
            // получаем список доступных портов
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                MessageBox.Show("Устройств не найдено!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = false;
                return;
            }
            // выводим список портов
            foreach (string p in ports)
                combo_port.Items.Add(p);
            serial_port.BaudRate = 9600; //указываем скорость
            serial_port.Parity = Parity.None;
            serial_port.Handshake = Handshake.None;
            serial_port.DataBits = 8;
            combo_port.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Visible = true;
            label3.Visible = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_cx > 20)
            {
                chart1.ChartAreas[0].AxisX.Maximum = _cx;
                chart1.ChartAreas[0].AxisX.Minimum = _cx - 20;
                //chart1.Series[0].Points.RemoveAt(0);
            }
            chart1.Series[0].Points.AddXY(_cx, _am);
            _avr += _am;
            double x = _avr / _cx;
            label3.Text = Convert.ToString(Math.Round(x, 3));
            _cx++;
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(50);
            try
            {
                _data = serial_port.ReadExisting();
            }
            catch (Exception) { }
            Thread.Sleep(50);
            // Привлечение делегата на потоке UI, и отправка данных, которые
            // были приняты привлеченным методом.
            // ---- Метод "si_dataReceived" будет выполнен в потоке UI,
            // который позволит заполнить текстовое поле TextBox
            BeginInvoke(new SetTextDeleg(SiDataReceived), new object[] { _data });
        }

        private void SiDataReceived(string _data)
        {
            string[] s = _data.Split('\r');
            s[0] = s[0].Replace('.', ',');
            _am = Convert.ToDouble(s[0]);
        }

        private void combo_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serial_port.IsOpen)
                serial_port.Close();
            serial_port.PortName = combo_port.SelectedItem.ToString();
            serial_port.Open();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serial_port.Close();
        }
        //поставить таймер каждую секунду добавляющий точку на график
        //в сериале дописывать значение тока в инт, а в таймере потом считывать с него
    }
}
