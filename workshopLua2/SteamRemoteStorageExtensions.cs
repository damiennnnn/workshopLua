using System.Text.Json;
using Steam.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using workshopLua2.SteamData;

namespace workshopLua2;

public static class SteamRemoteStorageExtensions
{
    private static FormUrlEncodedContent CreateFormUrlEncodedContent(string workshopCollectionId)
    {
        return new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "collectioncount", "1" },
            { "publishedfileids[0]", workshopCollectionId }
        });
    }

    // SteamWebApi2 library doesn't include a wrapper for the GetCollectionItems API, so we have to create our own.
    public static async Task<IEnumerable<ItemDetail>?> GetCollectionItems(this ISteamRemoteStorage storage,
        string workshopCollectionId, HttpClient httpClient)
    {
        // Send post request to Steam web API to get collection details.
        var response = await httpClient.PostAsync(SteamWebApiUri.GetCollectionDetailsUri,
            CreateFormUrlEncodedContent(workshopCollectionId));
        response.EnsureSuccessStatusCode();

        // Deserialise this into our response root object.
        var deserialised = await JsonSerializer.DeserializeAsync<ResponseRoot>(
            await response.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (deserialised.Response.CollectionDetails != null)
            return deserialised.Response.CollectionDetails.First().Children;

        throw new NullReferenceException("Collection details is null in Steam API response.");
    }
    
    // Using async enumerable lets us do fun things with the live display of Spectre.Console. I still don't understand how they fully work
    public static async IAsyncEnumerable<ISteamWebResponse<PublishedFileDetailsModel>> GetFileDetails(this ISteamRemoteStorage storage,
        IEnumerable<ItemDetail> collectionItems)
    {
        foreach (var item in collectionItems)
        {
            if (!uint.TryParse(item.PublishedFileId, out var fileId))
                throw new NullReferenceException("PublishedFileId is null in Steam API response.");
            
            yield return await storage.GetPublishedFileDetailsAsync(fileId);
        }
    }

    private struct ResponseRoot
    {
        // Steam web API returns the response wrapped by this response 'root' object, for some reason. This response property contains the useful information.
        public CollectionDetailsResponse Response { get; init; }
    }
}