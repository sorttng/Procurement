using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using OfficeOpenXml;
using Signet.Common;
using Signet.Model;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Signet.ViewModel
{
    public class GoodsInfo_Model
    {
        public long SerialNum { get; set; }
        public long Goods_ID { get; set; }
        public string Goods_Code { get; set; }
        public string Goods_Name { get; set; }
        public string Goods_Model { get; set; }
        public string GoodsType { get; set; }
        public long GoodsType_ID { get; set; }

        public string Classification { get; set; }

        public long Classification_ID { get; set; }

        public string Unit { get; set; }
        public long Unit_ID { get; set; }
        public int Inventory { get; set; }
    }


    public class GoodsList_ViewModel: ViewModelBase
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

        private GoodsList_Model _mGoodsList_Model;
        public GoodsList_Model mGoodsList_Model
        {
            get { return _mGoodsList_Model; }
            set { _mGoodsList_Model = value; RaisePropertyChanged(() => mGoodsList_Model); }
        }

        public GoodsList_ViewModel()
        {
            _dialogCoordinator = DialogCoordinator.Instance;

            mGoodsList_Model = new GoodsList_Model() {
            GoodsTypeList = new System.Collections.ObjectModel.ObservableCollection<GoodsType_Table>
            (SqlSugarHelper.mDB.Queryable<GoodsType_Table>().ToList()),
            SelectedGoodsTypeList = new System.Collections.ObjectModel.ObservableCollection<GoodsType_Table> (),
            GoodsUnitList = new System.Collections.ObjectModel.ObservableCollection<Unit_Table>
            (SqlSugarHelper.mDB.Queryable<Unit_Table>().ToList()),
            ClassificationList = new System.Collections.ObjectModel.ObservableCollection<Classification_Table> 
            (SqlSugarHelper.mDB.Queryable<Classification_Table>().ToList()),
            SelectedClassificationList = new System.Collections.ObjectModel.ObservableCollection<Classification_Table>(),
            };
            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization"); //This will also set the Company property to the organization name provided in the argument.

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
                GlobalInfo.InventoryThreshold = GlobalInfo.configService.GetConfigValue("InventoryThreshold", 10);

                int totalCount = 0;
                int totalPage = 0;

                var goodsList =
                SqlSugarHelper.mDB.Queryable<Goods_Table, GoodsType_Table, Unit_Table,Classification_Table>((gt,gtt,ut,ct) =>
                new JoinQueryInfos(
                    JoinType.Inner, gt.Goods_Type == gtt.GoodsType_ID,
                    JoinType.Inner, gt.Goods_Unit == ut.Unit_ID,
                    JoinType.Inner,gt.Classification == ct.Classification_ID))
                .WhereIF(!String.IsNullOrWhiteSpace(mGoodsList_Model.GoodsNameQuery),
                gt => gt.Goods_Name.Contains(mGoodsList_Model.GoodsNameQuery))
                .WhereIF(!String.IsNullOrWhiteSpace(mGoodsList_Model.GoodsModleQuery),
                gt => gt.Goods_Model.Contains(mGoodsList_Model.GoodsModleQuery))
                .WhereIF(mGoodsList_Model.SelectedGoodsTypeList.Count()>0,
                gt=> mGoodsList_Model.SelectedGoodsTypeList.Select(a => a.GoodsType_ID).ToArray().Contains(gt.Goods_Type))
                .WhereIF(mGoodsList_Model.SelectedClassificationList.Count() > 0,
                gt => mGoodsList_Model.SelectedClassificationList.Select(a => a.Classification_ID).ToArray().Contains(gt.Classification))
                .OrderBy(gt=>gt.Goods_ID,OrderByType.Asc)
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
                mGoodsList_Model.GoodsList = new System.Collections.ObjectModel.ObservableCollection<GoodsInfo_Model>(goodsList);

                logger.Info("物品查询成功！");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region 删除
        private RelayCommand<long> _Delete_Command;
        public RelayCommand<long> Delete_Command
        {
            get
            {
                if (_Delete_Command == null)
                {
                    _Delete_Command = new RelayCommand<long>(_Delete);
                }
                return _Delete_Command;
            }
            set { _Delete_Command = value; }
        }
        private void _Delete(long goodsid)
        {
            var settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确认",
                NegativeButtonText = "取消",
            };

            MessageDialogResult result = _dialogCoordinator.ShowModalMessageExternal(this, "是否确认删除该物品？", "",
                                        MessageDialogStyle.AffirmativeAndNegative, settings);
            if (result == MessageDialogResult.Negative)
            {
                return;
            }

            SqlSugarHelper.mDB.Deleteable<SqlSugarModel.Goods_Table>()
                .Where(a => a.Goods_ID == goodsid).ExecuteCommand();

            logger.Info($"删除物品：id={goodsid}");
            DataQuery(1, 50);
        }
        #endregion

        #region 新增
        private RelayCommand _Insert_Command;
        public RelayCommand Insert_Command
        {
            get
            {
                if (_Insert_Command == null)
                {
                    _Insert_Command = new RelayCommand(_Insert);
                }
                return _Insert_Command;
            }
            set { _Insert_Command = value; }
        }
        private void _Insert()
        {
            WindowManager.ShowDialog("GoodsManage", new GoodsManage_ViewModel("Add", -1));
            DataQuery(1, 50);
        }
        #endregion

        #region 编辑
        private RelayCommand<long> _Updata_Command;
        public RelayCommand<long> Updata_Command
        {
            get
            {
                if (_Updata_Command == null)
                {
                    _Updata_Command = new RelayCommand<long>(_Updata);
                }
                return _Updata_Command;
            }
            set { _Updata_Command = value; }
        }
        /// <summary>
        /// 编辑
        /// </summary>
        private void _Updata(long goodsid)
        {
            WindowManager.ShowDialog("GoodsManage", new GoodsManage_ViewModel("Edit", goodsid));
            DataQuery(1, 50);
        }
        #endregion

        #region 批量导入
        private RelayCommand _BatchImport_Command;
        public RelayCommand BatchImport_Command
        {
            get
            {
                if (_BatchImport_Command == null)
                {
                    _BatchImport_Command = new RelayCommand(_BatchImport);
                }
                return _BatchImport_Command;
            }
            set { _BatchImport_Command = value; }
        }
        private void _BatchImport()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 基本配置
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "xlsx (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false; // 是否允许多选
            openFileDialog.RestoreDirectory = true;
            // 显示对话框并处理结果
            if (openFileDialog.ShowDialog()== DialogResult.OK)
            {
                string selectedFileName = openFileDialog.FileName;
                // 可以在这里添加文件处理逻辑
                ExcelImportToDB(selectedFileName);
                ShowMessage("提示!", "批量导入成功！");

                DataQuery(1, 50);
            }
        }


        /// <summary>
        /// 读取Excel文件到DataTable（第一行为列名）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetIndex">工作表索引（从0开始）</param>
        /// <param name="hasHeader">第一行是否为列名</param>
        /// <returns>DataTable</returns>
        public static DataTable ReadToDataTable(string filePath, int sheetIndex = 0, bool hasHeader = true)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[sheetIndex];
                var dt = new DataTable();

                // 获取数据范围
                int startRow = hasHeader ? 2 : 1; // 如果有表头，数据从第2行开始
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // 添加列
                for (int col = 1; col <= colCount; col++)
                {
                    string columnName = hasHeader
                        ? worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}"
                        : $"Column{col}";
                    dt.Columns.Add(columnName);
                }

                // 添加数据行
                for (int row = startRow; row <= rowCount; row++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasData = false;

                    for (int col = 1; col <= colCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value;
                        dr[col - 1] = cellValue?.ToString() ?? "";
                        if (cellValue != null) hasData = true;
                    }

                    if (hasData) // 避免添加空行
                        dt.Rows.Add(dr);
                }

                return dt;
            }
        }


        private void ExcelImportToDB(string path)
        {
            DataTable dataTable = ReadToDataTable(path);
            using (var db = SqlSugarHelper.mDB)
            {
                SqlSugarHelper.mDB.BeginTran();
                try
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        SqlSugarModel.Goods_Table goods = new SqlSugarModel.Goods_Table()
                        {
                            Goods_Code = string.Empty,
                            Goods_Name = dr["物品名称"].ToString(),
                            Goods_Model = dr["物品型号"].ToString(),
                            Goods_Type = mGoodsList_Model.GoodsTypeList.Where(a => a.GoodsType_Name == dr["物资类型"].ToString()).FirstOrDefault().GoodsType_ID,
                            Classification = mGoodsList_Model.ClassificationList.Where(a => a.Classification_Name == dr["分类"].ToString()).FirstOrDefault().Classification_ID,
                            Goods_Unit = mGoodsList_Model.GoodsUnitList.Where(a => a.Unit_Name == dr["单位"].ToString()).FirstOrDefault().Unit_ID,
                        };

                        #region 使用事务获取物品编码
                        string goodscode = string.Empty;

                        int goodsid = SqlSugarHelper.mDB.Insertable(goods).ExecuteReturnIdentity();
                        goodscode = $"G{goodsid:D4}";
                        db.Updateable<Goods_Table>()
                            .SetColumns(gt => gt.Goods_Code, goodscode)
                            .Where(gt => gt.Goods_ID == goodsid)
                            .ExecuteCommand();
                        #endregion
                    }
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    ShowMessage("提示!", "批量导入失败！");
                    logger.Error(ex);
                }
            }
        }
        #endregion
    }
}
