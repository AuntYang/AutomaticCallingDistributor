using NLECloudSDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CloudPlatformInfo
{
    public static class TempInfo
    {//存储登录后的accesstoken
        public static String API_HOST = "http://api.nlecloud.com";
            //ApplicationSettings.Get("ApiHost");

        public static String Token;//登录成功后返回的Token
    }
}
