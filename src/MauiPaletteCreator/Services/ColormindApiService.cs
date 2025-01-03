using System.Text.Json;

namespace MauiPaletteCreator.Services;

public interface IColormindApiService
{
    Task<string[]> GetAvailableModelsAsync();
    Task<Color[]> GetPaletteWithInputAsync(Color?[] inputColors, string selectedModel = "default");
    Task<Color[]> GetPaletteAsync(string selectedModel = "default");
}

public class ColormindApiService : IColormindApiService
{
    readonly HttpClient _httpClient;
    const string BaseUrl = "http://colormind.io/api/";
    const string ListUrl = "http://colormind.io/list/";

    public ColormindApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<Color[]> GetPaletteAsync(string selectedModel = "default")
    {
        var request = new
        {
            model = selectedModel
        };

        var result = await SendPaletteRequestAsync(request);

        return result;
    }

    public async Task<Color[]> GetPaletteWithInputAsync(
        Color?[] inputColors,
        string selectedModel = "default")
    {
        var sendColors = ToColormind(inputColors);

        var request = new
        {
            model = selectedModel,
            input = sendColors
        };

        var result = await SendPaletteRequestAsync(request);

        return result;
    }

    public async Task<string[]> GetAvailableModelsAsync()
    {
        var response = await _httpClient.GetAsync(ListUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var models = JsonSerializer.Deserialize<Dictionary<string, string[]>>(content);

        return models?["result"] ?? [];
    }

    async Task<Color[]> SendPaletteRequestAsync(object requestBody)
    {
        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(responseContent))
            {
                if (doc.RootElement.TryGetProperty("result", out JsonElement resultElement))
                {
                    if (resultElement.ValueKind is JsonValueKind.Array)
                    {
                        var colors = resultElement.EnumerateArray()
                            .Select(colorArray =>
                            {
                                var rgbValues = colorArray.EnumerateArray()
                                    .Select(x => x.GetInt32())
                                    .ToArray();

                                return ToColor(rgbValues);
                            }).ToArray();

                        return colors;
                    }
                }
            }

            return [];
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
        catch (JsonException e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }

    Color ToColor(int[] rgb)
    {
        return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
    }

    object[] ToColormind(Color?[] colors)
    {
        return colors.Select(x =>
        {
            if (x is null)
            {
                return (object)"N";
            }
            x.ToRgb(out byte r, out byte g, out byte b);
            return (object)new[] { (int)r, (int)g, (int)b };
        }).ToArray();
    }
}
