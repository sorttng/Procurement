using GalaSoft.MvvmLight;
using Signet.SqlSugarModel;
using Signet.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.Model
{
    public class InventoryRecord_Model: ObservableObject
    {
        /// <summary>
        /// 记录
        /// </summary>
        private ObservableCollection<RecordInfo_Model> _RecordList;
        public ObservableCollection<RecordInfo_Model> RecordList
        {
            get { return _RecordList; }
            set { _RecordList = value; RaisePropertyChanged(() => RecordList); }
        }

        /// <summary>
        /// 物品名称模糊查询
        /// </summary>
        private string _GoodsNameQuery;
        public string GoodsNameQuery
        {
            get { return _GoodsNameQuery; }
            set { _GoodsNameQuery = value; RaisePropertyChanged(() => GoodsNameQuery); }
        }

        /// <summary>
        /// 物品型号模糊查询
        /// </summary>
        private string _GoodsModleQuery;
        public string GoodsModleQuery
        {
            get { return _GoodsModleQuery; }
            set { _GoodsModleQuery = value; RaisePropertyChanged(() => GoodsModleQuery); }
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
        /// 选中的物资类型列表
        /// </summary>
        private ObservableCollection<GoodsType_Table> _SelectedGoodsTypeList;
        public ObservableCollection<GoodsType_Table> SelectedGoodsTypeList
        {
            get { return _SelectedGoodsTypeList; }
            set { _SelectedGoodsTypeList = value; RaisePropertyChanged(() => SelectedGoodsTypeList); }
        }


        /// <summary>
        /// 单位列表
        /// </summary>
        private ObservableCollection<Unit_Table> _GoodsUnitList;
        public ObservableCollection<Unit_Table> GoodsUnitList
        {
            get { return _GoodsUnitList; }
            set { _GoodsUnitList = value; RaisePropertyChanged(() => GoodsUnitList); }
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
        /// 选中的分类列表
        /// </summary>
        private ObservableCollection<Classification_Table> _SelectedClassificationList;
        public ObservableCollection<Classification_Table> SelectedClassificationList
        {
            get { return _SelectedClassificationList; }
            set { _SelectedClassificationList = value; RaisePropertyChanged(() => SelectedClassificationList); }
        }


    }
}
