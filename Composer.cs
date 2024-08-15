using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Knowit.Umbraco.CorruptIndexFixer;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {

#if NET8_0_OR_GREATER
        builder.Services.Configure<UmbracoPipelineOptions>(options =>
            options.AddFilter(
                new UmbracoPipelineFilter("CorruptIndexHandler", prePipeline: app => app.UseMiddleware<IndexExceptionHandlingMiddleware>())));
#else

        builder.Services.Configure<UmbracoPipelineOptions>(options => 
                    options.AddFilter(
                        new UmbracoPipelineFilter("CorruptIndexHandler", 
                        prePipeline: app => app.UseMiddleware<IndexExceptionHandlingMiddleware>(),
                        postPipeline: null,
                        endpointCallback: null
                        )));
#endif
    }
}
