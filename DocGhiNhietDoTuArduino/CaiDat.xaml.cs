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
using System.Windows.Shapes;

namespace DocGhiNhietDoTuArduino
{
    /// <summary>
    /// Interaction logic for CaiDat.xaml
    /// </summary>
    public partial class CaiDat : Window
    {
        public event EventHandler ThayDoiCaiDat;
        public int BaudRate { get; set; }
        public int TocDoTruyen { get; set; }

        public CaiDat()
        {
            InitializeComponent();
            string[] listBaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            comBaudrate.ItemsSource = listBaudRate;
            string[] listSpeed = { "1", "2", "3", "4", "5", "10", "15","30" };
            comTocDo.ItemsSource = listSpeed;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnThayDoi_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
