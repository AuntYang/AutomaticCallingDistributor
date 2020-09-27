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
using NLECloudSDK;
using System.Data;

namespace 排队机
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //LoginWindow loginWindow = new LoginWindow();
        public SeriesCollection SeriesCollection {get;set;}
        public List<string> Labels { get; set; }
        private double trend;
        private double[] temp = { 1, 3, 2, 4, 3, 5, 2, 1 };
        
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;


        }
        DispatcherTimer time;//实例化一个定时器
        
        void MainWindow_Loaded(object sender,RoutedEventArgs e)
        {
            time = new DispatcherTimer();
            time.Interval = TimeSpan.FromSeconds(5);
            time.Tick += time_tick;
            //time.Tick += livechart;
            //time.Tick += linestart;
            time.Start();

        }
        void time_tick (object sender,EventArgs e)
        {
            if(MainBusiness.numberPeople()!=0)
            {
                NumberTheCurrent.Text =  MainBusiness.numberPeople().ToString();
            }
            else
            {
                MessageBox.Show("设备"+TempInfo.deviceid+"不在线", "提示");
            }

            
          
        }
         void livechart(object sender , EventArgs e)
        {
            LineSeries myllineseries = new LineSeries();//实例化一条折线图
            myllineseries.Title = "人数";//设置折线的标题
            myllineseries.LineSmoothness = 0;//设置折线图的直线形式
            myllineseries.PointGeometry = null; //设置折线图的无点样式
            Labels = new List<string> { };//添加横坐标
            myllineseries.Values = new ChartValues<double>(temp);
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> { 3, 5, 7, 4 }
                },
                new ColumnSeries
                {
                    Values = new ChartValues<decimal> {5,6,2,7}
                }
            };
            SeriesCollection.Add(myllineseries);
            trend = 8;
            DataContext = this;
        }
        /*
        public void linestart(object sender,EventArgs e)
        {
            Labels.Add(DateTime.Now.ToString());
            Labels.RemoveAt(0);//更新横坐标时间
            SeriesCollection[0].Values.Add(trend);
            SeriesCollection[0].Values.RemoveAt(0);
        }
        */
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
