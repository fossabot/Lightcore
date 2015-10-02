using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lightcore.Kernel.Data;
using Lightcore.Server.Models;
using Newtonsoft.Json;

namespace Lightcore.Server
{
    public class LightcoreApiItemProvider : IItemProvider, IDisposable
    {
        private readonly HttpClient _client;

        public LightcoreApiItemProvider()
        {
            _client = new HttpClient();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<Item> GetItemAsync(string pathOrId, Language language)
        {
            string query;

            if (pathOrId.Equals("/") || string.IsNullOrEmpty(pathOrId))
            {
                query = "/sitecore/content/Home";
            }
            else if (pathOrId.Contains("/"))
            {
                query = "/sitecore/content/Home" + pathOrId.ToLowerInvariant().Replace("/sitecore/content/home", "/");
            }
            else
            {
                query = pathOrId;
            }

            var getWatch = Stopwatch.StartNew();
            var url = "http://lightcore-testserver.ad.codehouse.com/-/lightcore/item/" + query + "?sc_database=web&sc_lang=" + language.Name + "&sc_device=default";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                getWatch.Stop();

                var parseWatch = Stopwatch.StartNew();
                var apiResponse = JsonConvert.DeserializeObject<ServerResponseModel>(content);
                var item = Map(apiResponse.Item, apiResponse.Fields, apiResponse.Presentation, language);

                item.Children = apiResponse.Children.Select(child => Map(child.Item, child.Fields, child.Presentation, language));

                parseWatch.Stop();

                item.Trace = $"Loaded in {getWatch.ElapsedMilliseconds} ms, mapped in {parseWatch.ElapsedMilliseconds} ms";

                return item;
            }

            return null;
        }

        private Item Map(ItemModel apiItem, IEnumerable<FieldModel> apiFields, PresentationModel apiPresentation, Language language)
        {
            var item = new Item
            {
                Language = language,
                Id = apiItem.Id,
                Key = apiItem.Name.ToLowerInvariant(),
                Name = apiItem.Name,
                Path = apiItem.FullPath,
                Url = "/" + language.Name.ToLowerInvariant() + apiItem.FullPath.ToLowerInvariant().Replace("/sitecore/content/home", ""),
                Fields = apiFields.Select(f => new Field
                {
                    Key = f.Key,
                    Type = f.Type,
                    Value = f.Value,
                    Id = f.Id
                })
            };

            if (apiPresentation != null)
            {
                item.Layout = apiPresentation.Layout.Path;
                item.Renderings = apiPresentation.Renderings.Select(r => new Rendering(r.Placeholder, r.Controller)).ToList();
            }

            return item;
        }
    }
}