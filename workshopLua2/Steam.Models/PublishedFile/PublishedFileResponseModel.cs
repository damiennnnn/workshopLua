namespace workshopLua2.SteamData;

public class PublishedFileResponseModel
{
    public int Result { get; set; }
    public int ResultCount { get; set; }
    public IEnumerable<PublishedFileDetailsModel> PublishedFileDetails { get; set; }
}