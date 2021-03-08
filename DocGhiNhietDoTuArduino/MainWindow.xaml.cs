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

    public partial class MainWindow : Window
    {
        #region Khởi tạo
        //List<int> baudRate = new List<int>; 
        #endregion
        #region Khởi tạo đồ thị
        public SeriesCollection SeriesCollection { get; set; }
        public List Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        #endregion
        #region Thiết lập timer
        DispatcherTimer timerDongHo = new DispatcherTimer();
        DispatcherTimer timerNhanDuLieu = new DispatcherTimer();
        DateTime DongHo = new DateTime();
        #endregion
        #region Khai báo
        SerialPort serialPort = new SerialPort();
        #endregion
        #region Event Timer
        private void TimerDongHo_Tick(object sender, EventArgs e)
        {
            dongHo.Text = DateTime.Now.ToShortTimeString();
        }

        private void TimerNhanDuLieu_Tick(object sender, EventArgs e)
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
                string DoAm = arrListStr[1];
                string KhongKhi = arrListStr[2];
                string DoAmDat = arrListStr[3].Substring(0, arrListStr[3].Length - 1);
                nhietDo.Text = NhietDo;
                doAm.Text = DoAm;
                khongKhi.Text = KhongKhi;
                doAmDat.Text = DoAmDat;
                SeriesCollection[0].Values.Add(Convert.ToDouble(NhietDo));
                SeriesCollection[1].Values.Add(Convert.ToDouble(DoAm));
                SeriesCollection[2].Values.Add(Convert.ToDouble(KhongKhi));
                SeriesCollection[3].Values.Add(Convert.ToDouble(DoAmDat));
                dataGrid.Items.Add(new Data()
                {
                    cNgay = DateTime.Now.ToShortDateString(),
                    cNhietDo = Convert.ToDouble(NhietDo),
                    cDoAm = Convert.ToDouble(DoAm),
                    cThoiGian = DateTime.Now.ToShortTimeString(),
                    cKhongKhi = Convert.ToDouble(KhongKhi),
                    cDoAmDat = Convert.ToDouble(DoAmDat)
                });
            }
        }
        #endregion
        #region Event load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timerDongHo.Start();
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
                },
                new LineSeries
                {
                    Title = "Không Khí",
                    Values = new ChartValues<double>{}
                },
                new LineSeries
                {
                    Title = "Độ Ẩm Đất",
                    Values = new ChartValues<double>{}
                }
            };

            YFormatter = value => value.ToString("");
            DataContext = this;
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            timerNhanDuLieu.Tick += TimerNhanDuLieu_Tick;
            timerDongHo.Tick += TimerDongHo_Tick;
            timerNhanDuLieu.Interval = new TimeSpan(0, 0, 5);
            timerNhanDuLieu.Start();
        }
        #region Kết nối
        private void btnKetNoi_Click(object sender, RoutedEventArgs e)
        {
            if (comList.Text == "")
            {
                MessageBox.Show("Xin vui lòng chọn cổng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
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
        }
        #endregion
        #region Lưu Excel
        private void Button_Click(object sender, RoutedEventArgs e)
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
        #endregion
        private void lstMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMenu.SelectedIndex == 0)
            {
                Window wCaiDat = new CaiDat();
                wCaiDat.ShowDialog();
            }
            else if (lstMenu.SelectedIndex == 1)
            {
                string thongTin = 
                    "PHẦN MỀM ĐỌC GHI DỮ LIỆU MÔI TRƯỜNG\r\n" +
                    "Sử dụng cảm biến DHT22, MQ135, SMS-V2\r\n" +
                    "Môn học: Cảm biến và cơ cấu chấp hành\r\n" +
                    "Thành viên: Nguyễn Trọng Đại, Trần Triệu Vĩ" +
                    "\r\n----------------" +
                    "\r\nTác giả: Nguyễn Trọng Đại - @ngtrdai\r\n" +
                    "Phiên bản phần mềm: v1.1\r\n" +
                    "Github: ngtrdai/DocGhiNhietDoTuArduino"
                    ;
                MessageBox.Show(thongTin,"Thông tin phần mềm",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            
        }

        private void btnLuuDoThi_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = new RenderTargetBitmap((int)DoThi.ActualWidth, (int)DoThi.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(DoThi);
            var frame = BitmapFrame.Create(bitmap);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(frame);
            string fileName = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG file (*.png)|*.png";
            saveFileDialog.ShowDialog();
            fileName = saveFileDialog.FileName;
            if (fileName != "")
            {
                using (var stream = File.Create(fileName)) encoder.Save(stream);
            }            
        }
    }
    public class Data
    {
        public string cNgay { get; set; }
        public double cNhietDo { get; set; }
        public double cDoAm { get; set; }
        public string cThoiGian { get; set; }
        public double cKhongKhi { get; set; }
        public double cDoAmDat { get; set; }
    }

}
