using System;
using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class PredictionDto
{
    [JsonRequired]
    public int Index { get; set; }
    
    [JsonRequired]
    public string Status { get; set; }
    
    [JsonRequired]
    public DateTime CreatedAt { get; set; }
    
    [JsonRequired]
    public dynamic Result { get; set; }
}
