using System.Text;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoanTaskEngine;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public abstract class SerializableObject
{
    public override string ToString()
    {
        StringBuilder sb = new(256);
        StringWriter sw = new(sb);
        using (JsonTextWriter jsonWriter = new(sw))
        {
            JsonHelper.DefaultSerializer.Serialize(jsonWriter, this);
        }

        return sw.ToString();
    }
}