using System;
using MySql.Data.MySqlClient;

namespace DBVM.MySql
{
    /// <summary>
    /// MySql 数据表管理实现
    /// </summary>
    public class MySqlVersionManager : BaseVersionManager
    {
        /// <summary>
        /// 构造MySql数据表管理器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="xmlFolder">升级描述文件所在目录</param>
        /// <param name="versionXml">升级描述文件文件名</param>
        /// <exception cref="Exception"></exception>
        public MySqlVersionManager(string connectionString, string xmlFolder = "DBVM", string versionXml = "MySql.xml") : base(xmlFolder, versionXml)
        {
            ConnectionString = connectionString;
            DbConnection = new MySqlConnection(connectionString);

            InitVersionTable();
        }

        /// <summary>
        /// 初始化版本升级数据库
        /// </summary>
        private void InitVersionTable()
        {
            EnsureConnectionOpened();

            string createTableSql = $@"
CREATE TABLE IF NOT EXISTS `{VersionTable}`(
	`version` INT PRIMARY KEY COMMENT '版本号',
	`sql` TEXT NOT NULL COMMENT 'sql语句',
	`author` VARCHAR(50) NOT NULL COMMENT '修改人',
	`date` VARCHAR(50) NOT NULL COMMENT '版本创建日期',
    `desc` TEXT NULL COMMENT '说明',
	`createdate` DateTime NOT NULL COMMENT '执行日期'
)ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT = 'DB版本控制表';
";
            using (var cmd = new MySqlCommand(createTableSql, (MySqlConnection)DbConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        #region override

        /// <summary>
        /// 获得最大版本号
        /// </summary>
        /// <returns></returns>
        public override int GetMaxVersionNum()
        {
            EnsureConnectionOpened();

            string maxVersionNumSql = $@"select MAX(version) from {VersionTable};";

            using (var cmd = new MySqlCommand(maxVersionNumSql, (MySqlConnection)DbConnection))
            {
                var maxVersion = cmd.ExecuteScalar()?.ToString();
                return string.IsNullOrEmpty(maxVersion) ? 0 : Convert.ToInt32(maxVersion);
            }
        }

        /// <inheritdoc />
        public override bool UpdateVersion(VersionItem item)
        {
            var conn = (MySqlConnection)DbConnection;

            var transaction = conn.BeginTransaction();
            try
            {
                var sql = TrimSql(item.Sql);
                using (var cmd = new MySqlCommand(sql, conn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand($"insert into {VersionTable} (`version`, `sql`, `author`, `date`, `desc`, `createdate`) values (@version, @sql, @author, @date, @desc, @createdate);", conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@version", item.Version);
                    cmd.Parameters.AddWithValue("@sql", item.Sql);
                    cmd.Parameters.AddWithValue("@author", item.Author);
                    cmd.Parameters.AddWithValue("@date", item.Date);
                    cmd.Parameters.AddWithValue("@desc", item.Desc??"");
                    cmd.Parameters.AddWithValue("@createdate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private string TrimSql(string sql)
        {
            return sql;
        }

        #endregion

    }
}