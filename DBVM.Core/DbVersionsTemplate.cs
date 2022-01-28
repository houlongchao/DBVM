using System.IO;
using System.Reflection;

namespace DBVM
{
    /// <summary>
    /// Db Version Xml 模板
    /// </summary>
    public static class DbVersionsTemplate
    {
        /// <summary>
        /// 获取 Db Version Xml 模板
        /// </summary>
        /// <returns></returns>
        public static string Get()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DBVM.dbversions.temp");
            var xml = new StreamReader(stream).ReadToEnd();
            return xml;
        }
    }
}
