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
        #region Khởi tạo biến
        public double nhietDoCaiDat = 50;
        #endregion

        #region Khởi tạo đồ thị
        public SeriesCollection SeriesCollection { get; set; }
        public List Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }
        #endregion
        #region Thiết lập timer
        DispatcherTimer timerDongHo = new DispatcherTimer();
        DispatcherTimer timerNhanDuLieu = new DispatcherTimer();
        #endregion
        #region Khai báo
        SerialPort serialPort = new SerialPort();
        #endregion
        #region Event Timer
        private void TimerDongHo_Tick(object sender, EventArgs e)
        {
            dongHo.Text = DateTime.Now.ToShortTimeString();
            if (serialPort.IsOpen)
            {
                try
                {
                    double dNhietDo = Convert.ToDouble(nhietDo.Text);
                    if (dNhietDo >= nhietDoCaiDat)
                    {
                        Window wdCanhBao = new CanhBao();
                        wdCanhBao.ShowDialog();
                    }
                }
                catch
                {

                }
            }            
        }

        private void TimerNhanDuLieu_Tick(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                trangThaiKetNoi.Kind = MaterialDesignThemes.Wpf.PackIconKind.LinkVariantOff;
            }
            else if (serialPort.IsOpen)
            {
                trangThaiKetNoi.Kind = MaterialDesignThemes.Wpf.PackIconKind.LinkVariant;
                string duLieuNhan = serialPort.ReadLine().ToString();
                string[] arrListStr = duLieuNhan.Split(',');
                string NhietDo = arrListStr[0];
                string DoAm = arrListStr[1];
                string KhongKhi = arrListStr[2];
                nhietDo.Text = NhietDo;
                doAm.Text = DoAm;
                khongKhi.Text = KhongKhi;
                SeriesCollection[0].Values.Add(Convert.ToDouble(NhietDo));
                SeriesCollection[1].Values.Add(Convert.ToDouble(DoAm));
                SeriesCollection[2].Values.Add(Convert.ToDouble(KhongKhi));
                dataGrid.Items.Add(new Data()
                {
                    cNgay = DateTime.Now.ToShortDateString(),
                    cNhietDo = Convert.ToDouble(NhietDo),
                    cDoAm = Convert.ToDouble(DoAm),
                    cThoiGian = DateTime.Now.ToShortTimeString(),
                    cKhongKhi = Convert.ToDouble(KhongKhi)
                });
            }
        }
        #endregion
        #region Event load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timerDongHo.Start();
            string[] nhietDoCbb = { "28°C", "30°C", "35°C", "38°C", "40°C" };
            foreach (string item in nhietDoCbb)
            {
                cbbNhietDo.Items.Add(item);
            }
            string[] danhSachCongCOM = SerialPort.GetPortNames();
            int[] danhSoCongCOM = new int[danhSachCongCOM.Length];
            for (int i = 0; i < danhSachCongCOM.Length; i++)
            {
                danhSoCongCOM[i] = Convert.ToInt32(danhSachCongCOM[i].Substring(3));
            }
            Array.Sort(danhSoCongCOM);
            foreach (int item in danhSoCongCOM)
            {
                comList.Items.Add("COM" + item.ToString());
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
                }
            };
            YFormatter = value => value.ToString("");
            DataContext = this;
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            string[] listBaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comBaudrate.ItemsSource = listBaudRate;
            string[] listSpeed = { "1", "2", "3", "4", "5", "10", "15", "30" };
            comTocDo.ItemsSource = listSpeed;
            timerNhanDuLieu.Tick += TimerNhanDuLieu_Tick;
            timerDongHo.Tick += TimerDongHo_Tick;
            timerNhanDuLieu.Interval = new TimeSpan(0, 0, 1);
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
        private void btnSaveCSV_Click(object sender, RoutedEventArgs e)
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
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8);
                sw.WriteLine(result);
                sw.Close();
                Process.Start(fileName);
            }
        }
        #endregion
        #region Đồ thị
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
        #endregion
        #region Cài đặt
        private void thietDat(object sender, RoutedEventArgs e)
        {
            if (cbbNhietDo.SelectedItem != null)
            {
                nhietDoCaiDat = Convert.ToDouble(cbbNhietDo.SelectedItem.ToString().Substring(0, cbbNhietDo.SelectedItem.ToString().Length - 2));
                MessageBox.Show("Cài đặt thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không hợp lệ!!!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void huyThietDat(object sender, RoutedEventArgs e)
        {
            nhietDoCaiDat = 40;
            MessageBox.Show("Khôi phục thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
        #region Thông tin
        private void btnOpenVideoHDSD_Click(object sender, RoutedEventArgs e)
        {
            Window wdVideo = new VideoHuongDan();
            wdVideo.Show();
        }

        private void btnOpenGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/ngtrdai/DocGhiNhietDoTuArduino");

        }
        private void btnPhone1_Click(object sender, RoutedEventArgs e)
        {
            string thongTin = "Tên: Nguyễn Trọng Đại\r\nSĐT: 0979.808.617\r\nEmail: 19146146@student.hcmute.edu.vn";
            MessageBox.Show(thongTin, "THÔNG TIN LIÊN LẠC", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btnMail1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"mailto:19146146@student.hcmute.edu.vn");
        }

        private void btnGit1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/ngtrdai");
        }

        private void btnFacebook1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://facebook.com/nguyendai5901");
        }
        private void btnPhone2_Click(object sender, RoutedEventArgs e)
        {
            string thongTin = "Tên: Trần Triệu Vĩ\r\nSĐT: 0XXX.XXX.XXX\r\nEmail: 19146301@student.hcmute.edu.vn";
            MessageBox.Show(thongTin, "THÔNG TIN LIÊN LẠC", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btnMail2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"mailto:19146301@student.hcmute.edu.vn");
        }

        private void btnGit2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/trantrieuvi");
        }

        private void btnFacebook2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://www.facebook.com/trieuvihh111");
        }
        #endregion


    }
    #region Class
    public class Data
    {
        public string cNgay { get; set; }
        public double cNhietDo { get; set; }
        public double cDoAm { get; set; }
        public string cThoiGian { get; set; }
        public double cKhongKhi { get; set; }
    }
    #endregion
}
