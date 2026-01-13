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
namespace Signet.ViewModel
{
    public class Inventory_ViewModel : ViewModelBase
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


        private Inventory_Model _mInventory_Model;
        public Inventory_Model mInventory_Model
        {
            get { return _mInventory_Model; }
            set { _mInventory_Model = value; RaisePropertyChanged(() => mInventory_Model); }
        }
        public Inventory_ViewModel()
        {
            mInventory_Model = new Inventory_Model() {
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

                var goodsList =
                SqlSugarHelper.mDB.Queryable<Goods_Table, GoodsType_Table, Unit_Table, Classification_Table>((gt, gtt, ut, ct) =>
                new JoinQueryInfos(
                    JoinType.Inner, gt.Goods_Type == gtt.GoodsType_ID,
                    JoinType.Inner, gt.Goods_Unit == ut.Unit_ID,
                    JoinType.Inner, gt.Classification == ct.Classification_ID))
                .WhereIF(!String.IsNullOrWhiteSpace(mInventory_Model.GoodsNameQuery),
                gt => gt.Goods_Name.Contains(mInventory_Model.GoodsNameQuery))
                .WhereIF(!String.IsNullOrWhiteSpace(mInventory_Model.GoodsModleQuery),
                gt => gt.Goods_Model.Contains(mInventory_Model.GoodsModleQuery))
                .WhereIF(mInventory_Model.SelectedGoodsTypeList.Count() > 0,
                gt => mInventory_Model.SelectedGoodsTypeList.Select(a => a.GoodsType_ID).ToArray().Contains(gt.Goods_Type))
                .WhereIF(mInventory_Model.SelectedClassificationList.Count() > 0,
                gt => mInventory_Model.SelectedClassificationList.Select(a => a.Classification_ID).ToArray().Contains(gt.Classification))
                .OrderBy(gt => gt.Goods_ID, OrderByType.Asc)
                .Select((gt, gtt, ut, ct) => new GoodsInfo_Model
                {
                    Goods_ID = gt.Goods_ID,
                    Goods_Code = gt.Goods_Code,
                    Goods_Name = gt.Goods_Name,
                    Goods_Model = gt.Goods_Model,
                    GoodsType = gtt.GoodsType_Name,
                    GoodsType_ID = gtt.GoodsType_ID,
                    Classification = ct.Classification_Name,
                    Classification_ID = ct.Classification_ID,
                    Unit = ut.Unit_Name,
                    Unit_ID = ut.Unit_ID,
                    Inventory = gt.Inventory,
                }).ToPageList(curPage, pageSize, ref totalCount, ref totalPage);
                TotalItems = totalCount;

                // 计算起始行号
                int startRowNum = (curPage - 1) * pageSize + 1;
                // 添加行号
                int currentRowNum = startRowNum;
                foreach (var goods in goodsList)
                {
                    goods.SerialNum = currentRowNum;
                    currentRowNum++;
                }
                mInventory_Model.GoodsList = new System.Collections.ObjectModel.ObservableCollection<GoodsInfo_Model>(goodsList);

                logger.Info("物品查询成功！");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region 入库
        private RelayCommand<long> _StockIn_Command;
        public RelayCommand<long> StockIn_Command
        {
            get
            {
                if (_StockIn_Command == null)
                {
                    _StockIn_Command = new RelayCommand<long>(_StockIn);
                }
                return _StockIn_Command;
            }
            set { _StockIn_Command = value; }
        }
        private void _StockIn(long goodsid)
        { 
            WindowManager.ShowDialog("InventoryOpration", new InventoryOperation_ViewModel(InventoryOperation_ViewModel.InventoryType.StockIn, goodsid));
            DataQuery(1, 50);
        }
        #endregion

        #region 出库
        private RelayCommand<long> _StockOut_Command;
        public RelayCommand<long> StockOut_Command
        {
            get
            {
                if (_StockOut_Command == null)
                {
                    _StockOut_Command = new RelayCommand<long>(_StockOut);
                }
                return _StockOut_Command;
            }
            set { _StockOut_Command = value; }
        }
        private void _StockOut(long goodsid)
        {
            WindowManager.ShowDialog("InventoryOpration", new InventoryOperation_ViewModel(InventoryOperation_ViewModel.InventoryType.StockOut, goodsid));
            DataQuery(1, 50);
        }
        #endregion
    }
}
