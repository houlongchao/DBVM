using System;
using Npgsql;

namespace DBVM.Postgres
{
    /// <summary>
    /// Postgres 数据表管理实现
    /// </summary>
    public class PostgresVersionManager : BaseVersionManager
    {
        /// <summary>
        /// 默认xml
        /// </summary>
        public const string DefaultXml = "Postgres.xml";

        /// <summary>
        /// 构造 Postgres 数据表管理器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="xmlFolder">升级描述文件所在目录</param>
        /// <param name="versionXml">升级描述文件文件名</param>
        /// <exception cref="Exception"></exception>
        public PostgresVersionManager(string connectionString, string xmlFolder = DefaultFolder, string versionXml = DefaultXml) : base(xmlFolder, versionXml)
        {

            ConnectionString = connectionString;
            DbConnection = new NpgsqlConnection(connectionString);

            InitVersionTable();
        }

        /// <summary>
        /// 初始化版本升级数据库
        /// </summary>
        private void InitVersionTable()
        {
            EnsureConnectionOpened();

            string createTableSql = $@"
CREATE TABLE IF NOT EXISTS ""{VersionTable}"" (
    ""version"" int4 NOT NULL,
	""sql"" text NOT NULL,
	""author"" VARCHAR(50) NOT NULL,
    ""date"" VARCHAR(50) NOT NULL,
    ""desc"" TEXT,
	""createdate"" date NOT NULL,
    PRIMARY KEY (""version"")
);

COMMENT ON TABLE ""{VersionTable}"" IS 'DB版本控制表';

COMMENT ON COLUMN ""{VersionTable}"".""version"" IS '版本号';
COMMENT ON COLUMN ""{VersionTable}"".""sql"" IS 'sql语句';
COMMENT ON COLUMN ""{VersionTable}"".""author"" IS '作者';
COMMENT ON COLUMN ""{VersionTable}"".""date"" IS '版本创建日期';
COMMENT ON COLUMN ""{VersionTable}"".""desc"" IS '说明';
COMMENT ON COLUMN ""{VersionTable}"".""createdate"" IS '执行日期';
";
            using (var cmd = new NpgsqlCommand(createTableSql, (NpgsqlConnection)DbConnection))
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

            using (var cmd = new NpgsqlCommand(maxVersionNumSql, (NpgsqlConnection)DbConnection))
            {
                var maxVersion = cmd.ExecuteScalar()?.ToString();
                return string.IsNullOrEmpty(maxVersion) ? 0 : Convert.ToInt32(maxVersion);
            }
        }

        /// <inheritdoc />
        public override bool UpdateVersion(VersionItem item)
        {
            var conn = (NpgsqlConnection)DbConnection;

            var transaction = conn.BeginTransaction();
            try
            {
                var sql = TrimSql(item.Sql);
                using (var cmd = new NpgsqlCommand(sql, conn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand($@"insert into {VersionTable} (""version"", ""sql"", ""author"", ""date"", ""desc"", ""createdate"") values (@version, @sql, @author, @date, @desc, @createdate);", conn, transaction))
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