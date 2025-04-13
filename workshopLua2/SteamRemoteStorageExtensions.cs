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
    
    public static async Task<ISteamWebResponse<IReadOnlyCollection<PublishedFileDetailsModel>>> GetFileDetails(this ISteamRemoteStorage storage,
        IEnumerable<ItemDetail> collectionItems)
    {
        var ids = collectionItems.Select(item =>
        {
            if (!ulong.TryParse(item.PublishedFileId, out var itemId))
                throw new NullReferenceException("Exception parsing PublishedFileId to uint.");

            return itemId;
        }).ToList();

        return await storage.GetPublishedFileDetailsAsync((uint)ids.Count, ids);
    }

    private struct ResponseRoot
    {
        // Steam web API returns the response wrapped by this response 'root' object, for some reason. This response property contains the useful information.
        public CollectionDetailsResponse Response { get; init; }
    }
}