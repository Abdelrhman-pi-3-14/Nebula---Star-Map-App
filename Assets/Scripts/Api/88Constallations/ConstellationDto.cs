using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ConstelltionDto
{
    [JsonProperty("results")]
    public List<ConstellationData> Results { get; set; }

    [JsonProperty("total_count")]
    public int TotalCount { get; set; }
}
