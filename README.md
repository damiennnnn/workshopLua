# workshopLua
------------
##### An application that utilises the Steam Web API to automatically produce workshop.lua files for use with a Garry's Mod dedicated server. Works on both Windows and Linux.
------------

**Requires .NET Core 3.1 or .NET 5**
	Download here: https://dotnet.microsoft.com/download
	
**Dependencies**
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/ "Newtonsoft.Json")
- [SteamWebApi2](https://github.com/babelshift/SteamWebAPI2 "SteamWebApi2")

##### A Steam API key is required for retrieving information from the Steam Workshop. You can access yours here: https://steamcommunity.com/dev/apikey

------------
## Build


1. Clone the repository locally.
`git clone https://github.com/damiennnnn/workshopLua.git`

2. Run with `dotnet run` in a terminal, or build with `dotnet build`

A Visual Studio solution is also provided

------------
## Usage

By default, a configuration file will be created, named `workshopgen.json`, in the working directory. This file will store the default config, used if the application is launched with no arguments.

An example config file would look like this:

    {
    	"fullpath" : "/home/example/gmod/garrysmod/lua/autorun/server", 
    	"filename" : "workshop.lua", 
    	"verbose" : "false"
    	"api_key" : "",
    	"workshop_id : ""
    }

`api_key` is a required field, as well as `workshop_id`. The rest can use default values.


The generated Lua file will be stored in the working directory by default, and will be named `workshop.lua`. If a value in `fullpath` is provided, the file will be saved to that directory.

A custom Workshop ID can be set by passing it as a command-line argument.

`./workshop 1234567890`
The ID `1234567890` would be used to generate the `workshop.lua` file, as opposed to the value provided in the config file.


------------

## Example

Example Workshop collection with ID `1234567890`

Addons in collection:
- Orbital Air Strike - ID `430091721`
- vFire - Dynamic Fire - ID `1525218777`

Example `workshop.lua` output:
```
resource.AddWorkshop("430091721") -- Orbital Air Strike
resource.AddWorkshop("152521877") -- vFire - Dynamic Fire
```
