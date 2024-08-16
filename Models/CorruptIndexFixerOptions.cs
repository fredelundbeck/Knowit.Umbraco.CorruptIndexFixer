using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Knowit.Umbraco.CorruptIndexFixer.Models;

public class CorruptIndexFixerOptionsSetup : IConfigureOptions<CorruptIndexFixerOptions>
{
    private const string SectionName = "CorruptIndexFixer";
    private readonly IConfiguration _configuration;

    public CorruptIndexFixerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(CorruptIndexFixerOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}

public class CorruptIndexFixerOptions
{
    public TimeSpan MinimumRebuildInterval { get; set; } = TimeSpan.FromMinutes(10);
    public bool LogToUmbraco { get; set; } = true;
}
