using System.Text.Json.Serialization;

namespace workshopLua2.SteamData;

public class ItemDetail
{
    public string? PublishedFileId { get; set; }
    public int SortOrder { get; set; }
    public int FileType { get; set; }
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(ItemDetail))]
internal partial class ItemDetailSourceGenerationContext : JsonSerializerContext
{
    
}