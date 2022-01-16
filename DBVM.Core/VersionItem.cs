namespace DBVM
{
    /// <summary>
    /// 数据库版本
    /// </summary>
    public class VersionItem
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 版本创建时间
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Desc { get; set; }

    }
}