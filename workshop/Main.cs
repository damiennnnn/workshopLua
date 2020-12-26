using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamWebAPI2;
using SteamWebAPI2.Utilities;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Mappings;
using Steam.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace workshopgen
{
    class Program
    {
        public static string WorkshopID = string.Empty;
        public static string APIKey = string.Empty;
        public static bool Verbose = true;
        public static Config config = new Config();

        public class Config
        {
            public string fullpath { get; set; }
            public string filename { get; set; }
            public string verbose { get; set; }
            public string api_key { get; set; }
            public string workshop_id { get; set; }
        }


        public static SteamRemoteStorage SteamInterface;

        static async Task<string> SendRequestAsync()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "collectioncount",  "1" },
                { "publishedfileids[0]",  config.workshop_id}
            };

            HttpClient HTTP = new HttpClient();
            var content = new FormUrlEncodedContent(data);
            var response = await HTTP.PostAsync("https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/", content);
            return await response.Content.ReadAsStringAsync();
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Title = "WorkshopGen";

            string workingPath = Directory.GetCurrentDirectory();
            string configLocation = Path.Combine(workingPath, "workshopgen.json");

            if (File.Exists(configLocation))
            {
                Console.WriteLine(string.Format("Found config file at: {0}", configLocation));
                string configContents = File.ReadAllText(configLocation);
                config = JsonConvert.DeserializeObject<Config>(configContents);
            }
            else
            {
                Console.WriteLine(string.Format("Creating config file at: {0}", configLocation));
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

            if (string.IsNullOrEmpty(config.verbose) || (config.verbose != "true"))
                Verbose = false;

            if (string.IsNullOrEmpty(config.filename))
                config.filename = "workshop.lua";

            if (string.IsNullOrEmpty(config.fullpath))
                config.fullpath = workingPath;

            if (!string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine(string.Format("Using provided Workshop ID: {0}", args[0]));
                config.workshop_id = args[0];
            }
            else
            {
                Console.WriteLine(string.Format("Using default Workshop ID: {0}", config.workshop_id));
            }

            SetupInterfaces();      // Setup the SteamRemoteStorage interface
            var response = await SendRequestAsync(); // Get the response from Steam WebAPI as string


            Root Deserialised = JsonConvert.DeserializeObject<Root>(response); // Deserialises into a Root object.
            List<Child> CollectionContents = Deserialised.response.collectiondetails[0].children; // We are looking for only the children of the collection.

            // Setting up a visual progress bar. (The generation process can take a while.)
            Console.Clear();
            int count = CollectionContents.Count;
            int progress = 0;
            Console.Write("[");
            for (int i = 0; i < count; i++)
            {
                Console.Write(" ");
            }
            Console.WriteLine("]");
            //

            List<PublishedFileDetailsModel> Files = new List<PublishedFileDetailsModel>();
            foreach (var file in Deserialised.response.collectiondetails[0].children)
            {
                var webResponse = await SteamInterface.GetPublishedFileDetailsAsync(ulong.Parse(file.publishedfileid));
                PublishedFileDetailsModel fileDetails = webResponse.Data;

                // Progress bar
                progress++;
                Console.SetCursorPosition(progress, 0);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\u25A0"); // block character
                Console.ResetColor();
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(string.Format(" {0}/{1} ", progress, count));
                Console.WriteLine("");
                //

                if (Verbose)
                    Console.WriteLine(string.Concat(fileDetails.Title, " ", fileDetails.PublishedFileId)); // Output information in realtime

                Files.Add(fileDetails);
            }

            List<string> Lua = new List<string>(); // Create a list of strings to write to our workshop.lua

            foreach (var file in Files)
            {
                string line = string.Format("resource.AddWorkshop(\"{0}\") ", file.PublishedFileId);
                line += string.Format("-- {0}", file.Title);
                Lua.Add(line);
            }

            if (Verbose)
            {
                Console.Clear();
                foreach (var line in Lua)
                    Console.WriteLine(line);
            }

            string filepath = Path.Combine(config.fullpath, config.filename);
            using (StreamWriter writer = new StreamWriter(File.Open(filepath, FileMode.OpenOrCreate)))
            {
                foreach (var str in Lua)
                    writer.WriteLine(str);
            }

        }

        static void SetupInterfaces()
        {
            var interfaceFactory = new SteamWebInterfaceFactory(config.api_key);
            SteamInterface = interfaceFactory.CreateSteamWebInterface<SteamRemoteStorage>(new HttpClient());
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
}
