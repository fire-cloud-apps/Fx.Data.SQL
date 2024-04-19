// See https://aka.ms/new-console-template for more information
using Fx.Data.SQL.Handler;
using Fx.Data.SQL.Interface;
using Spectre.Console;
using Spectre.Console.Cli;

AnsiConsole.Write(
    new FigletText("Test Fx.Data.SQL")
        .LeftJustified()
        .Color(Color.DarkCyan));


//Ref:
//1. Color : https://spectreconsole.net/appendix/colors
string sqlConnectionString = "Server=localhost;Database={0};User Id=sa;Password=System@1984;TrustServerCertificate=True;";

string command;
do
{
    command = InvokePrompt();
    //IEntityService entityService = new SQLEntityService(sqlConnectionString, null);
    switch (command)
    {
        case "Insert":
            AnsiConsole.MarkupLine($"Insert Command [green]{command}[/]!");
            var data = new Dictionary<string, string>();
            data.Add("Date", "10-02-2024");
            data.Add("DateTime", "10-02-2024 10:20:32");
            data.Add("Bit", "true");
            //var insertedItem = entityService.Create("DMS", "TestDates", data);
           //AnsiConsole.MarkupLine($"Insert Record [green]{insertedItem}[/]!");
            break;
        case "SelectOne":
            AnsiConsole.MarkupLine($"Select Command [darkcyan]{command}[/]!");
            break;
        default:
            AnsiConsole.MarkupLine($"Default Command [blue]{command}[/]!");
            break;
    }
    AnsiConsole.WriteLine($"Execution of [green]{command} Completed!");
    AnsiConsole.WriteLine(Environment.NewLine);

} while ((command != "end"));


string InvokePrompt()
{
    // Ask for the user's favorite fruit
    var command = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What's your [green]favorite Executions[/]?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more Executions)[/]")
            .AddChoices(new[] {
            "Insert", "Update", "Delete",
            "Banana", "Blackcurrant", "Blueberry",
            "Cherry", "Cloudberry", "Cocunut","end"
            }));

    // Echo the fruit back to the terminal
    AnsiConsole.WriteLine($"Execution of {command} Begins!");
    return command;
}
//Commands

