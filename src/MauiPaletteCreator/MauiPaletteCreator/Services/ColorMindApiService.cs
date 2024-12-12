using System.Text.Json;

namespace MauiPaletteCreator.Services;

public interface IColormindApiService
{
    Task<string[]> GetAvailableModelsAsync();
    Task<Color[]> GetPaletteWithInputAsync(Color?[] inputColors, string selectedModel = "default");
    Task<Color[]> GetRandomPaletteAsync(string selectedModel = "default");
}


/// <summary>
/// Service for interacting with the Colormind API
/// </summary>
public class ColormindApiService : IColormindApiService
{
    readonly HttpClient _httpClient;
    const string BaseUrl = "http://colormind.io/api/";
    const string ListUrl = "http://colormind.io/list/";

    public ColormindApiService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Get a random color palette using the default model
    /// </summary>
    /// <returns>A list of color palettes</returns>
    public async Task<Color[]> GetRandomPaletteAsync(string selectedModel = "default")
    {
        var request = new
        {
            model = selectedModel
        };

        var result = await SendPaletteRequestAsync(request);

        return [.. result];
    }

    /// <summary>
    /// Get a color palette with some input colors
    /// </summary>
    /// <param name="inputColors">List of input colors (use "N" for unknown slots)</param>
    /// <param name="selectedModel">Optional model name (defaults to "default")</param>
    /// <returns>A color palette</returns>
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

        return [.. result];
    }

    /// <summary>
    /// Get the array of currently available models
    /// </summary>
    /// <returns>Array of model names</returns>
    public async Task<string[]> GetAvailableModelsAsync()
    {
        var response = await _httpClient.GetAsync(ListUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var models = JsonSerializer.Deserialize<Dictionary<string, string[]>>(content);

        return models?["result"] ?? [];
    }

    /// <summary>
    /// Internal method to send palette requests to the API
    /// </summary>
    async Task<IEnumerable<Color>> SendPaletteRequestAsync(object requestBody)
    {
        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<int[][]>(responseContent);

            if (result is null || result.Length == 0)
            {
                return [];
            }

            var colors = from x in result select RgbToColor(x);

            return colors;
        }
        catch (HttpRequestException e)
        {
            return [];
        }
        catch (JsonException e)
        {
            return [];
        }
    }

    Color RgbToColor(int[] rgb)
    {
        return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
    }

    object[] ToColormind(Color?[] colors)
    {
        return colors.Select(x =>
        {
            if (x is null)
            {
                return "N";
            }
            x.ToRgb(out byte r, out byte g, out byte b);
            return (object)new[] { r, g, b };
        }).ToArray();

        //return $"[{string.Join(",", colors.Select(x =>
        //{
        //    if (x is null)
        //    {
        //        return "\"N\"";
        //    }
        //    x.ToRgb(out byte r, out byte g, out byte b);
        //    return $"[{r},{g},{b}]";
        //}))}]";
    }
}
