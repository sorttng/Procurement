using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signet.Model
{
    public class MonthStatistics_Model
    {
        public long SerialNum { get; set; }
        public long Goods_ID { get; set; }
        public string Goods_Code { get; set; }
        public string Goods_Name { get; set; }
        public string Goods_Model { get; set; }
        public string GoodsType { get; set; }
        public string Classification { get; set; }
        public string Unit { get; set; }
        public double InCount { get; set; }
        public double OutCount { get; set; }
    }

    public class Statistics_Model: ObservableObject
    {
        /// <summary>
        /// 入库列表
        /// </summary>
        private double[] _InCountList;
        public double[] InCountList
        {
            get { return _InCountList; }
            set { _InCountList = value; RaisePropertyChanged(() => InCountList); }
        }

        /// <summary>
        /// 出库列表
        /// </summary>
        private double[] _OutCountList;
        public double[] OutCountList
        {
            get { return _OutCountList; }
            set { _OutCountList = value; RaisePropertyChanged(() => OutCountList); }
        }

        /// <summary>
        /// 标题列表
        /// </summary>
        private string[] _LablesList;
        public string[] LablesList
        {
            get { return _LablesList; }
            set { _LablesList = value; RaisePropertyChanged(() => LablesList); }
        }

        private ObservableCollection<MonthStatistics_Model> _MonthStatistics;
        public ObservableCollection<MonthStatistics_Model> MonthStatistics
        {
            get { return _MonthStatistics; }
            set { _MonthStatistics = value; RaisePropertyChanged(() => MonthStatistics); }

        }

        private ObservableCollection<string> _MonList;
        public ObservableCollection<string> MonList
        {
            get { return _MonList; }
            set { _MonList = value; RaisePropertyChanged(() => MonList); }
        }

        private string _Sel_MonList;
        public string Sel_MonList
        {
            get { return _Sel_MonList; }
            set { _Sel_MonList = value; RaisePropertyChanged(() => Sel_MonList); }
        }
    }
}
