using GalaSoft.MvvmLight;
using Signet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.Model
{
    public class InventoryOperation_Model: ValidateModelBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        private string _Header;
        public string Header
        {
            get { return _Header; }
            set { _Header = value; RaisePropertyChanged(() => Header); }
        }

        /// <summary>
        /// 用户出入库时间
        /// </summary>
        private DateTime _UserTime;
        [Required(ErrorMessage = "请选择出入库时间")]
        public DateTime UserTime
        {
            get { return _UserTime; }
            set { _UserTime = value; RaisePropertyChanged(() => UserTime); }
        }

        /// <summary>
        /// 数量
        /// </summary>
        private int _Num;
        [Required(ErrorMessage = "请输入数量")]
        public int Num
        {
            get { return _Num; }
            set { _Num = value; RaisePropertyChanged(() => Num); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string _Remark;
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; RaisePropertyChanged(() => Remark); }
        }
    }
}
