using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Signet.Common;
using Signet.Model;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Signet.ViewModel
{

    public class Statistics_ViewModel: ViewModelBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Message
        private readonly IDialogCoordinator _dialogCoordinator;
        // Simple method which can be used on a Button
        public async void ShowMessage_async(string title, string msg)
        {
            await _dialogCoordinator.ShowMessageAsync(this, title, msg);
        }

        public void ShowMessage(string title, string msg)
        {
            _dialogCoordinator.ShowModalMessageExternal(this, title, msg);
        }

        #endregion

        private Statistics_Model _mStatistics_Model;
        public Statistics_Model mStatistics_Model
        {
            get { return _mStatistics_Model; }
            set { _mStatistics_Model = value; RaisePropertyChanged(() => mStatistics_Model); }
        }

        public Statistics_ViewModel()
        {
            mStatistics_Model = new Statistics_Model() {
                MonList = new System.Collections.ObjectModel.ObservableCollection<string>(
                SqlSugarHelper.mDB.Queryable<InventoryRecord_Table>()
                .GroupBy(it => it.UserTime.ToString("yyyy-MM"))
                .Select(it => new { yearmon = it.UserTime.ToString("yyyy-MM") }).MergeTable()
                .Select(a => a.yearmon).ToArray()),
            };
        }

        private void GetMonthData(int year, int mon)
        {
            mStatistics_Model.MonthStatistics = new System.Collections.ObjectModel.ObservableCollection<MonthStatistics_Model>(
                SqlSugarHelper.mDB.Queryable<InventoryRecord_Table>()
                .Where(rt => rt.UserTime.Year == year && rt.UserTime.Month == mon)
                .GroupBy(rt => rt.GoodsID)
                .Select(rt => new
                {
                    GoodsID = rt.GoodsID,
                    InCount = SqlFunc.AggregateSum(SqlFunc.IIF(rt.InventoryType == "入库", rt.InventoryNum, 0)),
                    OutCount = SqlFunc.AggregateSum(SqlFunc.IIF(rt.InventoryType == "出库", rt.InventoryNum, 0)),
                }).MergeTable()
                .LeftJoin<Goods_Table>((rt, gt) => rt.GoodsID == gt.Goods_ID)
                .LeftJoin<GoodsType_Table>((rt, gt, ggt) => gt.Goods_Type == ggt.GoodsType_ID)
                .LeftJoin<Classification_Table>((rt, gt, ggt, ct) => gt.Classification == ct.Classification_ID)
                .LeftJoin<Unit_Table>((rt, gt, ggt, ct, ut) => gt.Goods_Unit == ut.Unit_ID)
                .Select((rt, gt, ggt, ct, ut) => new MonthStatistics_Model
                {
                    SerialNum = SqlFunc.RowNumber(rt.GoodsID),
                    Goods_ID = gt.Goods_ID,
                    Goods_Code = gt.Goods_Code,
                    Goods_Name = gt.Goods_Name,
                    Goods_Model = gt.Goods_Model,
                    GoodsType = ggt.GoodsType_Name,
                    Classification = ct.Classification_Name,
                    Unit = ut.Unit_Name,
                    InCount = rt.InCount,
                    OutCount = rt.OutCount,
                }).ToList());

            //mStatistics_Model.InCountList = list.Select(a=>a.InCount).ToArray();
            //mStatistics_Model.OutCountList = list.Select(a => a.OutCount).ToArray();
            //mStatistics_Model.LablesList = list.Select(a => a.Goods_Name).ToArray();

        }

        #region 查询
        private RelayCommand _Search_Command;
        public RelayCommand Search_Command
        {
            get
            {
                if (_Search_Command == null)
                {
                    _Search_Command = new RelayCommand(_Search);
                }
                return _Search_Command;
            }
            set { _Search_Command = value; }
        }
        /// <summary>
        /// 查询
        /// </summary>
        private void _Search()
        {
            int year = Convert.ToInt32(mStatistics_Model.Sel_MonList.Split('-')[0]);
            int mon = Convert.ToInt32(mStatistics_Model.Sel_MonList.Split('-')[1]);
            GetMonthData(year, mon);
        }
        #endregion
    }
}
