using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Modbus.Device;

namespace Signet.Common
{
    public class GlobalInfo
    {
        public static ObservableCollection<SqlSugarModel.Location_Table> Locations;
        //用户所在城市
        public static SqlSugarModel.Location_Table CurLocation;
        //用于解密证书的私钥
        public readonly static string License_PrivateKey = RegisterHelper.ReadFile("RsaPrivateKey");
        //是否已注册
        public static bool registerState = false;
        public static DateTime registerTime;

        public static ModbusSlave slave = null;
        public static ConfigService configService;
        //库存阈值
        public static int InventoryThreshold = 10;
    }
}
