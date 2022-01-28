using DBVM.Command;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Threading.Tasks;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        AnsiConsole.Write(new FigletText("DBVM").Color(Color.Blue));

        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<MySqlCommand>("MySql");
            config.AddCommand<SqlServerCommand>("SqlServer");
            config.AddCommand<PostgresCommand>("Postgres");
        });
        return await app.RunAsync(args);
    }
}
