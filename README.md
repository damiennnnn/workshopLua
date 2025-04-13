# workshopLua2
------------
##### A small utility for generating workshop.lua files from a provided workshop collection ID.
------------

**Requires .NET 8**
	
**Dependencies**
- [Spectre.Console](https://spectreconsole.net/ "Spectre.Console")
- [SteamWebApi2](https://github.com/babelshift/SteamWebAPI2 "SteamWebApi2")

##### A Steam API key is required for retrieving information from the Steam Workshop. You can access yours here: https://steamcommunity.com/dev/apikey

------------
## Build


1. Clone the repository locally.
`git clone https://github.com/damiennnnn/workshopLua.git`

2. Run with `dotnet run` in a terminal, or build with `dotnet build`

To build as a single-file executable, publish the project with this command: 
`dotnet publish -r PLATFORM -p:PublishSingleFile=true --self-contained false`.

Replace `PLATFORM` with `linux-x64` or `win-x64`, platform dependent.

A Visual Studio solution is also provided

------------
## Usage

```
  --apiKey <apiKey>          The Steam Web API Developer Key to use.
  
  --workshopId <workshopId>  ID of the workshop collection to pull from.
  
  --path <path>              Path to save workshop.lua file to.
  
  --verbose                  Verbose output
  
  --help                     Show help information
```

Running workshopLua2 with this command `workshopLua2 --apiKey STEAM_API_KEY --workshopId WORKSHOP_ID` will produce a workshop.lua file in the current working directory.

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
