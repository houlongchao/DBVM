using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DBVM.Command
{
    abstract class BaseCommand : AsyncCommand<DbvmCommandSettings>
    {
        public Task ExecuteAsync(IVersionManager dbvm)
        {
            var needUpdates = dbvm.GetNeedUpdateVersions();
            var table = new Table();
            
            AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Top)
                .Start(ctx =>
                {
                    table.Title($"[[ [yellow]{needUpdates.Count} Versions Wait Update [/] ]]");
                    table.AddColumn("Version");
                    table.AddColumn("Date");
                    table.AddColumn("Author");
                    table.AddColumn("Desc");
                    table.AddColumn("Status");
                    foreach (var version in needUpdates)
                    {
                        table.AddRow(version.Version.ToString(), version.Date, version.Author, version.Desc, "[grey]Wait[/]");
                    }

                    ctx.Refresh();


                    for (var i = 0; i < needUpdates.Count; i++)
                    {
                        table.UpdateCell(i, 4, "[aqua]Running[/]");
                        ctx.Refresh();
                        dbvm.UpdateVersion(needUpdates[i]);
                        Thread.Sleep(500);

                        table.UpdateCell(i, 4, "[green]SUCCESS[/]");
                        ctx.Refresh();
                    }

                    table.Caption("[[ [green]FINISHED[/] ]]");
                    ctx.Refresh();
                });
            return Task.CompletedTask;
        }

        protected void WriteInfo(string title, string message)
        {
            AnsiConsole.MarkupLine($"{title}: [grey]{message}[/]{Environment.NewLine}");
        }

        protected void EnsureHasVersionXml(string xmlPath, string defaultXmlPath)
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), xmlPath ??"")) &&
                !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), BaseVersionManager.DefaultFolder, defaultXmlPath)) &&
                !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), defaultXmlPath)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), BaseVersionManager.DefaultFolder));
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), BaseVersionManager.DefaultFolder, defaultXmlPath), DbVersionsTemplate.Get());
            }
        }
    }
}
