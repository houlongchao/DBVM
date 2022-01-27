using DBVM.MySql;
using Spectre.Console.Cli;

namespace DBVM.Command
{
    internal class MySqlCommand : BaseCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, DbvmCommandSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Connection))
            {
                settings.Connection = $"Data Source={settings.Host??"localhost"};Port={settings.Port??3306};User ID={settings.User};Password={settings.Password}; Initial Catalog={settings.DbName};Charset=utf8; SslMode=none;Min pool size=1";
            }

            var dbvm = new MySqlVersionManager(settings.Connection, versionXml: settings.VersionXml ?? "MySql.xml");
            await ExecuteAsync(dbvm);
            return 0;
        }
    }
}
