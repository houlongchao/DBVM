using DBVM.Postgres;
using Spectre.Console.Cli;
using System.Threading.Tasks;

namespace DBVM.Command
{
    internal class PostgresCommand : BaseCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, DbvmCommandSettings settings)
        {
            EnsureHasVersionXml(settings.VersionXml, PostgresVersionManager.DefaultXml);

            if (string.IsNullOrEmpty(settings.Connection))
            {
                settings.Connection = $"Host={settings.Host??"localhost"};Port={settings.Port??5432};Username={settings.User};Password={settings.Password}; Database={settings.DbName};Pooling=true;Minimum Pool Size=1";
            }

            WriteInfo("DbConnection", settings.Connection);
            var dbvm = new PostgresVersionManager(settings.Connection, versionXml: settings.VersionXml ?? PostgresVersionManager.DefaultXml);
            WriteInfo("VersionXML", dbvm.VersionXmlPath);

            await ExecuteAsync(dbvm);
            return 0;
        }
    }
}
