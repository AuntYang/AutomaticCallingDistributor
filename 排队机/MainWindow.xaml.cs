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
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Threading;
using CloudPlatformInfo;

namespace 排队机
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginWindow loginWindow = new LoginWindow();
        
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;


        }
        DispatcherTimer time;//实例化一个定时器
        
        void MainWindow_Loaded(object sender,RoutedEventArgs e)
        {
            time = new DispatcherTimer();
            time.Interval = TimeSpan.FromSeconds(5000);
            time.Tick += time_tick;
            time.Start();

        }
        void time_tick (object sender,EventArgs e)
        {
            NumberTheCurrent.Text =  MainBusiness.numberPeople().ToString();
        }

        private void Button_Click_Began(object sender, RoutedEventArgs e)
        {//开始取号点击事件
            MainBusiness.Began();
        }
        private void Button_Click_Stop(object sender,RoutedEventArgs e)
        {//暂停取号点击事件
            MainBusiness.Stop();
        }
        private void Button_Click_YourTurn(object sender, RoutedEventArgs e)
        {//叫号点击事件
            MainBusiness.yourTurn();
        }
        private void Button_Click_TextSender(object sender,RoutedEventArgs e)
        {//语音播报发送点击时间
            if(text_sender.Text == null)
            {
                MessageBox.Show("要发送的文本不能为空！", "提示");
            }
            else
            {
                MainBusiness.broadcast(text_sender.Text);
            }
            
        }
        
        private void Button_Click_Export(object sender,RoutedEventArgs e)
        {//导出图表点击事件

        }
        private void Button_Click_Import(object sender, RoutedEventArgs e)
        {//导入图表点击事件

        }

        private void Label_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
