using System;
using System.Collections.Generic;
using System.Text;
using NLECloudSDK;
using NLECloudSDK.Model;
 
using System.Windows;
using System.Numerics;

namespace CloudPlatformInfo
{
    public class MainBusiness
    {
        static NLECloudAPI SDK = new NLECloudAPI(TempInfo.API_HOST);//用tempinfo类中存储的API_HOST初始化sdk
        
        
        public static ActuatorAddUpdate Began()
        {//开始取号
            var yxh = SDK.Cmds(119374, "bool_work",1, TempInfo.Token);
            return null;
        }

        public static ActuatorAddUpdate Stop()
        {//暂停取号
            var yxh = SDK.Cmds(119374, "bool_work", 0, TempInfo.Token);
            return null;
        }

        public static ActuatorAddUpdate yourTurn()
        {//叫号
            var yxh = SDK.Cmds(119374, "number_down",1,TempInfo.Token);
            return null;
        }
        public static ActuatorAddUpdate broadcast(string a)
        {//语音播报上传字符型数据
            var yxh = SDK.Cmds(119374, "string_play",a, TempInfo.Token);
            return null;
        }
        public static int numberPeople()
        {//当前排队人数
            var yxh = SDK.GetSensorInfo(119374,"number_up", TempInfo.Token);
            if(yxh.ResultObj.Value != null)
            {
                return int.Parse(yxh.ResultObj.Value.ToString());//注意类型转换
            }
            else
            {
                return 00;
            }
           
            
            
            
            /*
            if (yxh.IsSuccess())
            {
                return (int) yxh.ResultObj.Value;
            }
            else
            {
                return 0;
            }
            /*
            SensorDataFuzzyQryPagingParas query = new SensorDataFuzzyQryPagingParas()
            {
                DeviceID=119374,
                ApiTags = "number_up"
            };
            var yxh = SDK.GetSensorDatas(query, TempInfo.API_HOST);
            if(yxh.IsSuccess())
            {
                return yxh.ResultObj.Count;
            }
            else
            {
                return 0;
            }
            */
        }
    }
}
