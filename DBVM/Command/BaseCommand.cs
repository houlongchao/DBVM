using Spectre.Console;
using Spectre.Console.Cli;

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
    }
}
