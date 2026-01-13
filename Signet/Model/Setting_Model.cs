using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Data;
namespace Signet.Model
{
    public class Setting_Model : ObservableObject
    {
        private List<AppThemeMenuData> _AppThemes;
        /// <summary>
        /// 主题
        /// </summary>
        public List<AppThemeMenuData> AppThemes
        {
            get { return _AppThemes; }
            set { _AppThemes = value; RaisePropertyChanged(() => AppThemes); }
        }


        private List<AccentColorMenuData> _AccentColors;
        /// <summary>
        /// 颜色
        /// </summary>
        public List<AccentColorMenuData> AccentColors
        {
            get { return _AccentColors; }
            set { _AccentColors = value; RaisePropertyChanged(() => AccentColors); }
        }

        private AppThemeMenuData _Sel_Theme;
        /// <summary>
        /// 选中主题
        /// </summary>
        public AppThemeMenuData Sel_Theme
        {
            get { return _Sel_Theme; }
            set { _Sel_Theme = value; RaisePropertyChanged(() => Sel_Theme); }
        }

        private AccentColorMenuData _Sel_AccentColors;
        /// <summary>
        /// 选中颜色
        /// </summary>
        public AccentColorMenuData Sel_AccentColors
        {
            get { return _Sel_AccentColors; }
            set { _Sel_AccentColors = value; RaisePropertyChanged(() => Sel_AccentColors); }
        }

        private bool _IsOn;
        public bool IsOn
        {
            get { return _IsOn; }
            set { _IsOn = value; RaisePropertyChanged(() => IsOn); }
        }

        private ObservableCollection<SqlSugarModel.Location_Table> _Locations;
        /// <summary>
        /// 地区列表
        /// </summary>
        public ObservableCollection<SqlSugarModel.Location_Table> Locations
        {
            get { return _Locations; }
            set { _Locations = value; RaisePropertyChanged(() => Locations); }
        }

        private SqlSugarModel.Location_Table _Sel_Location;
        /// <summary>
        /// 选中地区
        /// </summary>
        public SqlSugarModel.Location_Table Sel_Location
        {
            get { return _Sel_Location; }
            set { _Sel_Location = value; RaisePropertyChanged(() => Sel_Location); }
        }


        private int _InventoryThreshold;
        /// <summary>
        /// 阈值
        /// </summary>
        public int InventoryThreshold
        {
            get { return _InventoryThreshold; }
            set { _InventoryThreshold = value; RaisePropertyChanged(() => InventoryThreshold); }
        }
    }

}
