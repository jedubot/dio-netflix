using Newtonsoft.Json;

namespace DioLabs.Netflix.DataAdd;

internal class CreateMovieRequest
{
    [JsonProperty("id")]
    public string ID => Guid.NewGuid().ToString();

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("year")]
    public string Year { get; set; }

    [JsonProperty("video")]
    public string Video { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }
}
