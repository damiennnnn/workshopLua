using System.Text.Json.Serialization;

namespace workshopLua2.SteamData;

public class PublishedFileDetailsModel
{
        [JsonPropertyName("publishedfileid")]
        public string PublishedFileId { get; set; }

        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("creator")]
        public string Creator { get; set; }

        [JsonPropertyName("creator_app_id")]
        public int CreatorAppId { get; set; }

        [JsonPropertyName("consumer_app_id")]
        public int ConsumerAppId { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("file_size")]
        public string FileSize { get; set; }

        [JsonPropertyName("file_url")]
        public string FileUrl { get; set; }

        [JsonPropertyName("hcontent_file")]
        public string FileContentHandle { get; set; }

        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonPropertyName("hcontent_preview")]
        public string PreviewContentHandle { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("time_created")]
        public int TimeCreated { get; set; }

        [JsonPropertyName("time_updated")]
        public int TimeUpdated { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("banned")]
        public int Banned { get; set; }

        [JsonPropertyName("ban_reason")]
        public string BanReason { get; set; }

        [JsonPropertyName("subscriptions")]
        public int Subscriptions { get; set; }

        [JsonPropertyName("favorited")]
        public int Favorited { get; set; }

        [JsonPropertyName("lifetime_subscriptions")]
        public int LifetimeSubscriptions { get; set; }

        [JsonPropertyName("lifetime_favorited")]
        public int LifetimeFavourited { get; set; }

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("tags")]
        public List<TagModel> tags { get; set; }
}

// Source generation stuff for native AOT compatibility
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(PublishedFileDetailsModel))]
internal partial class PublishedFileDetailsModelSourceGenerationContext : JsonSerializerContext
{
    
}