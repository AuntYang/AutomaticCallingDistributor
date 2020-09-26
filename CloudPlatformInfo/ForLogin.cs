using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLECloudSDK;


namespace CloudPlatformInfo
{
    public class ForLogin
    {
        private static NLECloudAPI SDK = null;
        
        public static bool UserLogin(AccountLoginDTO accountLoginDTO)
        {//以布尔值的形式返回给LoginWindow登录成功/失败信息
            SDK = new NLECloudAPI(TempInfo.API_HOST);
            var yxh = SDK.UserLogin(accountLoginDTO);
            if(yxh.IsSuccess())
            {
                TempInfo.Token = yxh.ResultObj.AccessToken;
                return true;
            }
            return false;
        }

        
    }
}
