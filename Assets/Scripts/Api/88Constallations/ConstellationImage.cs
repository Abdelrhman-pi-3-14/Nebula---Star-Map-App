using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationImage
{
    [JsonProperty("color_summary")]
    public List<string> ColorSummary { get; set; }

    [JsonProperty("etag")]
    public string ETag { get; set; }

    [JsonProperty("filename")]
    public string FileName { get; set; }

    [JsonProperty("format")]
    public string Format { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("last_synchronized")]
    public string LastSynchronized { get; set; }

    [JsonProperty("mimetype")]
    public string MimeType { get; set; }

    [JsonProperty("thumbnail")]
    public bool IsThumbnail { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }
}
