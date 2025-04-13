using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steam.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace workshopgen;

internal class Program
{
    public static string WorkshopID = string.Empty;
    public static string APIKey = string.Empty;
    public static bool Verbose = true;
    public static Config config = new();

    public static SteamRemoteStorage SteamInterface;

    private static async Task<string> SendRequestAsync()
    {
        var data = new Dictionary<string, string>
        {
            { "collectioncount", "1" },
            { "publishedfileids[0]", config.workshop_id }
        };

        var HTTP = new HttpClient();
        var content = new FormUrlEncodedContent(data);
        var response = await HTTP.PostAsync("https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/",
            content);
        return await response.Content.ReadAsStringAsync();
    }

    private static async Task Mainff(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Title = "WorkshopGen";

        var workingPath = Directory.GetCurrentDirectory();
        var configLocation = Path.Combine(workingPath, "workshopgen.json");

        if (File.Exists(configLocation))
        {
            Console.WriteLine("Found config file at: {0}", configLocation);
            var configContents = File.ReadAllText(configLocation);
            config = JsonConvert.DeserializeObject<Config>(configContents);
        }
        else
        {
            Console.WriteLine("Creating config file at: {0}", configLocation);
            File.WriteAllText(configLocation, JsonConvert.SerializeObject(config));
        }

        if (string.IsNullOrEmpty(config.api_key))
        {
            Console.WriteLine("No API key found.");
            return;
        }

        if (string.IsNullOrEmpty(config.workshop_id))
        {
            Console.WriteLine("No Workshop ID found.");
            return;
        }

        if (string.IsNullOrEmpty(config.verbose) || config.verbose != "true")
            Verbose = false;

        if (string.IsNullOrEmpty(config.filename))
            config.filename = "workshopLua.lua";

        if (string.IsNullOrEmpty(config.fullpath))
            config.fullpath = workingPath;

        if (args?.Length > 0)
        {
            Console.WriteLine("Using provided Workshop ID: {0}", args[0]);
            config.workshop_id = args[0];
        }
        else
        {
            Console.WriteLine("Using default Workshop ID: {0}", config.workshop_id);
        }

        SetupInterfaces(); // Setup the SteamRemoteStorage interface
        var response = await SendRequestAsync(); // Get the response from Steam WebAPI as string


        var Deserialised = JsonConvert.DeserializeObject<Root>(response); // Deserialises into a Root object.
        var CollectionContents =
            Deserialised.response.collectiondetails[0]
                .children; // We are looking for only the children of the collection.

        // Setting up a visual progress bar. (The generation process can take a while.)
        Console.Clear();
        var count = CollectionContents.Count;
        var progress = 0;
        Console.Write("[");
        for (var i = 0; i < 50; i++) Console.Write(" ");
        Console.WriteLine("]");
        //

        var Files = new List<PublishedFileDetailsModel>();
        foreach (var file in Deserialised.response.collectiondetails[0].children)
        {
            var webResponse = await SteamInterface.GetPublishedFileDetailsAsync(ulong.Parse(file.publishedfileid));
            try
            {
                var fileDetails = webResponse.Data;

                // Progress bar
                progress++;
                var percentage = progress / (decimal)count * 100;
                Console.SetCursorPosition(1 + (int)(percentage / 2), 0);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\u25A0"); // block character
                Console.ResetColor();
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(" {0}/{1} {2}%", progress, count, (int)percentage);
                Console.WriteLine("");
                //
                Console.WriteLine(string.Concat(fileDetails.Title, " ", fileDetails.PublishedFileId
                    , "                                                                            ")); // Output file information

                Files.Add(fileDetails);
            }
            catch (Exception e)
            {
                if (Verbose)
                    Console.WriteLine(e.Message);
            }
        }

        var Lua = new List<string>(); // Create a list of strings to write to our workshopLua.lua

        foreach (var file in Files)
        {
            var line = string.Format("resource.AddWorkshop(\"{0}\") ", file.PublishedFileId);
            line += string.Format("-- {0}", file.Title);
            Lua.Add(line);
        }

        if (Verbose)
        {
            Console.Clear();
            foreach (var line in Lua)
                Console.WriteLine(line);
        }

        var filepath = Path.Combine(config.fullpath, config.filename);
        using (var writer = new StreamWriter(File.Open(filepath, FileMode.OpenOrCreate)))
        {
            foreach (var str in Lua)
                writer.WriteLine(str);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Saved to: {0}", filepath);
            Console.ResetColor();
        }

        Console.CursorVisible = true;
    }

    private static void SetupInterfaces()
    {
        var interfaceFactory = new SteamWebInterfaceFactory(config.api_key);
        SteamInterface = interfaceFactory.CreateSteamWebInterface<SteamRemoteStorage>(new HttpClient());
    }

    public class Config
    {
        public string fullpath { get; set; }
        public string filename { get; set; }
        public string verbose { get; set; }
        public string api_key { get; set; }
        public string workshop_id { get; set; }
    }

    public class Child
    {
        public string publishedfileid { get; set; }
        public int sortorder { get; set; }
        public int filetype { get; set; }
    }

    public class Collectiondetail
    {
        public string publishedfileid { get; set; }
        public int result { get; set; }
        public List<Child> children { get; set; }
    }

    public class Response
    {
        public int result { get; set; }
        public int resultcount { get; set; }
        public List<Collectiondetail> collectiondetails { get; set; }
    }

    public class Root
    {
        public Response response { get; set; }
    }
}