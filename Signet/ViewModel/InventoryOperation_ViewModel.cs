using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Signet.Common;
using Signet.Model;
using Signet.SqlSugarModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;
using static IronPython.Modules.PythonIterTools;
using static Signet.ViewModel.InventoryOperation_ViewModel;
namespace Signet.ViewModel
{
    public class InventoryOperation_ViewModel: ViewModelBase
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

        private InventoryOperation_Model _mInventoryOperation_Model;
        public InventoryOperation_Model mInventoryOperation_Model
        {
            get { return _mInventoryOperation_Model; }
            set { _mInventoryOperation_Model = value; RaisePropertyChanged(() => mInventoryOperation_Model); }
        }

        public enum InventoryType
        {
            StockIn,
            StockOut,
        }

        InventoryType mInventoryType;
        long mGoodsID;
        public InventoryOperation_ViewModel(InventoryType inventoryType,long goodsID)
        {
            mInventoryType = inventoryType; mGoodsID = goodsID;
            mInventoryOperation_Model = new InventoryOperation_Model() { 
                Num = 0,
                UserTime = DateTime.Now,
            };

            if (mInventoryType == InventoryType.StockIn)
            {
                mInventoryOperation_Model.Header = "入库";
            }
            else if (mInventoryType == InventoryType.StockOut)
            {
                mInventoryOperation_Model.Header = "出库";
            }
        }

        #region 确认
        private RelayCommand _Confirm_Command;
        public RelayCommand Confirm_Command
        {
            get
            {
                if (_Confirm_Command == null)
                {
                    _Confirm_Command = new RelayCommand(_Confirm);
                }
                return _Confirm_Command;
            }
            set { _Confirm_Command = value; }
        }
        /// <summary>
        /// 确认
        /// </summary>
        private void _Confirm()
        {
            if (!mInventoryOperation_Model.IsValidated)
            {
                ShowMessage("提示!", mInventoryOperation_Model.dataErrors.First().Value);
                logger.Info(mInventoryOperation_Model.dataErrors.First().Value);
                return;
            }

            if (mInventoryType == InventoryType.StockIn)
            {
                int result = SqlSugarHelper.mDB.Updateable<Goods_Table>()
                    .SetColumns(it => it.Inventory == it.Inventory + mInventoryOperation_Model.Num)
                    .Where(it => it.Goods_ID == mGoodsID)
                    .ExecuteCommand();
                if (result > 0)
                {
                    InventoryRecord_Table inventoryRecord_Table = new InventoryRecord_Table() { 
                        GoodsID = mGoodsID,
                        Operator = UserInfo.LogedUserInfo.UserID,
                        Remarks = mInventoryOperation_Model.Remark,
                        UserTime = mInventoryOperation_Model.UserTime,
                        InventoryType = "入库",
                        InventoryNum = mInventoryOperation_Model.Num
                    };
                    SqlSugarHelper.mDB.Insertable<InventoryRecord_Table>(inventoryRecord_Table).ExecuteCommand();
                }
                else {
                    ShowMessage("提示!", "入库失败！");
                    return;
                }

            }
            else if (mInventoryType == InventoryType.StockOut) {
                int result = SqlSugarHelper.mDB.Updateable<Goods_Table>()
                .SetColumns(it => it.Inventory == it.Inventory - mInventoryOperation_Model.Num)
                .Where(it => it.Goods_ID == mGoodsID && it.Inventory >= mInventoryOperation_Model.Num)
                .ExecuteCommand();

                if (result > 0)
                {
                    InventoryRecord_Table inventoryRecord_Table = new InventoryRecord_Table()
                    {
                        GoodsID = mGoodsID,
                        Operator = UserInfo.LogedUserInfo.UserID,
                        Remarks = mInventoryOperation_Model.Remark,
                        UserTime = mInventoryOperation_Model.UserTime,
                        InventoryType = "出库",
                        InventoryNum = mInventoryOperation_Model.Num
                    };
                    SqlSugarHelper.mDB.Insertable<InventoryRecord_Table>(inventoryRecord_Table).ExecuteCommand();

                }
                else {
                    ShowMessage("提示!", "库存不足！");
                    return;

                }
            }
            ToClose = true;
        }
        #endregion

        #region 取消
        private RelayCommand _Cancel_Command;
        public RelayCommand Cancel_Command
        {
            get
            {
                if (_Cancel_Command == null)
                {
                    _Cancel_Command = new RelayCommand(_Cancel);
                }
                return _Cancel_Command;
            }
            set { _Cancel_Command = value; }
        }
        /// <summary>
        /// 取消
        /// </summary>
        private void _Cancel()
        {
            ToClose = true;
        }

        #endregion

        #region 关闭窗口函数
        private bool toClose = false;
        /// <summary>
        /// 是否要关闭窗口
        /// </summary>
        public bool ToClose
        {
            get
            {
                return toClose;
            }
            set
            {
                toClose = value;
                if (toClose)
                {
                    this.RaisePropertyChanged("ToClose");
                }
            }
        }
        #endregion

    }
}
