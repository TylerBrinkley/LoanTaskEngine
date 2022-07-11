using EnumsNET;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LoanTaskEngine.Tasks;

public sealed class EntityTask : SerializableObject
{
    private string _name;
    private EntityType _entity;
    private List<Condition>? _triggerConditions;
    private List<Condition>? _completionConditions;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Id { get; internal set; }

    public string Name { get => _name; set => _name = Preconditions.NotNullOrEmpty(value); }

    [JsonConverter(typeof(StringEnumConverter))]
    public EntityType Entity { get => _entity; set => _entity = value.Validate(nameof(value)); }

    public List<Condition> TriggerConditions { get => _triggerConditions ??= new(); set => _triggerConditions = value; }

    public List<Condition> CompletionConditions { get => _completionConditions ??= new(); set => _completionConditions = value; }

#pragma warning disable CS8618 // Compiler isn't smart enough to know _name is assigned through the Field property
    public EntityTask(string name, EntityType entity)
#pragma warning restore CS8618
    {
        Name = name;
        Entity = entity;
    }
}