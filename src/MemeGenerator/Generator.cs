using MemeGenerator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MemeGenerator
{
    [Serializable]
    public class Generator : IGenerator
    {
        private const string urlGenerator = "https://memegen.link/api";
        private static HttpClient httpClient;
        private static IList<MemeTemplate> templates;

        static Generator()
        {
            templates = new List<MemeTemplate>();
            httpClient = new HttpClient();
        }

        public async Task<IList<MemeTemplate>> GetMemesTemplateAsync()
        {
            if (templates.Count > 0)
            {
                return templates;
            }

            var result = await httpClient.GetStringAsync(urlGenerator + "/templates/").ConfigureAwait(false);
            DeserializarTemplates(result);

            return await Task.FromResult(templates).ConfigureAwait(false);
        }

        private void DeserializarTemplates(string result)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            foreach (var kv in dict)
            {
                templates.Add(new MemeTemplate { Name = kv.Key, Value = kv.Value });
            }
        }

        public async Task<Meme> GetMemeAsync(string templateNome, string texto1, string texto2)
        {
            var template = templates.First(x => x.Name == templateNome);

            var result = await httpClient.GetStringAsync($"{template.Value}/{texto1}/{texto2}").ConfigureAwait(false);

            var meme = JsonConvert.DeserializeObject<Meme>(result);

            return await Task.FromResult(meme).ConfigureAwait(false);
        }
    }
}