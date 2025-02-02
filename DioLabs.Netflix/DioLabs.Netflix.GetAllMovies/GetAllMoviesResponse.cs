using Newtonsoft.Json;

namespace DioLabs.Netflix.GetAllMovies;

internal class GetAllMoviesResponse
{
    [JsonProperty("id")]
    public string ID { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("year")]
    public string Year { get; set; }

    [JsonProperty("video")]
    public string Video { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }
}
