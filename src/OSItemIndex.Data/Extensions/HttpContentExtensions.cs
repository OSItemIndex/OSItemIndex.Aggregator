using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OSItemIndex.Data.Extensions
{
    public static class HttpContentExtensions
    {
        public static Task<T?> ReadFromJsonAnonymousAsync<T>(this HttpContent content, T anonymousTypeObject,
                                                             JsonSerializerOptions? options = null,
                                                             CancellationToken cancellationToken = default) =>
            content.ReadFromJsonAsync<T>(options, cancellationToken);
    }
}
