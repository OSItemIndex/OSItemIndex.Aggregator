using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using OSItemIndex.Data;
using System.Text.Json;
using OSItemIndex.Data.Extensions;
using Serilog;

namespace OSItemIndex.Updater.Services
{
    public class RealtimePriceClient : IRealtimePriceClient
    {
        private readonly HttpClient _client;

        public RealtimePriceClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.ObserverUserAgent);
            _client = client;
        }

        public async Task<IEnumerable<T>?> GetPricesAsync<T>(string uri) where T: PriceEntity
        {
            using var response = await _client.GetAsync(Endpoints.Realtime.Api + uri);
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadFromJsonAnonymousAsync(new
                    {
                        data = new Dictionary<string, T>(),
                        timestamp = (long?) null
                    });

                    if (json != null)
                    {
                        foreach (var (key, value) in json.data)
                        {
                            value.Id = int.Parse(key);
                            if (json.timestamp != null && value is RealtimeItemPrice.AveragePrice price)
                            {
                                price.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long) json.timestamp).UtcDateTime;
                            }
                        }
                        return json.data.Values;
                    }
                }
                catch (HttpRequestException e)
                {
                    Log.Error(e, "Response status not OK");
                }
                catch (JsonException e)
                {
                    Log.Error(e, "JSON exception caught");
                }
            }
            return null;
        }
    }
}
