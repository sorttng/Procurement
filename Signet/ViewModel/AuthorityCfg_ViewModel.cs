using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Signet.Common;
using Signet.Model;
using Signet.SqlSugarModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Signet.ViewModel
{
    public class AuthorityCfg_ViewModel : ViewModelBase
    {
        private AuthorityCfg_Model _mAuthorityCfg_Model;

        public AuthorityCfg_Model mAuthorityCfg_Model
        {
            get { return _mAuthorityCfg_Model; }
            set { _mAuthorityCfg_Model = value; RaisePropertyChanged(() => mAuthorityCfg_Model); }
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


        public AuthorityCfg_ViewModel()
        {
            _dialogCoordinator = DialogCoordinator.Instance;

            mAuthorityCfg_Model = new AuthorityCfg_Model()
            {
                Roles = new System.Collections.ObjectModel.ObservableCollection<Role_Table>
                (SqlSugarHelper.mDB.Queryable<SqlSugarModel.Role_Table>().ToList()),
                Authorities = new System.Collections.ObjectModel.ObservableCollection<Authorities_Table>
                (SqlSugarHelper.mDB.Queryable<SqlSugarModel.Authorities_Table>().ToList()),
                AuthoritiesWithCheck = new System.Collections.ObjectModel.ObservableCollection<AuthoritiesWithCheck>(),                
            };

            mAuthorityCfg_Model.Selected_Role = mAuthorityCfg_Model.Roles.FirstOrDefault();

            LoadAuthority();
        }

        #region 选择角色
        private RelayCommand _AuthorityChange_Command;
        public RelayCommand AuthorityChange_Command
        {
            get
            {
                if (_AuthorityChange_Command == null)
                {
                    _AuthorityChange_Command = new RelayCommand(_AuthorityChange);
                }
                return _AuthorityChange_Command;
            }
            set { _AuthorityChange_Command = value; }
        }
        /// <summary>
        /// 选择角色
        /// </summary>
        private void _AuthorityChange()
        {
            LoadAuthority();
        }

        public static List<Authorities_Table> LoadRoleAuthoritys(long roleID)
        {
            //该角色的所有权限
            List<Authorities_Table> roleAuthority = SqlSugarHelper.mDB.Queryable<SqlSugarModel.Role_Table, SqlSugarModel.Role_Auth_Relationship_Table, SqlSugarModel.Authorities_Table>
                ((rt, rat, at) => new JoinQueryInfos(
                    JoinType.Inner, rt.Role_ID == rat.Role_ID,
                    JoinType.Inner, rat.Authority_ID == at.Authorities_ID))
                .Where(rt => rt.Role_ID == roleID)
                .Select((rt, rat, at) =>
                new Authorities_Table{
                    //Role_ID = rt.Role_ID,
                    //Role_Name = rt.Role_Name,
                    Authorities_ID = at.Authorities_ID,
                    Authority_Name = at.Authority_Name,
                    Description = at.Description,
                }).ToList();

            return roleAuthority;
        }

        private void LoadAuthority()
        {
            List<Authorities_Table> roleAuthority = LoadRoleAuthoritys(mAuthorityCfg_Model.Selected_Role.Role_ID);

            mAuthorityCfg_Model.AuthoritiesWithCheck =
            new System.Collections.ObjectModel.ObservableCollection<AuthoritiesWithCheck>(
                mAuthorityCfg_Model.Authorities.Select(x => new AuthoritiesWithCheck()
                {
                    Authorities_ID = x.Authorities_ID,
                    Authority_Name = x.Authority_Name,
                    IsCheck = roleAuthority.Count(a => a.Authorities_ID == x.Authorities_ID) > 0 ? true : false,
                    Description = x.Description,
                    IsEffect = x.IsEffect,
                }));

        }

        #endregion


        #region 提交更改
        private RelayCommand _CommitChange_Command;
        public RelayCommand CommitChange_Command
        {
            get
            {
                if (_CommitChange_Command == null)
                {
                    _CommitChange_Command = new RelayCommand(_CommitChange);
                }
                return _CommitChange_Command;
            }
            set { _CommitChange_Command = value; }
        }
        /// <summary>
        /// 更新权限
        /// </summary>
        private void _CommitChange()
        {
            using (var db = SqlSugarHelper.mDB)
            {
                db.BeginTran();
                try
                {
                    //删除所有权限
                    db.Deleteable<Role_Auth_Relationship_Table>().
                            Where(r => r.Role_ID == mAuthorityCfg_Model.Selected_Role.Role_ID)
                            .ExecuteCommand();


                    List<long> checkedAuthoritys = mAuthorityCfg_Model.AuthoritiesWithCheck.Where(a=>a.IsCheck == true).Select(a=>a.Authorities_ID).ToList();

                    if (checkedAuthoritys.Any())
                    {
                        List<SqlSugarModel.Role_Auth_Relationship_Table> newRecords = new List<Role_Auth_Relationship_Table>();
                        foreach (var AuthorityID in checkedAuthoritys) {
                            newRecords.Add(new Role_Auth_Relationship_Table()
                            {
                                Role_ID = mAuthorityCfg_Model.Selected_Role.Role_ID,
                                Authority_ID = AuthorityID,
                            });
                        }

                        db.Insertable(newRecords).ExecuteCommand();
                    }

                    db.CommitTran();
                    ShowMessage("提示！","权限更改成功！");
                }
                catch (Exception ex) { 
                    db.RollbackTran();
                    logger.Error(ex);
                }
            }
        }
        #endregion
    }
}
