﻿using DBVM.SqlServer;
using Spectre.Console.Cli;
using System.Threading.Tasks;

namespace DBVM.Command
{
    internal class SqlServerCommand : BaseCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context, DbvmCommandSettings settings)
        {
            EnsureHasVersionXml(settings.VersionXml, SqlServerVersionManager.DefaultXml);
            
            if (string.IsNullOrEmpty(settings.Connection))
            {
                settings.Connection = $"Data Source={settings.Host??"localhost"},{settings.Port??1433};User Id={settings.User};Password={settings.Password};Initial Catalog={settings.DbName};TrustServerCertificate=true;Pooling=true;Min Pool Size=1";
            }

            WriteInfo("DbConnection", settings.Connection);
            var dbvm = new SqlServerVersionManager(settings.Connection, versionXml: settings.VersionXml ?? SqlServerVersionManager.DefaultXml);
            WriteInfo("VersionXML", dbvm.VersionXmlPath);

            await ExecuteAsync(dbvm);
            return 0;
        }
    }
}
