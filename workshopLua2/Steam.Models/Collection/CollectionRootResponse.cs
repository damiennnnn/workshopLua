using System.Text.Json.Serialization;

namespace workshopLua2.SteamData;

public class CollectionRootResponse
{
    // Steam web API returns the response wrapped by this response 'root' object, for some reason. This response property contains the useful information.
    public required CollectionDetailsResponse Response { get; init; }
}

// Source generation stuff for native AOT compatibility
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(CollectionRootResponse))]
internal partial class CollectionRootResponseSourceGenerationContext : JsonSerializerContext
{
    
}