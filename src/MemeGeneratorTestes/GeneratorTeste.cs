using FluentAssertions;
using MemeGenerator;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MemeGeneratorTestes
{
    public class GeneratorTeste
    {
        private IGenerator generator;

        [SetUp]
        public void Setup()
        {
            generator = new Generator();
        }

        [Test]
        public async Task deve_listar_memes_template()
        {
            var memesTemplate = await generator.GetMemesTemplateAsync().ConfigureAwait(false);

            memesTemplate.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task deve_gerar_meme_a_partir_do_template()
        {
            var memesTemplate = await generator.GetMemesTemplateAsync().ConfigureAwait(false);

            var meme = await generator.GetMemeAsync(memesTemplate.Last().Name, "Texto 1", "Texto 2").ConfigureAwait(false);

            meme.Should().NotBeNull();
        }
    }
}