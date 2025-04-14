namespace workshopLua2;

public class WorkshopLuaFile
{
    private StreamWriter _writer;
    public WorkshopLuaFile(string path)
    {
        // If supplied path exists, create workshop.lua file there. Otherwise, create it in our working directory.
        _writer = new StreamWriter(
            Directory.Exists(path) 
                ? Path.Combine(path, "workshop.lua") 
                : "workshop.lua");
        
        _writer.WriteLine("-- Created by workshopLua2");
        _writer.WriteLine("-- github.com/damiennnnn/workshopLua");
    }

    public void AppendAddon(string id, string title)
    {
        string toAppend = $"resource.AddWorkshop(\"{id}\") -- {title}";
        
        _writer.WriteLine(toAppend);
    }
    
    public void Flush() => _writer.Flush();
}