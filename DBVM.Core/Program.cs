//using System;
//using UDS.DBVersionManager.MsServer;
//using UDS.DBVersionManager.Mysql;
//using UDS.DBVersionManager.Oracle;

//namespace DBVM
//{
//    /// <summary>
//    /// 入口
//    /// </summary>
//    public class Program
//    {
//        /// <summary>
//        /// 主函数
//        /// </summary>
//        /// <param name="args"></param>
//        static int Main(string[] args)
//        {
//            var defaultForegroundColor = Console.ForegroundColor;
//            try
//            {
//                if (args.Length != 4)
//                {
//                    ShowCmdInfo();
//                    return -1;
//                }

//                string dbType = args[0].Equals("-t") ? args[1] : args[2].Equals("-t") ? args[3] : string.Empty;
//                string connStr = args[0].Equals("-c") ? args[1] : args[2].Equals("-c") ? args[3] : string.Empty;

//                if (string.IsNullOrEmpty(dbType) || string.IsNullOrEmpty(connStr) || dbType.Equals(connStr))
//                {
//                    ShowCmdInfo();
//                    return -1;
//                }

//                IVersionManager versionManager;

//                switch (dbType)
//                {
//                    case "sqlserver":
//                        versionManager = new MsServerVersionManager(connStr);
//                        break;
//                    case "mysql":
//                        versionManager = new MysqlVersionManager(connStr);
//                        break;
//                    case "oracle":
//                        versionManager = new OracleVersionManager(connStr);
//                        break;
//                    default:
//                        Console.ForegroundColor = ConsoleColor.Yellow;
//                        Console.Error.WriteLine("未识别数据库类型");
//                        Console.ForegroundColor = defaultForegroundColor;
//                        return -1;
//                }

//                var needUpdateVersions = versionManager.GetNeedUpdateVersions();
//                Console.WriteLine($"有 {needUpdateVersions.Count} 条新版本需要升级");

//                foreach (var versionItem in needUpdateVersions)
//                {
//                    var result = versionManager.UpdateVersion(versionItem);
//                    if (result)
//                    {
//                        Console.ForegroundColor = ConsoleColor.Green;
//                        Console.WriteLine($"{"SUCCESS",-10}{versionItem.Version,-5}{versionItem.Author,-15}{versionItem.Date,-10}{versionItem.Desc}");
//                        Console.ForegroundColor = defaultForegroundColor;
//                    }
//                    else
//                    {
//                        Console.ForegroundColor = ConsoleColor.Red;
//                        Console.WriteLine($"{"ERROR",-10}{versionItem.Version,-5}{versionItem.Author,-15}{versionItem.Date,-10}{versionItem.Sql}");
//                        Console.ForegroundColor = defaultForegroundColor;
//                    }
//                }

//                Console.WriteLine();
//                Console.WriteLine("运行结束");
//                return 0;
//            }
//            catch (Exception e)
//            {
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.Error.WriteLine(e);
//                Console.ForegroundColor = defaultForegroundColor;
//                return -1;
//            }
//        }

//        static void ShowCmdInfo()
//        {
//            var defaultForegroundColor = Console.ForegroundColor;
//            Console.ForegroundColor = ConsoleColor.DarkGray;
//            string sqlserverConn = "server=localhost;uid=sa;pwd=123456;database=testdb";
//            string mysqlConn = "Server=localhost;Database=testdb;Uid=root;Pwd=123456;";
//            string oracleConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=7521))(CONNECT_DATA=(SID=orcl)));User Id=root;Password=123456;";

//            Console.WriteLine();
//            Console.WriteLine("===== UDS 简易数据库升级工具 =====");
//            Console.WriteLine();
//            Console.WriteLine("UDS.DBVersionManager.exe [options]");
//            Console.WriteLine();
//            Console.WriteLine("options:");
//            Console.WriteLine("  -t[ype] <DbType>");
//            Console.WriteLine("          <DbTtype> : sqlserver | mysql | oracle");
//            Console.WriteLine("  -c[onnection] <ConnectionString>");
//            Console.WriteLine();
//            Console.WriteLine("eg:");
//            Console.WriteLine($"   UDS.DBVersionManager.exe -t sqlserver -c \"{sqlserverConn}\"");
//            Console.WriteLine($"   UDS.DBVersionManager.exe -t mysql -c \"{mysqlConn}\"");
//            Console.WriteLine($"   UDS.DBVersionManager.exe -t oracle -c \"{oracleConn}\"");
//            Console.WriteLine();
//            Console.ForegroundColor = defaultForegroundColor;
//        }
//    }
//}