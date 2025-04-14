using System.Text.Json.Serialization;

namespace workshopLua2.SteamData;

public class PublishedFileRootModel
{
    public PublishedFileResponseModel Response { get; set; }
}

// Source generation stuff for native AOT compatibility
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(PublishedFileRootModel))]
internal partial class PublishedFileRootSourceGenerationContext : JsonSerializerContext
{
    
}