using NUnit.Framework;

namespace DBVM.SqlServer.Test
{
    public class MsServerVersionManagerTests
    {
        [Test]
        public void MsServerVersionManagerTest()
        {
            var manager = new SqlServerVersionManager($"server=localhost;uid=sa;pwd=Hlc12345678;database=dbvm");
            //manager.CheckAndUpdate();
            var versions = manager.GetNeedUpdateVersions();
            foreach (var versionItem in versions)
            {
                manager.UpdateVersion(versionItem);
            }
        }
    }
}