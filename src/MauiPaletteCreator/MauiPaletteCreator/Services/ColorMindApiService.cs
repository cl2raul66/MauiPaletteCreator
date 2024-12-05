using MauiPaletteCreator.Models;
using System.Text.Json;

namespace MauiPaletteCreator.Services;

/// <summary>
/// Service for interacting with the Colormind API
/// </summary>
public class ColorMindApiService
{
    readonly HttpClient _httpClient;
    const string BaseUrl = "http://colormind.io/api/";
    const string ListUrl = "http://colormind.io/list/";

    public ColorMindApiService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Get a random color palette using the default model
    /// </summary>
    /// <returns>A list of color palettes</returns>
    public async Task<ColorPalette> GetRandomPaletteAsync()
    {
        var request = new
        {
            model = "default"
        };

        return await SendPaletteRequestAsync(request);
    }

    /// <summary>
    /// Get a color palette with some input colors
    /// </summary>
    /// <param name="inputColors">List of input colors (use "N" for unknown slots)</param>
    /// <param name="selectedModel">Optional model name (defaults to "default")</param>
    /// <returns>A color palette</returns>
    public async Task<ColorPalette> GetPaletteWithInputAsync(
        List<object> inputColors,
        string selectedModel = "default")
    {
        var request = new
        {
            model = selectedModel,
            input = inputColors
        };

        return await SendPaletteRequestAsync(request);
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

        return models["result"];
    }

    /// <summary>
    /// Internal method to send palette requests to the API
    /// </summary>
    private async Task<ColorPalette> SendPaletteRequestAsync(object requestBody)
    {
        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(
            jsonRequest,
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(BaseUrl, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, List<int[]>>>(responseContent);

        return new ColorPalette
        {
            Colors = result["result"]
        };
    }

    /// <summary>
    /// Convert RGB integer array to System.Drawing.Color
    /// </summary>
    public static System.Drawing.Color RgbToColor(int[] rgb)
    {
        return System.Drawing.Color.FromArgb(rgb[0], rgb[1], rgb[2]);
    }
}
