using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
using CloudPlatformInfo;
using NLECloudSDK;

//using 排队机;


namespace 排队机
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        //public string accesstoken;
        //public NLECloudAPI SDK = null;
        //MainWindow mainWindow = new MainWindow();
        public LoginWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;//窗口出现在屏幕的位置
        }
        
        public string schoolname;
       
        private void isremeberme (object sender ,RoutedEventArgs e)
        {

        }
        private void button_login_Click(object sender, RoutedEventArgs e)
        {
            AccountLoginDTO accountLoginDTO = new AccountLoginDTO();//实例化AccountLoginDTO
            {//将控件中的账号密码传入云平台
                accountLoginDTO.Account = username.Text;
                accountLoginDTO.Password = password.Password;
                accountLoginDTO.IsRememberMe = (click_remeberme.IsChecked == true);
            }
           // ResultMsg<AccountLoginResultDTO> resultMsg = SDK.UserLogin(accountLoginDTO);
            bool isLogin = ForLogin.UserLogin(accountLoginDTO);
            if(click_remeberme.IsChecked == true)
            {
                password.Password = accountLoginDTO.Password;
            }
            if(isLogin)
            {
                //schoolname = resultMsg.ResultObj.CollegeName;
               //accesstoken = resultMsg.ResultObj.AccessToken;
                MessageBox.Show("登录成功！，您位于" + accountLoginDTO.Account + "\n即将进入排队系统！", "登录成功");
                new MainWindow().Show();
                this.Close();
            }

            else
            {
                MessageBox.Show("登录失败!，请重试", "登录失败");
            }

        }
    }
}
