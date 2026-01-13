using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Signet.Common;
using Signet.Model;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

namespace Signet.ViewModel
{
    public class RecordInfo_Model
    {
        public long SerialNum { get; set; }
        public string Goods_Code { get; set; }
        public string Goods_Name { get; set; }
        public string Goods_Model { get; set; }
        public string GoodsType { get; set; }
        public string Classification { get; set; }
        public string InventoryType { get; set; }
        public int InventoryNum { get; set; }
        public DateTime UserTime { get; set; }
        public string Operator { get; set; }
        public DateTime OperateTime { get; set; }
        public string Remarks { get; set; }

    }

    public class InventoryRecord_ViewModel:ViewModelBase
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

        #region 分页
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        private int _pageSize = 50;
        public int PageSize
        {
            get => _pageSize;
            set => Set(ref _pageSize, value);
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => Set(ref _totalItems, value);
        }

        #endregion

        private InventoryRecord_Model _mInventoryRecord_Model;
        public InventoryRecord_Model mInventoryRecord_Model
        {
            get { return _mInventoryRecord_Model; }
            set { _mInventoryRecord_Model = value; RaisePropertyChanged(() => mInventoryRecord_Model); }
        }

        public InventoryRecord_ViewModel()
        {
            mInventoryRecord_Model = new InventoryRecord_Model()
            {
                GoodsTypeList = new System.Collections.ObjectModel.ObservableCollection<GoodsType_Table>
            (SqlSugarHelper.mDB.Queryable<GoodsType_Table>().ToList()),
                SelectedGoodsTypeList = new System.Collections.ObjectModel.ObservableCollection<GoodsType_Table>(),
                GoodsUnitList = new System.Collections.ObjectModel.ObservableCollection<Unit_Table>
            (SqlSugarHelper.mDB.Queryable<Unit_Table>().ToList()),
                ClassificationList = new System.Collections.ObjectModel.ObservableCollection<Classification_Table>
            (SqlSugarHelper.mDB.Queryable<Classification_Table>().ToList()),
                SelectedClassificationList = new System.Collections.ObjectModel.ObservableCollection<Classification_Table>(),
            };


            // 监听分页变化
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CurrentPage) ||
                    e.PropertyName == nameof(PageSize))
                {
                    DataQuery(CurrentPage, PageSize);

                    //Console.WriteLine($"Loading page {CurrentPage} with size {PageSize}");
                }
            };
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
            if (CurrentPage == 1)
            {
                DataQuery(CurrentPage, PageSize);
            }
            else
            {
                CurrentPage = 1;
            }
        }

        private void DataQuery(int curPage, int pageSize)
        {
            try
            {
                int totalCount = 0;
                int totalPage = 0;

                var recordList = 
                    SqlSugarHelper.mDB.Queryable<InventoryRecord_Table>()
                    .LeftJoin<Goods_Table>((rt,gt)=>rt.GoodsID == gt.Goods_ID)
                    .LeftJoin<GoodsType_Table>((rt,gt,gtt)=>gt.Goods_Type == gtt.GoodsType_ID)
                    .LeftJoin<Classification_Table>((rt, gt, gtt,ct) => gt.Classification == ct.Classification_ID)
                    .LeftJoin<User_Table>((rt, gt, gtt, ct,ut) => rt.Operator == ut.UserID)
                    .WhereIF(!String.IsNullOrWhiteSpace(mInventoryRecord_Model.GoodsNameQuery),
                    (rt, gt, gtt, ct, ut) => gt.Goods_Name.Contains(mInventoryRecord_Model.GoodsNameQuery))
                    .WhereIF(!String.IsNullOrWhiteSpace(mInventoryRecord_Model.GoodsModleQuery),
                    (rt, gt, gtt, ct, ut) => gt.Goods_Model.Contains(mInventoryRecord_Model.GoodsModleQuery))
                    .WhereIF(mInventoryRecord_Model.SelectedGoodsTypeList.Count() > 0,
                    (rt, gt, gtt, ct, ut) => mInventoryRecord_Model.SelectedGoodsTypeList.Select(a => a.GoodsType_ID).ToArray().Contains(gt.Goods_Type))
                    .WhereIF(mInventoryRecord_Model.SelectedClassificationList.Count() > 0,
                    (rt, gt, gtt, ct, ut) => mInventoryRecord_Model.SelectedClassificationList.Select(a => a.Classification_ID).ToArray().Contains(gt.Classification))
                    .OrderBy((rt, gt, gtt, ct, ut) => rt.UserTime, OrderByType.Desc)
                    .Select((rt, gt, gtt, ct, ut) => new RecordInfo_Model { 
                    Goods_Code = gt.Goods_Code,
                    Goods_Name = gt.Goods_Name,
                    Goods_Model = gt.Goods_Model,
                    GoodsType = gtt.GoodsType_Name,
                    Classification = ct.Classification_Name,
                    InventoryType = rt.InventoryType,
                    InventoryNum = rt.InventoryNum,
                    UserTime = rt.UserTime,
                    Operator = ut.UserName,
                    OperateTime = rt.OperateTime,
                    Remarks =  rt.Remarks,
                    }).ToPageList(curPage, pageSize, ref totalCount, ref totalPage);

                TotalItems = totalCount;

                // 计算起始行号
                int startRowNum = (curPage - 1) * pageSize + 1;
                // 添加行号
                int currentRowNum = startRowNum;
                foreach (var record in recordList)
                {
                    record.SerialNum = currentRowNum;
                    currentRowNum++;
                }
                mInventoryRecord_Model.RecordList = new System.Collections.ObjectModel.ObservableCollection<RecordInfo_Model>(recordList);

                logger.Info("记录查询成功！");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
        #endregion

    }
}
