using Lightcore.Configuration;
using Lightcore.Kernel.Data;
using Lightcore.Kernel.Data.Providers;
using Microsoft.AspNet.Http;

namespace Lightcore.Kernel.Pipelines.Startup
{
    public class StartupArgs : PipelineArgs
    {
        public StartupArgs(HttpContext httpContext, IItemProvider itemProvider, LightcoreOptions options)
        {
            HttpContext = httpContext;
            Options = options;
            ItemProvider = itemProvider;
        }

        public HttpContext HttpContext { get; }

        public LightcoreOptions Options { get; }

        public IItemProvider ItemProvider { get; }
    }
}