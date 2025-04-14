using System.CommandLine;
using System.Diagnostics;
using System.Net;
using workshopLua2.Steam.Api;

namespace workshopLua2;

public static class WorkshopLua
{
    public static async Task Main(string[] args)
    {
        var apiKeyOption = new Option<string>(
            "--apiKey",
            parseArgument: result =>
            {
                if (!result.Tokens.Any())
                {
                    result.ErrorMessage = "--apiKey is required";
                    return string.Empty;
                }
                
                var value = result.Tokens.Single().Value;
                if (string.IsNullOrEmpty(value) || value.Length != 32)
                    result.ErrorMessage = "Please provide a valid Steam API Key";

                return value;
            },
            isDefault: true,
            "The Steam Web API Developer Key to use.");

        var workshopIdOption = new Option<string>(
            "--workshopId",
            parseArgument: result =>
            { 
                if (!result.Tokens.Any())
                {
                    result.ErrorMessage = "--workshopId is required";
                    return string.Empty;
                }
                
                var value = result.Tokens.Single().Value;
                if (!long.TryParse(value, out _))
                    result.ErrorMessage = "Please provide a valid Workshop ID.";

                return value;
            },
            isDefault: true,
            "ID of the workshop collection to pull from.");
        
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
        // Spent too much time battling with async methods, this works and I don't want to touch it
        DoWorkshopProcess(apiKey, workshopId, filePath, verbose)
            .Wait();
    }

    private static async Task DoWorkshopProcess(string apiKey, string workshopId, string filePath, bool verbose)
    {
        var remoteStorage = new SteamRemoteStorage(apiKey, new HttpClient(
            new HttpClientHandler()
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
        }));
        
        var workshopLua = new WorkshopLuaFile(filePath);
        
        Console.WriteLine($"Gathering collection information for collection ID {workshopId}...");
        var itemsEnumerable = await remoteStorage.GetCollectionItems(workshopId);
        
        // This shouldn't really be possible. ReSharper will complain without it.
        if (itemsEnumerable is null)
            throw new NullReferenceException("Collection items is null!");
        
        // Avoid resharper complaining about multiple enumeration.
        var itemsArray = itemsEnumerable.ToArray();
        
        Console.WriteLine($"{itemsArray.Length} collection item(s) found.");

        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (var file in await remoteStorage.GetFileDetails(itemsArray))
        {
            // Append title and ID info to workshop.lua file
            workshopLua.AppendAddon(file.PublishedFileId, file.Title);
                    
            if (verbose)
                Console.WriteLine($"Verbose: {file.Title} - {file.PublishedFileId}");
        }

        stopwatch.Stop();
        if (verbose)
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms elapsed.");
        
        // Flush underlying StreamWriter to ensure we actually write to the workshop.lua file.
        workshopLua.Flush();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\u2705 Complete!");
        Console.ResetColor();
    }
}