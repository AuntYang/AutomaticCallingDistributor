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
using System.IO;

namespace 排队机
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region -- 参数设置 --
        //LoginWindow loginWindow = new LoginWindow();
        public SeriesCollection SeriesCollection { get; set; }//存放
        public List<string> Labels { get; set; }//横坐标,存放图表x轴数据
        public List<int> NumY { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,};//存放图表y轴数据
        private int[] temp = { 0, 0, 0, 0, 0, 0 ,0 , 0 , 0 , 0 };//横坐标初始值
        //private string[] ExportNum = new string[100];//存放导出的人数
        //private string[] ExportTime =new string[100]; //存放导出的时间

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
                NumberTheCurrent.FontSize = 36;
                NumberTheCurrent.Text =  MainBusiness.numberPeople().ToString();
                /*
                 * 直接取用LiveCharts的图表数据，不再使用本代码
                 * 
                 * 
                for (int i = 0; i < 100; i++)
                {//存放待导出的数据
                    ExportNum[i] = MainBusiness.numberPeople().ToString();
                    ExportTime[i] = DateTime.Now.ToString();
                    //MessageBox.Show(ExportNum[i] + ExportTime[i], "test");//测试提示框
                }
                */
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
            Labels = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            mylineseries.Values = new ChartValues<int>(temp);
            myAxisX.Separator.Step = 1;//设置轴间距，设置为0.5时数值1分为两格
            myAxisY.Separator.Step = 1;
            SeriesCollection = new SeriesCollection { };
            SeriesCollection.Add(mylineseries);
            linestart(null, null);
            DataContext = this;
        }

        
        public void linestart(object sender, EventArgs e)
        {//折线图绘制函数
            Labels.Add(DateTime.Now.ToString("HH:mm:s"));
            Labels.RemoveAt(0);
            SeriesCollection[0].Values.Add(MainBusiness.numberPeople());
            SeriesCollection[0].Values.RemoveAt(0);
            NumY.Add(MainBusiness.numberPeople());
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
            if(text_sender.Text == "")
            {
                MessageBox.Show("要发送的文本不能为空！", "提示");
            }
            else
            {
                MainBusiness.broadcast(text_sender.Text);
            }
        }
        #endregion

        #region -- 导入导出功能 -- 
        private void Button_Click_Export(object sender,RoutedEventArgs e)
        {//导出图表点击事件
         // ExportFunction();
            string[] sttime = new string[10];
            int[] stnum = new int[10];
            sttime = Labels.ToArray();//将List<string> Labels存入string[] st 数组做流输出，存放横坐标的时间数据
            stnum = NumY.ToArray();
            //MessageBox.Show(st[9], "test");//测试消息框

            ExportFunction(sttime, stnum);

        }
        private void Button_Click_Import(object sender, RoutedEventArgs e)
        {//导入图表点击事件
            OutputFunction(Labels.ToArray(),NumY.ToArray());//调用流输入函数对动态图表的xy轴数据存储list进行重写
            SeriesCollection[0].Values.Add(NumY.ToArray());//因为没找到y轴数据对应的Ilist中存放数据的list，所以自己建立一个同步存放的list进行重写
        }
        #endregion

        #region -- 流输入输出函数 --
        static void ExportFunction(string[] argstime,int[] argsnum)
        {//流输出函数
            int Params = 2;//存放本函数的参数个数
            //MessageBox.Show(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "test");//测试消息框
            FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\chartData.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(argsnum.Length+","+Params);//再写入数据前写入行列信息，2为列数，即参数个数
            for(int i = 0;i<argsnum.Length;i++)
            {//写入参数接受数组的值
                sw.Write("\n"+argstime[i]+","+argsnum[i]);       
            }
            sw.Flush();//结束流
            sw.Close();//关闭流
            fs.Close();//手动关闭文件，也可以用using自动回收
        }


        static void OutputFunction(string[] argstime,int[] argsnum )
        {//流输入函数
            FileStream file = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\chartData.txt", FileMode.Open);
            StreamReader sw = new StreamReader(file);
            //以上两行代码可以简化为:
            //StreamReader sw = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\chartData.txt");
            string FileInformation = sw.ReadLine();//存放txt文件第一行的数据行列信息
            string[] AnArrayInformation = FileInformation.Split(',');//将上面读取的数据行列信息进行分割
            int row = int.Parse(AnArrayInformation[0]);//将分割后的数据行数量信息强制由string型转换为int型赋值给row
            int col = int.Parse(AnArrayInformation[1]);//同上，列数量信息
            for(int i = 0; i < row; i++)
            {//通过行列数量信息格式化读取txt文件,外层嵌套读取行
                string TemporaryLine = sw.ReadLine();//暂时存放列读取的数据，等待下一步分割
                string[] TemporaryArray = TemporaryLine.Split(',');//对上一步暂存的数据进行分割，分隔符：，
                argstime[i] = TemporaryArray[0];//将逻辑第一列存入
                argsnum[i] = int.Parse(TemporaryArray[1]);//将逻辑第二行进行类型转换后存入

                /*
                 * 读取二维数组时使用
                for (int j = 0; j < col; j++)
                {//内层嵌套读取列
                    
                }
                */

            }
            sw.Close();//关闭流
            file.Close();//关闭文件
        }
        #endregion
    }
}
