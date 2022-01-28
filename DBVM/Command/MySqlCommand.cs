using DBVM.MySql;
using Spectre.Console.Cli;
using System.Threading.Tasks;

namespace DBVM.Command
{
    internal class MySqlCommand : BaseCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, DbvmCommandSettings settings)
        {
            EnsureHasVersionXml(settings.VersionXml, MySqlVersionManager.DefaultXml);

            if (string.IsNullOrEmpty(settings.Connection))
            {
                settings.Connection = $"Data Source={settings.Host??"localhost"};Port={settings.Port??3306};User ID={settings.User};Password={settings.Password}; Initial Catalog={settings.DbName};Charset=utf8; SslMode=none;Min pool size=1";
            }

            WriteInfo("DbConnection", settings.Connection);
            var dbvm = new MySqlVersionManager(settings.Connection, versionXml: settings.VersionXml ?? MySqlVersionManager.DefaultXml);
            WriteInfo("VersionXML", dbvm.VersionXmlPath);

            await ExecuteAsync(dbvm);
            return 0;
        }
    }
}
