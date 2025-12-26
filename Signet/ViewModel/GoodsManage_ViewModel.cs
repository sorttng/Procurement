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
    public class GoodsManage_ViewModel : ViewModelBase
    {
        private GoodsManage_Model _mGoodsManage_Model;

        public GoodsManage_Model mGoodsManage_Model
        {
            get { return _mGoodsManage_Model; }
            set { _mGoodsManage_Model = value; RaisePropertyChanged(() => mGoodsManage_Model); }
        }

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

        private string OptionType;
        private long GoodsID;

        public GoodsManage_ViewModel(string optiontype, long goodsID)
        {
            _dialogCoordinator = DialogCoordinator.Instance;

            OptionType = optiontype;
            GoodsID = goodsID;
            mGoodsManage_Model= new GoodsManage_Model()
            {
                GoodsTypeList = new System.Collections.ObjectModel.ObservableCollection<GoodsType_Table>
                (SqlSugarHelper.mDB.Queryable<GoodsType_Table>().ToList()),
                ClassificationList = new System.Collections.ObjectModel.ObservableCollection<Classification_Table>
                (SqlSugarHelper.mDB.Queryable<Classification_Table>().ToList()),
                UnitList = new System.Collections.ObjectModel.ObservableCollection<Unit_Table> 
                (SqlSugarHelper.mDB.Queryable<Unit_Table>().ToList()),
            };

            if (optiontype == "Add")
            {
                mGoodsManage_Model.Header = "用户信息新增";

                mGoodsManage_Model.GoodsCode = string.Empty;
                mGoodsManage_Model.GoodsName = string.Empty;
                mGoodsManage_Model.GoodsModel = string.Empty;
            }
            else if (optiontype == "Edit")
            {
                mGoodsManage_Model.Header = "用户信息编辑";

                var goodsInfo =
                SqlSugarHelper.mDB.Queryable<Goods_Table, GoodsType_Table, Unit_Table, Classification_Table>((gt, gtt, ut, ct) =>
                new JoinQueryInfos(
                    JoinType.Inner, gt.Goods_Type == gtt.GoodsType_ID,
                    JoinType.Inner, gt.Goods_Unit == ut.Unit_ID,
                    JoinType.Inner, gt.Classification == ct.Classification_ID
                    ))
                .Where(gt=>gt.Goods_ID == GoodsID)
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
                }).ToList().FirstOrDefault();

                mGoodsManage_Model.GoodsCode = goodsInfo.Goods_Code;
                mGoodsManage_Model.GoodsName = goodsInfo.Goods_Name;
                mGoodsManage_Model.GoodsModel = goodsInfo.Goods_Model;
                mGoodsManage_Model.SelectedGoodsType = mGoodsManage_Model.GoodsTypeList.Where(a => a.GoodsType_ID == goodsInfo.GoodsType_ID).First();
                mGoodsManage_Model.SelectedClassification = mGoodsManage_Model.ClassificationList.Where(a => a.Classification_ID == goodsInfo.Classification_ID).First();
                mGoodsManage_Model.SelectedUnit = mGoodsManage_Model.UnitList.Where(a=>a.Unit_ID==goodsInfo.Unit_ID).First();
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
            if (!mGoodsManage_Model.IsValidated)
            {
                ShowMessage("提示!", mGoodsManage_Model.dataErrors.First().Value);
                logger.Info(mGoodsManage_Model.dataErrors.First().Value);
                return;
            }

            if (OptionType == "Add")
            {
                SqlSugarModel.Goods_Table goods = new SqlSugarModel.Goods_Table()
                {
                    Goods_Code = mGoodsManage_Model.GoodsCode,
                    Goods_Name = mGoodsManage_Model.GoodsName,
                    Goods_Model = mGoodsManage_Model.GoodsModel,
                    Goods_Type = mGoodsManage_Model.SelectedGoodsType.GoodsType_ID,
                    Classification = mGoodsManage_Model.SelectedClassification.Classification_ID,
                    Goods_Unit = mGoodsManage_Model.SelectedUnit.Unit_ID,
                };

                #region 使用事务获取物品编码
                string goodscode = string.Empty;
                using (var db = SqlSugarHelper.mDB)
                {
                    SqlSugarHelper.mDB.BeginTran();

                    int goodsid = SqlSugarHelper.mDB.Insertable(goods).ExecuteReturnIdentity();
                    goodscode = $"G{goodsid:D4}";
                    db.Updateable<Goods_Table>()
                        .SetColumns(gt => gt.Goods_Code, goodscode)
                        .Where(gt => gt.Goods_ID == goodsid)
                        .ExecuteCommand();

                    db.CommitTran();
                }
                _dialogCoordinator.ShowModalInputExternal(this, "物品新增成功！", "新物品的唯一编码为", new MetroDialogSettings
                {
                    DefaultText = goodscode,
                });
                #endregion
            }
            else if (OptionType == "Edit")
            {
                SqlSugarModel.Goods_Table goods = new SqlSugarModel.Goods_Table()
                {
                    Goods_ID = GoodsID ,
                    Goods_Code = mGoodsManage_Model.GoodsCode,
                    Goods_Name = mGoodsManage_Model.GoodsName,
                    Goods_Model = mGoodsManage_Model.GoodsModel,
                    Goods_Type = mGoodsManage_Model.SelectedGoodsType.GoodsType_ID,
                    Classification = mGoodsManage_Model.SelectedClassification.Classification_ID,
                    Goods_Unit = mGoodsManage_Model.SelectedUnit.Unit_ID,
                };

                SqlSugarHelper.mDB.Updateable(goods)
                    .IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
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
