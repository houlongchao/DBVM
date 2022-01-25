using DBVM.SqlServer;
using Spectre.Console.Cli;

namespace DBVM.Command
{
    internal class SqlServerCommand : BaseCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, DbvmCommandSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Connection))
            {
                settings.Connection = $"Data Source={settings.Host??"localhost"},{settings.Port??1433};User Id={settings.User};Password={settings.Password};Initial Catalog={settings.DbName};TrustServerCertificate=true;Pooling=true;Min Pool Size=1";
            }

            var dbvm = new SqlServerVersionManager(settings.Connection);
            await ExecuteAsync(dbvm);
            return 0;
        }
    }
}
