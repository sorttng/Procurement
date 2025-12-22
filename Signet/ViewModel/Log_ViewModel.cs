using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Signet.Model;
using GalaSoft.MvvmLight.Command;
using Signet.Common;
using Signet.SqlSugarModel;
using MahApps.Metro.Controls.Dialogs;
using Signet.UI;
using System.Windows.Data;
using System.ComponentModel;
namespace Signet.ViewModel
{
    public class Log_ViewModel: ViewModelBase
    {
        private Log_Model _mLog_Model;

        public Log_Model mLog_Model
        {
            get { return _mLog_Model; }
            set { _mLog_Model = value; RaisePropertyChanged(() => mLog_Model); }
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
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

        public Log_ViewModel()
        {
            mLog_Model = new Log_Model()
            {
                LogStartTime = DateTime.Now.Date.AddDays(-1),
                LogEndTime = DateTime.Now.Date,
                UserList = new System.Collections.ObjectModel.ObservableCollection<User_Table>(SqlSugarHelper.mDB.Queryable<User_Table>().ToList()),
                SelectedUserList = new System.Collections.ObjectModel.ObservableCollection<User_Table>(),
                LogList = new System.Collections.ObjectModel.ObservableCollection<SqlSugarModel.Nlog_Model>(),
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
                mLog_Model.LogList = new System.Collections.ObjectModel.ObservableCollection<Nlog_Model>
                    (SqlSugarHelper.mDB.Queryable<Nlog_Model>()
                    .Where(a => a.Logged >= mLog_Model.LogStartTime 
                    && a.Logged <= mLog_Model.LogEndTime
                    && mLog_Model.SelectedUserList.Select(b=>b.UserID).ToList().Contains(a.UserID))
                    .ToPageList(curPage, pageSize, ref totalCount, ref totalPage));
                TotalItems = totalCount;
                logger.Info("日志查询成功！");
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
