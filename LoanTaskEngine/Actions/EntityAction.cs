using EnumsNET;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace LoanTaskEngine.Actions;

[JsonConverter(typeof(EntityActionConverter))]
public abstract class EntityAction : SerializableObject
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public ActionName Action { get; }

    public EntityAction()
    {
        // Uses convention based approach to matching action type to enum value
        var action = GetType().Name[..^"Action".Length];
        Action = Enums.Parse<ActionName>(action);
    }

    public abstract Entity Execute(ILoanRepository loanRepository);
}

public sealed class EntityActionConverter : JsonConverter<EntityAction>
{
    public override EntityAction? ReadJson(JsonReader reader, Type objectType, EntityAction? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Extract json
        var jObject = serializer.Deserialize<JObject>(reader);
        if (jObject is null)
        {
            return null;
        }
        var actionName = jObject["action"];
        if (actionName is null)
        {
            throw new InvalidOperationException("action must be supplied");
        }

        // Find matching type for action name found in json based on naming convention
        var actionType = Type.GetType($"LoanTaskEngine.Actions.{actionName}Action", throwOnError: false, ignoreCase: true);
        if (actionType is null)
        {
            throw new NotSupportedException($"action of '{actionName}' is not supported");
        }

        // Deserialize json to correct action type
        return (EntityAction)jObject.ToObject(actionType, serializer)!;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, EntityAction? value, JsonSerializer serializer) => throw new NotSupportedException("Only supports deserialization. Serialization should be default.");
}