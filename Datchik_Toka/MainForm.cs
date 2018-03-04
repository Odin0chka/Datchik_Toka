using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Datchik_Toka
{
    public partial class MainForm : Form
    {
        private int _cx;        // Значение по Х
        private string _data;   // Данные полученные с COM порта
        private double _am;     // Величина тока
        private double _avr;    // Сумма значений токов, для расчета среднего

        private delegate void SetTextDeleg(string text);

        public MainForm()
        {
            InitializeComponent();
            _cx = 0;
            _avr = 0;
            // Настройка графика
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Title = "t, c";
            chart1.ChartAreas[0].AxisY.Title = "I, A";
            chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 16);
            chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 16);
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 20;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            // Проверка наличия устройств
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                MessageBox.Show("Устройств не найдено!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                startButton.Enabled = false;
                return;
            }

            // Настройка портов
            foreach (string p in ports)
                combo_port.Items.Add(p);
            serial_port.BaudRate = 9600;
            serial_port.Parity = Parity.None;
            serial_port.Handshake = Handshake.None;
            serial_port.DataBits = 8;
            combo_port.SelectedIndex = 0;
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            serial_port.Close();
        }

        //Кнопки
        private void StartButtonClick(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;
            pauseButton.Enabled = true;
            combo_port.Enabled = false;
            aboutButton.Visible = false;

            chart1.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = 0;
            chart1.Series[0].Points.Clear();
            _cx = 0;
            timer1.Start();
        }

        private void StopButtonClick(object sender, EventArgs e)
        {
            startButton.Enabled = true;
            stopButton.Enabled = false;
            pauseButton.Enabled = false;
            combo_port.Enabled = true;
            aboutButton.Visible = true;
            timer1.Stop();
        }

        private void PauseButtonClick(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;
            pauseButton.Enabled = true;
            combo_port.Enabled = false;
            if (pauseButton.Text == "Пауза")
            {
                pauseButton.Text = "Продолжить";
                timer1.Stop();
            }
            else
            {
                pauseButton.Text = "Пауза";
                timer1.Start();
            }
        }

        private void AboutButtonClick(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        //Таймер
        private void TimerTick(object sender, EventArgs e)
        {
            if (_cx > 20)
                chart1.ChartAreas[0].AxisX.ScrollBar.Axis.ScaleView.Position = _cx - 19;

            chart1.Series[0].Points.AddXY(_cx, _am);
            _avr += _am;
            double x = _avr / _cx;
            label3.Text = Convert.ToString(Math.Round(x, 3));
            _cx++;
        }

        //Получение данных с порта
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
            try
            {
                string[] s = _data.Split('\r');
                s[0] = s[0].Replace('.', ',');
                _am = Convert.ToDouble(s[0]);
            }
            catch (FormatException)
            {

            }
        }

        //Выбор порта
        private void ComboPortSelectedIndexChanged(object sender, EventArgs e)
        {
            if (serial_port.IsOpen)
                serial_port.Close();
            serial_port.PortName = combo_port.SelectedItem.ToString();
            serial_port.Open();
        }

    }
}
