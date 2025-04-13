using System.Text.Json.Serialization;

namespace workshopLua2.SteamData;

public class CollectionDetail
{
    public string? PublishedFileId { get; set; }
    public int Result { get; set; }
    public IEnumerable<ItemDetail>? Children { get; set; } 
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(CollectionDetail))]
internal partial class CollectionDetailSourceGenerationContext : JsonSerializerContext
{
    
}