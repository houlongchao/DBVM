using System.Collections.Generic;

namespace DBVM
{
    /// <summary>
    /// 数据库版本管理接口
    /// </summary>
    public interface IVersionManager
    {
        /// <summary>
        /// 获得表中最大的Version号
        /// </summary>
        /// <returns></returns>
        int GetMaxVersionNum();

        /// <summary>
        /// 获得需要升级的 VersionItem
        /// </summary>
        /// <returns></returns>
        IList<VersionItem> GetNeedUpdateVersions();

        /// <summary>
        /// 更新Version
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool UpdateVersion(VersionItem item);

        /// <summary>
        /// 检查并且更新所有需要更新的Version
        /// </summary>
        /// <returns></returns>
        bool CheckAndUpdate();

    }
}