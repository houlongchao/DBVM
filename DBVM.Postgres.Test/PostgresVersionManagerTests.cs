using NUnit.Framework;

namespace DBVM.Postgres.Test
{
    public class MsServerVersionManagerTests
    {
        [Test]
        public void PostgresVersionManagerTest()
        {
            var manager = new PostgresVersionManager($"Host=localhost;Port=5432;Username=postgres;Password=123456; Database=dbvm;Pooling=true;Minimum Pool Size=1");
            //manager.CheckAndUpdate();
            var versions = manager.GetNeedUpdateVersions();
            foreach (var versionItem in versions)
            {
                manager.UpdateVersion(versionItem);
            }
        }
    }
}