using EnumsNET;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LoanTaskEngine.Tasks;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public sealed class Condition
{
    private string _field;
    private Comparator _comparator;

    public string Field { get => _field; set => _field = Preconditions.NotNullOrEmpty(value); }

    [JsonConverter(typeof(StringEnumConverter), true)]
    public Comparator Comparator { get => _comparator; set => _comparator = value.Validate(nameof(value)); }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public object? Value { get; set; }

#pragma warning disable CS8618 // Compiler isn't smart enough to know _field is assigned through the Field property
    public Condition(string field, Comparator comparator, object? value = null)
#pragma warning restore CS8618
    {
        Field = field;
        Comparator = comparator;
        Value = value;
    }

    public bool Evaluate(Entity entity)
    {
        Preconditions.NotNull(entity);

        var property = (JsonHelper.DefaultSerializer.ContractResolver.ResolveContract(entity.GetType()) as JsonObjectContract)!.Properties.GetClosestMatchProperty(Field);
        if (property is null)
        {
            throw new InvalidOperationException($"Could not find condition field '{Field}' for entity");
        }
        var value = property.ValueProvider!.GetValue(entity);

        return Comparator switch
        {
            Comparator.Exists => value != null && (value is not string str || str.Length > 0),
            Comparator.Equals => value is string || value is null || Value is null
                ? string.Equals(Value?.ToString() ?? string.Empty, value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                : Convert.ToDecimal(Value) == Convert.ToDecimal(value), // Assumes value is a number if it's not a string or null
            _ => throw new NotSupportedException($"Comparator of value {Comparator} is not supported") // Should never hit this since Comparator is already validated
        };
    }
}