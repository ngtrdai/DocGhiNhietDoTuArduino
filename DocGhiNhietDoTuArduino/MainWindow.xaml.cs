using System;
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
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace DocGhiNhietDoTuArduino
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        #region Vẽ đồ thị
        public SeriesCollection SeriesCollection { get; set; }
        public List Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }


        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        SerialPort serialPort = new SerialPort();
        DispatcherTimer timerRealTime = new DispatcherTimer();
        DispatcherTimer timerReviceData = new DispatcherTimer();
        DateTime RealDate = new DateTime();



        List<duLieuClass> datas = new List<duLieuClass>();
        public MainWindow()
        {
            InitializeComponent();
            timerRealTime.Tick += TimerRealTime_Tick;
            timerRealTime.Start();
            timerReviceData.Interval = new TimeSpan(0, 0, 5);
            timerReviceData.Tick += TimerReviceData_Tick;
            
        }



        private void TimerReviceData_Tick(object sender, EventArgs e)
        {
               
            if (!serialPort.IsOpen)
            {
                trangThaiKetNoi.Text = "Chưa kết nối";                   
            }
            else if (serialPort.IsOpen)
            {
                trangThaiKetNoi.Text = "Đã kết nối";
                string duLieuNhan = serialPort.ReadLine().ToString();
                string[] arrListStr = duLieuNhan.Split(',');
                string NhietDo = arrListStr[0];
                string DoAm = arrListStr[1].Substring(0, arrListStr[1].Length - 1);
                nhietDo.Text = NhietDo + " (°C)";
                doAm.Text = DoAm + "%";
                SeriesCollection[0].Values.Add(Convert.ToDouble(NhietDo));
                SeriesCollection[1].Values.Add(Convert.ToDouble(DoAm));
                dataGrid.Items.Add(new duLieuClass() { cNhietDo = Convert.ToDouble(NhietDo), cDoAm = Convert.ToDouble(DoAm), cThoiGian = DateTime.Now.ToShortTimeString() });
                
            }
        }
        private void TimerRealTime_Tick(object sender, EventArgs e)
        {
            realTime.Text = DateTime.Now.ToShortTimeString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            timerReviceData.Start();
            RealDate = DateTime.Now;
            realDate.Text = RealDate.ToShortDateString();
            string[] danhSachCongCOM = SerialPort.GetPortNames();

            foreach (string item in danhSachCongCOM)
            {
                comList.Items.Add(item);
            }


            // Vẽ đồ thị
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Nhiệt Độ",
                    Values = new ChartValues<double> {}
                },
                new LineSeries
                {
                    Title = "Độ Ẩm",
                    Values = new ChartValues<double> {}
                }
            };

            YFormatter = value => value.ToString("");
            DataContext = this;

        }

        private void btnKetNoi_Click(object sender, RoutedEventArgs e)
        {
            if (comList.Text == "")
            {
                MessageBox.Show("Xin vui lòng chọn cổng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                btnKetNoi.Content = "Kết nối";
                comList.IsEnabled = true;
            }
            else
            {
                try
                {

                    serialPort.PortName = comList.Text;
                    serialPort.BaudRate = 9600;
                    serialPort.Open();
                    btnKetNoi.Content = "Ngắt kết nối";
                    comList.IsEnabled = false;


                }
                catch
                {
                    MessageBox.Show("Không thể mở cổng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnXuatFile_Click(object sender, RoutedEventArgs e)
        {
            this.dataGrid.SelectAllCells();
            this.dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, this.dataGrid);
            this.dataGrid.UnselectAllCells();
            String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            string fileName = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Data file (*.csv)|*.csv";
            saveFileDialog.ShowDialog();
            fileName = saveFileDialog.FileName;
            if (fileName != "")
            {
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.GetEncoding("iso-8859-1"));
                sw.WriteLine(result);
                sw.Close();
                Process.Start(fileName);
            }
        }
    }
    public class duLieuClass
    {
        public double cNhietDo { get; set; }

        public double cDoAm { get; set; }

        public string cThoiGian { get; set; }
    }
    
}
