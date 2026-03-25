using System.Text.Json;

namespace $safeprojectname$.Helpers;
public static class Json
{
    public static JsonSerializerOptions options = new()
    {
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
        WriteIndented = true
    };

    public static async Task<T?> DeserializeAsync<T>(string value, JsonSerializerOptions? options = default)
    {
        return await Task.Run<T?>(() =>
        {
            return JsonSerializer.Deserialize<T>(value, options); //JsonConvert.DeserializeObject<T>(value);
        });
        //return await JsonSerializer.DeserializeAsync<T>(value);
    }

    public static async Task<string> SerializeAsync(object value, JsonSerializerOptions? options = default)
    {
        return await Task.Run<string>(() =>
        {
            return JsonSerializer.Serialize(value, options); //JsonConvert.SerializeObject(value);
        });

        //return await JsonSerializer.Serialize(value);
    }

    public static T? DeserializeAnonymousType<T>(string json, T anonymousTypeObject, JsonSerializerOptions? options = default)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }

    public static async Task<T?> DeserializeAnonymousTypeAsync<T>(string json, T anonymousTypeObject, JsonSerializerOptions? options = default)
    {
        // These two options do not work when catching error in the calling method
        //return await DeserializeAsync<T>(json, options);
        //return await Task.Run<T?>(() =>
        //{
        //    return JsonSerializer.Deserialize<T>(json, options);
        //});

        return JsonSerializer.Deserialize<T>(json, options);
    }
}