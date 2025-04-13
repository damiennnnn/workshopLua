using System.CommandLine;
using Spectre.Console;
using SteamWebAPI2.Utilities;

namespace workshopLua2;

public static class WorkshopLua
{
    public static async Task Main(string[] args)
    {
        var apiKeyOption = new Option<string>(
            "--apiKey",
            "The Steam Web API Developer Key to use.");
        apiKeyOption.AddValidator(r =>
        {
            if (string.IsNullOrEmpty(r.GetValueForOption(apiKeyOption)))
                r.ErrorMessage = "Please provide a non-empty Steam Web API Developer Key";
        });

        var workshopIdOption = new Option<string>(
            "--workshopId",
            "ID of the workshop collection to pull from.");
        workshopIdOption.AddValidator(optionResult =>
        {
            if (string.IsNullOrEmpty(optionResult.GetValueForOption(workshopIdOption)))
                optionResult.ErrorMessage = "Please provide a non-empty Workshop ID";
        });
        
        var filePathOption = new Option<string>(
            "--path",
            "Path to save workshop.lua file to.");
        
        var verboseOption = new Option<bool>("--verbose",
            () => true,
            "Verbose output");
        
        var rootCommand =
            new RootCommand("workshopLua 2 - workshop.lua generator for Garry's Mod dedicated servers");
        rootCommand.AddOption(apiKeyOption);
        rootCommand.AddOption(workshopIdOption);
        rootCommand.AddOption(filePathOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler(RootCommandHandler, apiKeyOption, workshopIdOption, filePathOption, verboseOption);

        await rootCommand.InvokeAsync(args);
    }
    private static void RootCommandHandler(string apiKey, string workshopId, string filePath, bool verbose)
    {
        DoWorkshopProcess(apiKey, workshopId, filePath, verbose)
            .Wait();
    }

    private static async Task DoWorkshopProcess(string apiKey, string workshopId, string filePath, bool verbose)
    {
        var interfaceFactory = new SteamWebInterfaceFactory(apiKey);
        
        var builder = new WorkshopLuaUtilities(workshopId, interfaceFactory);
        var workshopLua = new WorkshopLuaFile(filePath);
        
        AnsiConsole.WriteLine($"Gathering collection information for collection ID {workshopId}...");
        var items = await builder.GetCollectionItems();
        
        var table = new Table().LeftAligned();
        var columns = new[]
        {
            new TableColumn("Addon Title"), new TableColumn("ID")
        };
        
        if (verbose)
            table.AddColumns(columns);
        
        await AnsiConsole.Live(table)
            .StartAsync(async ctx =>
            {
                await foreach (var file in builder.PublishedFileDetails(items))
                { 
                    workshopLua.AppendAddon(file.Data.PublishedFileId, file.Data.Title);
                    
                    if (!verbose) continue;
                    table.AddRow(Markup.Escape(file.Data.Title), file.Data.PublishedFileId.ToString());
                    ctx.Refresh();
                }
            });
        
        workshopLua.Flush();
        AnsiConsole.WriteLine("Complete!");
    }
}