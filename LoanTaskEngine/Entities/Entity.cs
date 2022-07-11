using System.Collections.ObjectModel;
using LoanTaskEngine.Tasks;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoanTaskEngine.Entities;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
public abstract class Entity : SerializableObject
{
    internal readonly Dictionary<string, EntityTaskStatus> _taskStatuses = new(StringComparer.OrdinalIgnoreCase);

    public string Id { get; }

    [JsonIgnore]
    public IReadOnlyDictionary<string, EntityTaskStatus> TaskStatuses { get; }

    public Entity(string id)
    {
        Id = Preconditions.NotNullOrEmpty(id, nameof(id));
        TaskStatuses = new ReadOnlyDictionary<string, EntityTaskStatus>(_taskStatuses);
    }
}