using System;
using System.CommandLine;
using Spectre.Console;

namespace workshopLua;

public class WorkshopLua2
{
        public static void Main(string[] args)
        {
                var apiKeyOption = new Option<string>(
                        name: "--apiKey",
                        description: "The Steam Web API Developer Key to use.");
                var workshopIdOption = new Option<string>(
                        name: "--workshopId",
                        description: "ID of the workshop collection to pull from.");

                var rootCommand =
                        new RootCommand("workshopLua 2 - workshop.lua generator for Garry's Mod dedicated servers");
                rootCommand.AddOption(apiKeyOption);
                rootCommand.AddOption(workshopIdOption);
                
                rootCommand.SetHandler(Handler, apiKeyOption, workshopIdOption);
        }

        public static void Handler(string apiKey, string workshopId)
        {
                Console.WriteLine($"{apiKey}: {workshopId}");
        }
}
