using Signet.Common;
using Signet.SqlSugarModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Signet.Model
{
    public class GoodsManage_Model : ValidateModelBase
    {
        //标题
        private string _Header;
        public string Header
        {
            get { return _Header; }
            set { _Header = value; RaisePropertyChanged(() => Header); }
        }

        /// <summary>
        /// 物品编码
        /// </summary>
        private string _GoodsCode;
        public string GoodsCode
        {
            get { return _GoodsCode; }
            set { _GoodsCode = value; RaisePropertyChanged(() => GoodsCode); }
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        private string _GoodsName;
        [Required(ErrorMessage = "物品名称不能为空")]
        public string GoodsName
        {
            get { return _GoodsName; }
            set { _GoodsName = value; RaisePropertyChanged(() => GoodsName); }
        }

        /// <summary>
        /// 物品型号
        /// </summary>
        private string _GoodsModel;
        [Required(ErrorMessage = "物品型号不能为空")]
        public string GoodsModel
        {
            get { return _GoodsModel; }
            set { _GoodsModel = value; RaisePropertyChanged(() => GoodsModel); }
        }


        /// <summary>
        /// 物资类型列表
        /// </summary>
        private ObservableCollection<GoodsType_Table> _GoodsTypeList;
        public ObservableCollection<GoodsType_Table> GoodsTypeList
        {
            get { return _GoodsTypeList; }
            set { _GoodsTypeList = value; RaisePropertyChanged(() => GoodsTypeList); }
        }

        /// <summary>
        /// 选中的物资类型
        /// </summary>
        private GoodsType_Table _SelectedGoodsType;
        [Required(ErrorMessage = "物资类型不能为空")]
        public GoodsType_Table SelectedGoodsType
        {
            get { return _SelectedGoodsType; }
            set { _SelectedGoodsType = value; RaisePropertyChanged(() => SelectedGoodsType); }
        }


        /// <summary>
        /// 单位列表
        /// </summary>
        private ObservableCollection<Unit_Table> _UnitList;
        public ObservableCollection<Unit_Table> UnitList
        {
            get { return _UnitList; }
            set { _UnitList = value; RaisePropertyChanged(() => UnitList); }
        }

        /// <summary>
        /// 选中的单位
        /// </summary>
        private Unit_Table _SelectedUnit;
        [Required(ErrorMessage = "单位不能为空")]
        public Unit_Table SelectedUnit
        {
            get { return _SelectedUnit; }
            set { _SelectedUnit = value; RaisePropertyChanged(() => SelectedUnit); }
        }


        /// <summary>
        /// 分类列表
        /// </summary>
        private ObservableCollection<Classification_Table> _ClassificationList;
        public ObservableCollection<Classification_Table> ClassificationList
        {
            get { return _ClassificationList; }
            set { _ClassificationList = value; RaisePropertyChanged(() => ClassificationList); }
        }

        /// <summary>
        /// 选中的分类
        /// </summary>
        private Classification_Table _SelectedClassification;
        [Required(ErrorMessage = "分类不能为空")]
        public Classification_Table SelectedClassification
        {
            get { return _SelectedClassification; }
            set { _SelectedClassification = value; RaisePropertyChanged(() => SelectedClassification); }
        }
    }
}
