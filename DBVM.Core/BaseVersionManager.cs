using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml;

namespace DBVM
{
    /// <summary>
    /// 数据库版本管理抽象类
    /// </summary>
    public abstract class BaseVersionManager : IVersionManager, IDisposable
    {
        /// <summary>
        /// 默认xml文件夹
        /// </summary>
        public const string DefaultFolder = "DBVM";

        /// <summary>
        /// 当前工作目录
        /// </summary>
	    protected readonly string CurrentFolder = Directory.GetCurrentDirectory();

        /// <summary>
        /// 数据库版本管理
        /// </summary>
        /// <param name="xmlFolder">升级描述文件所在目录</param>
        /// <param name="versionXml">升级描述文件文件名</param>
        protected BaseVersionManager(string xmlFolder = DefaultFolder, string versionXml = "__dbversion.xml")
        {
            VersionXmlPath = Path.Combine(CurrentFolder, versionXml);

            if (!File.Exists(VersionXmlPath))
            {
                VersionXmlPath = Path.Combine(CurrentFolder, xmlFolder, versionXml);
            }

            if (!File.Exists(VersionXmlPath))
            {
                VersionXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, versionXml);
            }

            if (!File.Exists(VersionXmlPath))
            {
                VersionXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFolder, versionXml);
            }
        }

        /// <summary>
        /// 数据库中存储版本升级信息的表
        /// </summary>
        protected virtual string VersionTable { get; } = "___dbversions";

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected DbConnection DbConnection { get; set; }

        /// <summary>
        /// 数据库版本升级描述文件路径
        /// </summary>
        public string VersionXmlPath { get; }

        /// <summary>
        /// 确保数据库连接打开
        /// </summary>
        protected void EnsureConnectionOpened()
        {
            if (DbConnection.State == ConnectionState.Broken || DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DbConnection?.Dispose();
        }


        #region IVersionManager

        /// <inheritdoc />
        public abstract int GetMaxVersionNum();

        /// <inheritdoc />
        public abstract bool UpdateVersion(VersionItem item);

        /// <inheritdoc />
        public virtual IList<VersionItem> GetNeedUpdateVersions()
        {
            var versions = new List<VersionItem>();
            var doc = new XmlDocument();
            doc.Load(VersionXmlPath);
            var dbversion = doc.DocumentElement;
            foreach (XmlNode node in dbversion.GetElementsByTagName("versionitem"))
            {
                var item = new VersionItem();
                item.Version = Convert.ToInt32(node.Attributes["version"].InnerText);
                item.Author = node.Attributes["author"].InnerText;
                item.Sql = node.InnerText.Trim();
                //如果引用的外部sql文件
                if (node.Attributes["type"].InnerText.Trim().ToLower().Equals("sqlfile"))
                {
                    var sqlFile = Path.Combine(Path.GetDirectoryName(VersionXmlPath), item.Sql);
                    if (File.Exists(sqlFile))
                    {
                        item.Sql = File.ReadAllText(sqlFile, Encoding.UTF8);
                    }
                    else
                    {
                        item.Sql = File.ReadAllText(item.Sql, Encoding.UTF8);
                    }
                }
                item.Date = node.Attributes["date"]?.InnerText;
                item.Desc = node.Attributes["desc"]?.InnerText;

                if (item.Version > GetMaxVersionNum())
                {
                    versions.Add(item);
                }

            }
            return versions;
        }

        /// <inheritdoc />
        public virtual bool CheckAndUpdate()
        {
            var needUpdateVersions = GetNeedUpdateVersions();
            foreach (var needUpdateVersion in needUpdateVersions)
            {
                var updateVersion = UpdateVersion(needUpdateVersion);
                if (!updateVersion)
                {
                    return false;
                }
            }
            return true;
        }


        #endregion
    }
}