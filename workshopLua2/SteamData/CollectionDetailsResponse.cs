namespace workshopLua2.SteamData;

public class CollectionDetailsResponse
{
    public int Result { get; set; }
    public int ResultCount { get; set; }
    public IEnumerable<CollectionDetail>? CollectionDetails { get; set; }
}