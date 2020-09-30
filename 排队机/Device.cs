using NLECloudSDK;
using NLECloudSDK.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfApp1
{
    public struct sensorData
    {
        public String apiTag;
        public String value;
        public String time;
    };
    public static class Device
    {
        public static String deviceID = "90596";
        public static String accessToken;
        public static NLECloudAPI SDK = null;
    }

    public interface nlecloud
    {
        bool UserLogin(String userName, String passWord);
        bool cmds(String apiTag, object data);
        List<Dictionary<string, string>> GetSensorDatas(String apiTag, String sort = "DESC");
        bool AddSensorData(sensorData sensordata);
        bool DeviceOnline(String deviceTag, String secretKey);
        bool CloseSocket();
        bool SocketSendData(sensorData sensor);
        bool GetDeviceStatus();
    }

    public class Cloud : nlecloud
    {
        private Socket socketSend = null;
        private DispatcherTimer timer = null;
        public Cloud(String deviceID)
        {
            Device.deviceID = deviceID;
        }
        public Cloud() {

        }
        ///<param name = "userName">用户名</param>
        ///<param name = "passWord">密码</param>
        ///<returns>true表示成功,false表示失败
        ///</returns>
        public bool UserLogin(String userName, String passWord)
        {
            Device.SDK = new NLECloudAPI("http://api.nlecloud.com");
            AccountLoginDTO submitData = new AccountLoginDTO()
            {
                Account = userName,
                Password = passWord
            };
            ResultMsg<AccountLoginResultDTO> userLogin = Device.SDK.UserLogin(submitData);
            if (userLogin.IsSuccess())
            {
                Device.accessToken = userLogin.ResultObj.AccessToken;
                return true;
            }
            Log(new StackTrace(new StackFrame(true)).GetFrame(0), userLogin.Msg);
            return false;
        }
        ///<param name = "apiTag">传感器标识符</param>
        ///<param name = "data">数据</param>
        ///<returns>true表示成功,false表示设备不在线或传感器设备出错.
        ///</returns>
        public bool cmds(String apiTag, object data)
        {
            Result result = Device.SDK.Cmds(Convert.ToInt32(Device.deviceID), apiTag, data, Device.accessToken);
            if (result.IsSuccess())
            {
                return true;
            }
            Log(new StackTrace(new StackFrame(true)).GetFrame(0), result.Msg);
            return false;
        }
        ///<param name = "apiTag">传感器标识符</param>
        ///<param name="sort">排序方式</param>
        ///<summary>获取传感数据
        ///</summary>
        ///<returns>一个列表.
        ///</returns>
        public List<Dictionary<string, string>> GetSensorDatas(String apiTag, String sort = "DESC")
        {
            dynamic qry = new SensorDataFuzzyQryPagingParas()
            {
                DeviceID = Convert.ToInt32(Device.deviceID),
                ApiTags = apiTag,
                Method = 4,
                TimeAgo = 30,
                StartDate = "2019-01-01 01:01:01",
                EndDate = "2021-01-01 01:01:01",
                PageSize = 50,
                Sort = sort,
                PageIndex = 1
            };
            ResultMsg<SensorDataPageDTO> sensorDatas = Device.SDK.GetSensorDatas(qry, Device.accessToken);
            IEnumerable<SensorDataAddDTO> sensorDTO = sensorDatas.ResultObj.DataPoints;
            List<Dictionary<string, string>> sensorInfo = new List<Dictionary<string, string>>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                foreach (var l in sensorDTO)
                {
                    foreach (var m in l.PointDTO)
                    {
                        dictionary.Add("value", m.Value.ToString());
                        dictionary.Add("time", m.RecordTime);
                        sensorInfo.Add(dictionary);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("暂无传感信息 + " + ex);
            }
            return sensorInfo;
        }

        ///<param name = "sensorData">sensordata结构体</param>
        ///<returns>true表示成功,false表示设备不在线或传感器设备出错.
        ///</returns>
        ///<summary>上传传感数据</summary>
        public bool AddSensorData(sensorData sensordata)
        {
            DateTime dt;
            if (sensordata.apiTag == "" || sensordata.value == "" || !DateTime.TryParse(sensordata.time, out dt))
            {
                Log(new StackTrace(new StackFrame(true)).GetFrame(0), "格式错误");
                return false;
            }
            var dto = new SensorDataListAddDTO();
            dto.DatasDTO = new List<SensorDataAddDTO>()
            {
                new SensorDataAddDTO()
                {
                    ApiTag = sensordata.apiTag,
                    PointDTO = new List<SensorDataPointDTO>()
                    {
                        new SensorDataPointDTO()
                        {
                            Value = sensordata.value,
                            RecordTime = sensordata.time
                        }
                    }
                },
            };
            Result result = Device.SDK.AddSensorDatas(Convert.ToInt32(Device.deviceID), dto, Device.accessToken);
            if (result.IsSuccess())
            {
                return true;
            }
            Log(new StackTrace(new StackFrame(true)).GetFrame(0), "数据上传失败");
            return false;
        }
        ///<param name = "sensorDatas">sensordata结构体</param>
        ///<returns>true表示成功,false表示设备不在线或传感器设备出错.
        ///</returns>
        ///<summary>上传传感数据</summary>
        public bool AddSensorDatas(List<sensorData> sensorDatas)
        {
            DateTime dt;
            var dto = new SensorDataListAddDTO();
            List<SensorDataAddDTO> l = new List<SensorDataAddDTO>();
            foreach (var sensor in sensorDatas) {

                if (sensor.apiTag == "" || sensor.value == "" || !DateTime.TryParse(sensor.time, out dt))
                {
                    Log(new StackTrace(new StackFrame(true)).GetFrame(0), "格式错误");
                    return false;
                }
                var sensorDataAddDTO = new SensorDataAddDTO();
                sensorDataAddDTO.ApiTag = sensor.apiTag;
                sensorDataAddDTO.PointDTO = new List<SensorDataPointDTO>()
                {
                    new SensorDataPointDTO()
                        {
                            Value = sensor.value,
                            RecordTime = sensor.time
                        }
                };
                l.Add(sensorDataAddDTO);
            }
            dto.DatasDTO = l;
            Result result = Device.SDK.AddSensorDatas(Convert.ToInt32(Device.deviceID), dto, Device.accessToken);
            if (result.IsSuccess())
            {
                return true;
            }
            Log(new StackTrace(new StackFrame(true)).GetFrame(0), "数据上传失败");
            return false;
        }
        /// <summary>
        /// 随机上传传感数据
        /// </summary>
        /// <param name="apiTags">标识符数组</param>
        /// <param name="minValue">随机最小值</param>
        /// <param name="maxValue">随机最大值</param>
        /// <param name="count">默认5条</param>
        /// <returns>true表示成功,false表示失败</returns>
        public bool RandomAddSensorDatas(String[] apiTags, int[] minValue, int[] maxValue, int count=5)
        {
            List<sensorData> sensorDatas = new List<sensorData>();
            for (var i = 0; i < apiTags.Length; i++)
            {
                var r = new Random();
                for (var j = 0; j < count; j++)
                {
                    sensorData sensor = new sensorData()
                    {
                        apiTag = apiTags[i],
                        value = Convert.ToString(r.Next(minValue[i], maxValue[i])),
                        time = DateTime.Now.ToString()
                    };
                    sensorDatas.Add(sensor);
                }
            }
            if (AddSensorDatas(sensorDatas))
            {
                return true;
            }
            return false;
        }



        ///<param name = "deviceTag">设备标识符</param>
        ///<param name = "secretKey">秘钥</param>
        ///<returns>true表示成功,false表示设备不在线或传感器设备出错.
        ///</returns>
        ///<summary>设备模拟在线</summary>
        public bool DeviceOnline(String deviceTag,String secretKey)
        {
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse("117.78.1.201");
            socketSend.Connect(ipaddress, Convert.ToInt32("8600"));
            //发送消息.  
            string strMsg = "{\"t\":1,\"device\":\""  +deviceTag + 
                "\",\"key\":\"" + secretKey+"\",\"ver\":\"v0.0.0.0\"}";
            try
            {
                //string strMsg = this.txt_Msg.Text.Trim();  
                byte[] buffer = new byte[2048];
                buffer = Encoding.Default.GetBytes(strMsg);
                int receive = socketSend.Send(buffer);
                //开启定时器,发送心跳.  
                timer = new DispatcherTimer();//创建timer定时器      
                timer.Interval = TimeSpan.FromMilliseconds(2000);//设置周期为4000毫秒      
                timer.Tick += new EventHandler(Refresh_Time);
                timer.Start();
                Log(new StackTrace(new StackFrame(true)).GetFrame(0),"设备在线");
            }
            catch (Exception ex)
            {
                Log(new StackTrace(new StackFrame(true)).GetFrame(0), "发送消息出错:"+ ex.Message);
            }
            return false;
        }
        public void Refresh_Time(object sender, EventArgs e)
        {
            //发送心跳  
            String cmd = "$#AT#\r";
            byte[] buffer = new byte[10];
            buffer = Encoding.Default.GetBytes(cmd);
            socketSend.Send(buffer);
        }
        public bool CloseSocket()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
                socketSend.Close();
                socketSend = null;
                return true;
            }
            return false;
        }
        ///<param name = "sensor">sensorData结构体成员</param>
        ///<returns>true表示成功,false表示设备不在线或传感器设备出错.
        ///</returns>
        ///<summary>设备模拟发送数据</summary>
        public bool SocketSendData(sensorData sensor)
        {
            if (socketSend != null)
            {
                String cmd = "{\"t\":3,\"datatype\":2,\"datas\":{\"" + sensor.apiTag+
                    "\":{\""+sensor.time +"\":"+sensor.value +"}},\"msgid\":001}";
                byte[] buffer = new byte[105];
                buffer = Encoding.Default.GetBytes(cmd);
                socketSend.Send(buffer);
                Log(new StackTrace(new StackFrame(true)).GetFrame(0), "数据发送成功");
                return true;
            }
            return false;
        }

        ///<returns>true表示在线,false表示设备不在线
        ///</returns>
        ///<summary>获取设备在线状态</summary>
        public bool GetDeviceStatus()
        {
            ResultMsg<IEnumerable<OnlineDataDTO>> online = Device.SDK.GetDevicesStatus(Device.deviceID, Device.accessToken);
            foreach(var l in online.ResultObj)
            {
                if (l.IsOnline)
                    return l.IsOnline;
            }
            Log(new StackTrace(new StackFrame(true)).GetFrame(0), "设备不在线");
            return false;
        }
        /// <summary>
        /// 查询单个传感器
        /// </summary>
        /// <param name="apiTag"></param>
        /// <returns></returns>
        public String GetSensorInfo(String apiTag)
        {
            ResultMsg<SensorBaseInfoDTO> sensor = Device.SDK.GetSensorInfo(Convert.ToInt32(Device.deviceID), apiTag, Device.accessToken);
            if (sensor.ResultObj != null)
            {
                Log(new StackTrace(new StackFrame(true)).GetFrame(0), "传感器数据:" + sensor.ResultObj.Value.ToString());
                return sensor.ResultObj.Value.ToString();
            }
            return "暂无数据";
        }
        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="sf">获取栈指针</param>
        /// <param name="msg">打印的消息</param>
        private void Log(StackFrame sf,String msg)
        {                                    
            Console.WriteLine(" Method: {0}", sf.GetMethod().Name);                         //函数名
            Console.WriteLine(" Line Number: {0},Msg is {1}", sf.GetFileLineNumber(),msg);   //文件行号
            Console.WriteLine(" ----------------\r\n"); 
        }
    }


}
