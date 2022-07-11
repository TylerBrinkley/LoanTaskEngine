using Newtonsoft.Json;

namespace LoanTaskEngine.Utilities;

/// <summary>
/// This converter is to be used to ensure the default serialization and deserialization behavior.
/// </summary>
public sealed class DefaultConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => true;

    public override bool CanRead => false;

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => throw new NotSupportedException();

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotSupportedException();
}