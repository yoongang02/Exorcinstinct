using System;
using Newtonsoft.Json;

[Serializable]
public class HintRuleResponse
{
    [JsonProperty("version")] public string Version;
    [JsonProperty("category")] public string Category; // "name" | "feature" | "location" | "cause" | "unsure"
    [JsonProperty("rule_id")] public string RuleId;
    [JsonProperty("confidence")] public float? Confidence;
    [JsonProperty("reason")] public string Reason;
    [JsonProperty("params")] public RuleParams Params;
}

[Serializable]
public class RuleParams
{
    // name
    [JsonProperty("name")] public string Name;
    [JsonProperty("surname")] public string Surname;
    [JsonProperty("last_char")] public string LastChar;

    // feature
    [JsonProperty("category")] public string FeatureCategory;
    [JsonProperty("correct_feature")] public string CorrectFeature;
    [JsonProperty("correct_features")] public string[] CorrectFeatures;
    [JsonProperty("repeat_features")] public string[] RepeatFeatures;

    // location
    [JsonProperty("floor")] public string Floor;
    [JsonProperty("wing")] public string Wing;
    [JsonProperty("room")] public string Room;
    [JsonProperty("random_pool")] public string[] RandomPool;

    // cause
    [JsonProperty("onomatopoeia")] public string Onomatopoeia;
}
