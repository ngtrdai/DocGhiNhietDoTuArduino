using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DocGhiNhietDoTuArduino
{
    /// <summary>
    /// Interaction logic for VideoHuongDan.xaml
    /// </summary>
    public partial class VideoHuongDan : Window
    {
        private bool videoDangChay = false;
        private bool dangKeoSlider = false;
        public VideoHuongDan()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mdMain.NaturalDuration.HasTimeSpan && !dangKeoSlider)
            {
                slider.Minimum = 0;
                slider.Maximum = mdMain.NaturalDuration.TimeSpan.TotalSeconds;
                slider.Value = mdMain.Position.TotalSeconds;
            }
        }
        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mdMain != null) && (mdMain.Source != null);
        }
        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mdMain.Play();
            videoDangChay = true;
        }
        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = videoDangChay;
        }
        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mdMain.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = videoDangChay;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mdMain.Stop();
            videoDangChay = false;
        }

        private void slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            dangKeoSlider = true;
        }

        private void slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            dangKeoSlider = false;
            mdMain.Position = TimeSpan.FromSeconds(slider.Value);
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelDuration.Text = TimeSpan.FromSeconds(slider.Value).ToString(@"hh\:mm\:ss");
        }
    }
}
