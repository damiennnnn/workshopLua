using System.Numerics;
using System.Text.Json;
using workshopLua2.SteamData;

namespace workshopLua2.Steam.Api;

public static class SteamWebApi
{
    private const string GetCollectionDetailsUri =
        "https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1";

    private const string GetPublishedFileDetailsUri =
        "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1";

    private static FormUrlEncodedContent CreateFormUrlEncodedContent(string workshopCollectionId)
    {
        return new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "collectioncount", "1" },
            { "publishedfileids[0]", workshopCollectionId }
        });
    }
    
    private static FormUrlEncodedContent CreateFormUrlEncodedContent(IEnumerable<long> workshopCollectionIds)
    {
        var idArray = workshopCollectionIds.ToArray();
        
        var content = new Dictionary<string, string>()
        {
            { "itemcount", $"{idArray.Length}" }
        };
        
        for (var i = 0; i < idArray.Length; i++)
        {
            content.Add($"publishedfileids[{i}]", idArray[i].ToString());
        }
        
        return new FormUrlEncodedContent(content);
    }
    
    public static async Task<CollectionRootResponse> GetCollectionDetailsAsync(string workshopCollectionId, HttpClient httpClient)
    {
        // Send post request to Steam web API to get collection details.
        var response = await httpClient.PostAsync(GetCollectionDetailsUri,
            CreateFormUrlEncodedContent(workshopCollectionId));
        response.EnsureSuccessStatusCode();

        // Deserialise this into our response root object.
        return await JsonSerializer.DeserializeAsync(
            await response.Content.ReadAsStreamAsync(),
            CollectionRootResponseSourceGenerationContext.Default.CollectionRootResponse) 
               ?? throw new NullReferenceException("Deserialized response is null.");
    }
    
    public static async Task<PublishedFileRootModel> GetPublishedFileDetailsAsync(IEnumerable<long> workshopCollectionIds, HttpClient httpClient)
    {
        // Send post request to Steam web API to get published file details.
        var response = await httpClient.PostAsync(GetPublishedFileDetailsUri,
            CreateFormUrlEncodedContent(workshopCollectionIds));
        response.EnsureSuccessStatusCode();

        // Deserialise this into our response root object.
        return await JsonSerializer.DeserializeAsync(
                   await response.Content.ReadAsStreamAsync(),
                   PublishedFileRootSourceGenerationContext.Default.PublishedFileRootModel) 
               ?? throw new NullReferenceException("Deserialized response is null.");
    }
}