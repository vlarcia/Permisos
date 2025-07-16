using Business.Interfaces;

using RazorLight;
using System.Reflection;
using System.Threading.Tasks;

namespace Business.Implementacion
{
    public class PlantillaService : IPlantillaService
    {
        private readonly RazorLightEngine _engine;

        public PlantillaService()
        {


            var assembly = Assembly.GetExecutingAssembly(); // o typeof(PlantillaService).Assembly

            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(assembly)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderizarVistaAsync<T>(string viewName, T modelo)
        {

            string templateKey = $"Business.Plantillas.{viewName}";  // Sin extensión .cshtml
            string resultado = await _engine.CompileRenderAsync(templateKey, modelo);
            return resultado;
        }
    }
}