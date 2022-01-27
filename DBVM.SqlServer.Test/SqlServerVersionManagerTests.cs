using NUnit.Framework;

namespace DBVM.SqlServer.Test
{
    public class SqlServerVersionManagerTests
    {
        [Test]
        public void SqlServerVersionManagerTest()
        {
            var dbvm = new SqlServerVersionManager($"Data Source=host-1.udschina.com;User Id=sa;Password=ZXCasdqwe123;Initial Catalog=A;TrustServerCertificate=true;Pooling=true;Min Pool Size=1", versionXml: @"D:\UDS\02 Other Codes\UDS.CfgSys\UDS.CfgSys.Domain.Schindler.QOT\DBVM\SqlServer.xml");
            //dbvm.CheckAndUpdate();
            var versions = dbvm.GetNeedUpdateVersions();
            foreach (var versionItem in versions)
            {
                dbvm.UpdateVersion(versionItem);
            }
        }
    }
}