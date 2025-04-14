using System.Text.Json;
using workshopLua2.SteamData;

namespace workshopLua2.Steam.Api;

public class SteamRemoteStorage(string apiKey, HttpClient httpClient)
{
    // Seems to work fine without using the api key?
    public async Task<IEnumerable<ItemDetail>?> GetCollectionItems(string workshopCollectionId)
    {
        var root = await SteamWebApi.GetCollectionDetailsAsync(workshopCollectionId, httpClient);
        
        if (root.Response.CollectionDetails is null)
            throw new NullReferenceException("RootResponse Collection Details is null!");
        
        return root.Response.CollectionDetails.First().Children;
    }

    public async Task<IEnumerable<PublishedFileDetailsModel>> GetFileDetails(IEnumerable<ItemDetail> collectionItems)
    {
        var ids = collectionItems.Select(item =>
        {
            if (!long.TryParse(item.PublishedFileId, out var itemId))
                throw new NullReferenceException("Exception parsing PublishedFileId to long.");

            return itemId;
        });
        
        var response = await SteamWebApi.GetPublishedFileDetailsAsync(ids, httpClient);

        return response.Response.PublishedFileDetails;
    }
}