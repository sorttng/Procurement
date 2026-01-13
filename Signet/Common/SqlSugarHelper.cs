using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Windows.Forms;
namespace Signet.Common
{
    public class SqlSugarHelper
    {
        //ORM建库功能说明：建议不要加目录ORM没办法创建文件夹，如果加目录需要手动建文件夹
        //sqllite
        //static string path = Application.StartupPath + "\\mDatabase.db";
        //public static string ConnectionString = $"Data Source={path};Version=3;";
        //public static string ConnectionString = @"DataSource=mDatabase.db;Version=3;Journal Mode=WAL;Pooling=True;";

        public static string ServerIP = "localhost";
        public static string DB_UerName = "yst";
        public static string DB_Pwd = "123456";
        public static string Db_Name = "Procurement_DB";

        ///sqlserver        
        public static string ConnectionString = string.Format
            ("server={0};uid={1};pwd={2};database={3}"
            , ServerIP, DB_UerName, DB_Pwd, Db_Name);
        public static SqlSugarScope mDB =
        new SqlSugarScope(
        new ConnectionConfig()
        {
            ConnectionString = ConnectionString,
            DbType = DbType.SqlServer,//设置数据库类型
            IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
            InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
        },
        db => {
            //(A)全局生效配置点，一般AOP和程序启动的配置扔这里面 ，所有上下文生效
            //调试SQL事件，可以删掉
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响

                //获取原生SQL推荐 5.1.4.63  性能OK
                //UtilMethods.GetNativeSql(sql,pars)

                //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
            };

            //多个配置就写下面
            //db.Ado.IsDisableMasterSlaveSeparation=true;

            //注意多租户 有几个设置几个
            //db.GetConnection(i).Aop
        });

    }
}
