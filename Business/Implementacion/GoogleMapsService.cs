using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class GoogleMapsService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public GoogleMapsService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<string> GetMapImageUrl(string latitud, string longitud,
     int zoom = 15, string size = "700x600", string markerColor = "red",
     string mapType = "hybrid")
    {
        if (string.IsNullOrEmpty(latitud) || string.IsNullOrEmpty(longitud))
            return null;

        var apiKey = _config["GoogleMaps:ApiKey"];
        var baseUrl = _config["GoogleMaps:StaticMapUrl"];

        return $"{baseUrl}?center={latitud},{longitud}" +
               $"&zoom={zoom}&size={size}" +
               $"&maptype={mapType}" +
               $"&markers=color:{markerColor}%7C{latitud},{longitud}" +
               $"&key={apiKey}";
    }

    public async Task<byte[]> GetMapImageBytes(string latitud, string longitud)
    {
        var url = await GetMapImageUrl(latitud, longitud);
        if (url == null) return null;

        try
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            return null;
        }
    }
}