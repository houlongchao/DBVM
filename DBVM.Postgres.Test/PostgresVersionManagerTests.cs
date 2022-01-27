using NUnit.Framework;

namespace DBVM.Postgres.Test
{
    public class PostgresVersionManagerTests
    {
        [Test]
        public void PostgresVersionManagerTest()
        {
            var dbvm = new PostgresVersionManager($"Host=localhost;Port=5432;Username=postgres;Password=123456; Database=dbvm;Pooling=true;Minimum Pool Size=1");
            //dbvm.CheckAndUpdate();
            var versions = dbvm.GetNeedUpdateVersions();
            foreach (var versionItem in versions)
            {
                dbvm.UpdateVersion(versionItem);
            }
        }
    }
}