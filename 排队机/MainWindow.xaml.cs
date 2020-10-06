using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        #region -- 参数设置 --
        //LoginWindow loginWindow = new LoginWindow();
        public SeriesCollection SeriesCollection { get; set; }//折线图
        public List<string> Labels { get; set; }//横坐标
        private int[] temp = { 0, 0, 0, 0, 0, 0 ,0,0,0,0};

        #endregion

        #region -- 定时器 --
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;//窗口出现在屏幕的位置
            livechart();
        }
        DispatcherTimer time;//实例化一个定时器
        
        void MainWindow_Loaded(object sender,RoutedEventArgs e)
        {
            time = new DispatcherTimer();
            time.Interval = TimeSpan.FromSeconds(5);
            time.Tick += CurrentLineNumber;
            //time.Tick += livechart;
            
            time.Tick += linestart;
            time.Start();


        }
        #endregion

        #region -- 当前人数函数 --
        void CurrentLineNumber(object sender,EventArgs e)
        {
            //计时刷新当前人数
            if(MainBusiness.numberPeople()!= 00)
            {
                NumberTheCurrent.Text =  MainBusiness.numberPeople().ToString();
                
               
            }
            else
            {
                NumberTheCurrent.FontSize = 16;
                NumberTheCurrent.Text = "暂无数据";
            }
        }
        #endregion
        
        #region -- 动态图表 --
        public void livechart()
        {//创建折线图函数
            LineSeries mylineseries = new LineSeries();
            mylineseries.Title = "当前人数";
            //mylineseries.LineSmoothness = 0;//折线图直线形式
            //mylineseries.PointGeometry = null;//折线图的无点样式
            Labels = new List<string> { "", "", "", "", "", "", "", "", "", "" };
            mylineseries.Values = new ChartValues<int>(temp);
            myAxisX.Separator.Step = 1;//设置轴间距，0.5时数值1分为两格
            myAxisY.Separator.Step = 1;
            SeriesCollection = new SeriesCollection { };
            SeriesCollection.Add(mylineseries);
            linestart(null, null);
            DataContext = this;
        }

        
        public void linestart(object sender, EventArgs e)
        {//折线图绘制函数
            Labels.Add(DateTime.Now.ToString());
            Labels.RemoveAt(0);
            SeriesCollection[0].Values.Add(MainBusiness.numberPeople());
            SeriesCollection[0].Values.RemoveAt(0);
        }
        
            #endregion

        #region -- 按钮功能实现 --
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
        #endregion

    }
}
