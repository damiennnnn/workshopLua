using System.Net;
using Steam.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using workshopLua2.SteamData;

namespace workshopLua2;

public class WorkshopLuaUtilities
{
    private readonly HttpClient _httpClient;
    private readonly ISteamWebInterfaceFactory _interfaceFactory;
    private readonly ISteamRemoteStorage _steamRemoteStorage;
    private readonly string _workshopId;

    public WorkshopLuaUtilities(string workshopId, ISteamWebInterfaceFactory interfaceFactory)
    {
        _workshopId = workshopId;
        _interfaceFactory = interfaceFactory;
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
        };
        _httpClient = new HttpClient(handler);
        _steamRemoteStorage = _interfaceFactory.CreateSteamWebInterface<SteamRemoteStorage>(_httpClient);
    }

    public async Task<IEnumerable<ItemDetail>?> GetCollectionItems()
    {
        return await _steamRemoteStorage.GetCollectionItems(_workshopId, _httpClient);
    }

    public IReadOnlyCollection<PublishedFileDetailsModel> PublishedFileDetails(
        IEnumerable<ItemDetail> itemDetails)
    {
        return _steamRemoteStorage.GetFileDetails(itemDetails).Result.Data;
    }
}