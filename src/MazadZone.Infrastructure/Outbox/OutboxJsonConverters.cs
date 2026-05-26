using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MazadZone.Domain.Orders;
using MazadZone.Domain.ValueObjects;
using MazadZone.Domain.Shared.ValueObjects;

namespace MazadZone.Infrastructure.Outbox;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override void WriteJson(JsonWriter writer, Money? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteStartObject();
        writer.WritePropertyName("Amount");
        writer.WriteValue(value.Amount);
        writer.WritePropertyName("Currency");
        serializer.Serialize(writer, value.Currency);
        writer.WriteEndObject();
    }

    public override Money? ReadJson(JsonReader reader, Type objectType, Money? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var jsonObject = JObject.Load(reader);
        var amount = jsonObject["Amount"]?.Value<decimal>() ?? jsonObject["amount"]?.Value<decimal>() ?? 0m;
        var currencyToken = jsonObject["Currency"] ?? jsonObject["currency"];
        
        Currency? currency = null;
        if (currencyToken != null)
        {
            currency = currencyToken.ToObject<Currency>(serializer);
        }

        if (currency == null)
        {
            throw new JsonSerializationException("Currency is required to deserialize Money.");
        }

        var result = Money.Create(amount, currency);
        if (result.IsFailure)
        {
            throw new JsonSerializationException(result.TopError.Message);
        }

        return result.Value;
    }
}

public class CurrencyJsonConverter : JsonConverter<Currency>
{
    public override void WriteJson(JsonWriter writer, Currency? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteStartObject();
        writer.WritePropertyName("Code");
        writer.WriteValue(value.Code);
        writer.WriteEndObject();
    }

    public override Currency? ReadJson(JsonReader reader, Type objectType, Currency? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        if (reader.TokenType == JsonToken.String)
        {
            var code = reader.Value?.ToString();
            return string.IsNullOrEmpty(code) ? null : Currency.FromCode(code);
        }

        var jsonObject = JObject.Load(reader);
        var codeValue = jsonObject["Code"]?.Value<string>() ?? jsonObject["code"]?.Value<string>();

        if (string.IsNullOrEmpty(codeValue))
        {
            throw new JsonSerializationException("Code is required to deserialize Currency.");
        }

        return Currency.FromCode(codeValue);
    }
}

public class NameJsonConverter : JsonConverter<Name>
{
    public override void WriteJson(JsonWriter writer, Name? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteValue(value.Value);
    }

    public override Name? ReadJson(JsonReader reader, Type objectType, Name? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        string? val = null;
        if (reader.TokenType == JsonToken.String)
        {
            val = reader.Value?.ToString();
        }
        else
        {
            var jsonObject = JObject.Load(reader);
            val = jsonObject["Value"]?.Value<string>() ?? jsonObject["value"]?.Value<string>();
        }

        if (string.IsNullOrEmpty(val)) return null;

        var result = Name.Create(val);
        if (result.IsFailure)
        {
            throw new JsonSerializationException(result.TopError.Message);
        }
        return result.Value;
    }
}

public class ImageJsonConverter : JsonConverter<Image>
{
    public override void WriteJson(JsonWriter writer, Image? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteStartObject();
        writer.WritePropertyName("Path");
        writer.WriteValue(value.Path);
        writer.WritePropertyName("AltText");
        writer.WriteValue(value.AltText);
        writer.WritePropertyName("IsMain");
        writer.WriteValue(value.IsMain);
        writer.WriteEndObject();
    }

    public override Image? ReadJson(JsonReader reader, Type objectType, Image? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var jsonObject = JObject.Load(reader);
        var path = jsonObject["Path"]?.Value<string>() ?? jsonObject["path"]?.Value<string>() ?? string.Empty;
        var altText = jsonObject["AltText"]?.Value<string>() ?? jsonObject["altText"]?.Value<string>();
        var isMain = jsonObject["IsMain"]?.Value<bool>() ?? jsonObject["isMain"]?.Value<bool>() ?? false;

        var result = Image.Create(path, altText, isMain);
        if (result.IsFailure)
        {
            throw new JsonSerializationException(result.TopError.Message);
        }
        return result.Value;
    }
}

public class DescriptionJsonConverter : JsonConverter<Description>
{
    public override void WriteJson(JsonWriter writer, Description? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteValue(value.Value);
    }

    public override Description? ReadJson(JsonReader reader, Type objectType, Description? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        string? val = null;
        if (reader.TokenType == JsonToken.String)
        {
            val = reader.Value?.ToString();
        }
        else
        {
            var jsonObject = JObject.Load(reader);
            val = jsonObject["Value"]?.Value<string>() ?? jsonObject["value"]?.Value<string>();
        }

        if (string.IsNullOrEmpty(val)) return null;

        var result = Description.Create(val);
        if (result.IsFailure)
        {
            throw new JsonSerializationException(result.TopError.Message);
        }
        return result.Value;
    }
}

public class TitleJsonConverter : JsonConverter<Title>
{
    public override void WriteJson(JsonWriter writer, Title? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteValue(value.Value);
    }

    public override Title? ReadJson(JsonReader reader, Type objectType, Title? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        string? val = null;
        if (reader.TokenType == JsonToken.String)
        {
            val = reader.Value?.ToString();
        }
        else
        {
            var jsonObject = JObject.Load(reader);
            val = jsonObject["Value"]?.Value<string>() ?? jsonObject["value"]?.Value<string>();
        }

        if (string.IsNullOrEmpty(val)) return null;

        var result = Title.Create(val);
        if (result.IsFailure)
        {
            throw new JsonSerializationException(result.TopError.Message);
        }
        return result.Value;
    }
}

public class ReasonJsonConverter : JsonConverter<Reason>
{
    public override void WriteJson(JsonWriter writer, Reason? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        writer.WriteValue(value.Text);
    }

    public override Reason? ReadJson(JsonReader reader, Type objectType, Reason? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        string? val = null;
        if (reader.TokenType == JsonToken.String)
        {
            val = reader.Value?.ToString();
        }
        else
        {
            var jsonObject = JObject.Load(reader);
            val = jsonObject["Text"]?.Value<string>() ?? jsonObject["text"]?.Value<string>();
        }

        if (string.IsNullOrEmpty(val)) return null;

        var result = Reason.Create(val);
        if (result.IsFailure)
        {
            throw new JsonSerializationException(result.TopError.Message);
        }
        return result.Value;
    }
}
