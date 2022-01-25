using DBVM.Postgres;
using Spectre.Console.Cli;

namespace DBVM.Command
{
    internal class PostgresCommand : BaseCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, DbvmCommandSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Connection))
            {
                settings.Connection = $"Host={settings.Host??"localhost"};Port={settings.Port??5432};Username={settings.User};Password={settings.Password}; Database={settings.DbName};Pooling=true;Minimum Pool Size=1";
            }

            var dbvm = new PostgresVersionManager(settings.Connection);
            await ExecuteAsync(dbvm);
            return 0;
        }
    }
}
