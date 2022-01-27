using NUnit.Framework;

namespace DBVM.MySql.Test
{
    public class MySqlVersionManagerTests
    {
        [Test]
        public void MySqlVersionManagerTest()
        {
            var dbvm = new MySqlVersionManager($"Server=localhost;Database=dbvm;Uid=root;Pwd=123456;");
            dbvm.CheckAndUpdate();
        }
    }
}