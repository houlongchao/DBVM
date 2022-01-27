using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DBVM.SqlServer
{
    /// <summary>
    /// Sql Server 数据表管理实现
    /// </summary>
    public class SqlServerVersionManager : BaseVersionManager
    {
        /// <summary>
        /// 构造SqlServer数据表版本管理器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="xmlFolder">升级描述文件所在目录</param>
        /// <param name="versionXml">升级描述文件文件名</param>
        /// <exception cref="Exception"></exception>
        public SqlServerVersionManager(string connectionString, string xmlFolder = "DBVM", string versionXml = "SqlServer.xml") : base(xmlFolder, versionXml)
        {
            ConnectionString = connectionString;
            DbConnection = new SqlConnection(connectionString);

            InitVersionTable();
        }

        /// <summary>
        /// 初始化版本升级数据库
        /// </summary>
        private void InitVersionTable()
        {
            EnsureConnectionOpened();

            string createTableSql = $@"
IF NOT EXISTS (select * from dbo.sysobjects where xtype='U' and Name = '{VersionTable}')
BEGIN
CREATE TABLE [dbo].[{VersionTable}](
	[version] [int] PRIMARY KEY,
	[sql] [nvarchar](max) NOT NULL,
	[author] [nvarchar](50) NOT NULL,
	[date] [nvarchar](50) NOT NULL,
    [desc] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL
);
EXEC sp_addextendedproperty 'MS_Description', N'DB版本控制表', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}';
EXEC sp_addextendedproperty 'MS_Description', N'版本号', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}', 'COLUMN', N'version';
EXEC sp_addextendedproperty 'MS_Description', N'Sql语句', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}', 'COLUMN', N'sql';
EXEC sp_addextendedproperty 'MS_Description', N'作者', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}', 'COLUMN', N'author';
EXEC sp_addextendedproperty 'MS_Description', N'版本创建日期', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}', 'COLUMN', N'date';
EXEC sp_addextendedproperty 'MS_Description', N'说明', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}', 'COLUMN', N'desc';
EXEC sp_addextendedproperty 'MS_Description', N'执行日期', 'SCHEMA', N'dbo', 'TABLE', N'{VersionTable}', 'COLUMN', N'createdate';
END
";
            using (var cmd = new SqlCommand(createTableSql, (SqlConnection)DbConnection))
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

            string maxVersionNumSql = $@"select MAX([version]) from [dbo].[{VersionTable}];";

            using (var cmd = new SqlCommand(maxVersionNumSql, (SqlConnection)DbConnection))
            {
                var maxVersion = cmd.ExecuteScalar()?.ToString();
                return string.IsNullOrEmpty(maxVersion) ? 0 : Convert.ToInt32(maxVersion);
            }
        }

        /// <inheritdoc />
        public override bool UpdateVersion(VersionItem item)
        {
            var conn = (SqlConnection)DbConnection;

            var transaction = conn.BeginTransaction();
            try
            {
                var sql = TrimSql(item.Sql);
                using (var cmd = new SqlCommand(sql, conn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand($"insert into [{VersionTable}] ([version], [sql], [author], [date], [desc], [createdate]) values (@version, @sql, @author, @date, @desc, @createdate);", conn, transaction))
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
            return Regex.Replace(sql, "\r\n\\s*GO", "");
        }

        #endregion

    }
}