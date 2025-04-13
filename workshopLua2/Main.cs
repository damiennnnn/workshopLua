using System.CommandLine;
using System.Diagnostics;
using Spectre.Console;
using SteamWebAPI2.Utilities;
using workshopLua2.SteamData;

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
        var itemsEnumerable = await builder.GetCollectionItems();
        
        // This shouldn't really be possible.
        if (itemsEnumerable is null)
            throw new NullReferenceException("Collection items is null!");
        
        // Avoid resharper complaining about multiple enumeration.
        var itemsArray = itemsEnumerable.ToArray();
        
        AnsiConsole.WriteLine($"{itemsArray.Length} collection item(s) found.");

        var itemCounter = 0;
        
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("Processing...", ctx =>
            {   
                // Stopwatch for profiling purposes
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                foreach (var file in builder.PublishedFileDetails(itemsArray))
                {
                    ctx.Status = $"Processing item {++itemCounter}/{itemsArray.Length}...";
                    workshopLua.AppendAddon(file.PublishedFileId, file.Title);
                    
                    if (verbose)
                        AnsiConsole.MarkupLineInterpolated($"[bold]Verbose[/]: {file.Title} - {file.PublishedFileId}");
                }

                stopwatch.Stop();
                if (verbose)
                    AnsiConsole.MarkupLineInterpolated($"[yellow]{stopwatch.ElapsedMilliseconds}ms elapsed.[/]");
            });
        
        workshopLua.Flush();
        AnsiConsole.MarkupLine("[green]:check_mark_button: Complete![/]");
    }
}