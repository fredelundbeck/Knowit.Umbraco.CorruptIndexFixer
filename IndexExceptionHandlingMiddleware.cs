using Lucene.Net.Index;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Examine;

namespace Knowit.Umbraco.CorruptIndexFixer;

public class IndexExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static DateTime? _lastRebuildTime;
    private static readonly TimeSpan MinRebuildInterval = TimeSpan.FromMinutes(10);

    public IndexExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CorruptIndexException)
        {
            await TryRebuildExamineIndexes(context);
        }
        catch (FileNotFoundException e)
        {
            if (e.Message.Contains("ExamineIndexes", StringComparison.OrdinalIgnoreCase))
            {
                await TryRebuildExamineIndexes(context);
            }
        }
    }

    private async Task TryRebuildExamineIndexes(HttpContext context)
    {
        if (_lastRebuildTime.HasValue && DateTime.Now - _lastRebuildTime < MinRebuildInterval)
        {
            _logger.LogInformation("[CorruptIndexFixer] Rebuild attempt skipped - too soon since last rebuild.");
            return;
        }

        if (_semaphore.CurrentCount is 0)
        {
            _logger.LogInformation("[CorruptIndexFixer] Rebuilding Examine indexes is already in progress");

            if (_lastRebuildTime.HasValue)
            {
                var timeSince = DateTime.Now - _lastRebuildTime.Value;
                _logger.LogInformation("[CorruptIndexFixer] Time since last rebuild: {TimeSince}", timeSince);
            }

            return;
        }
        
        await _semaphore.WaitAsync();

        try
        {
            var indexRebuilder = context.RequestServices.GetRequiredService<IIndexRebuilder>();
            if (indexRebuilder is null)
            {
                _logger.LogError("[CorruptIndexFixer] Could not get IIndexRebuilder service");
                return;
            }

            _logger.LogInformation("[CorruptIndexFixer] Rebuilding Examine indexes");

            indexRebuilder.RebuildIndexes(false);
            _lastRebuildTime = DateTime.Now;

            _logger.LogInformation("[CorruptIndexFixer] Rebuilding Examine indexes completed");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}