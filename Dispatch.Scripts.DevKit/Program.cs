// See https://aka.ms/new-console-template for more information
using Dispatch.Scripts;
using Dispatch.Scripts.Abstractions;
using Dispatch.Scripts.DevKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Information)
    .AddConsole());

var arguments = Environment.GetCommandLineArgs();
if (arguments.Length < 3)
{
    throw new Exception("Invalid number of arguments, we expect 2 arguments or more, first one being the class name of the script to execute, optionally the second one can be the class name of the baseline script and the last one being the json data file");
}

object? GetScript(string scriptName)
{
    if (scriptName is null)
    {
        return null;
    }

    var scriptType = typeof(Program).Assembly.GetExportedTypes().FirstOrDefault(x => x.Name == scriptName);
    if (scriptType is null)
    {
        throw new Exception($"Couldn't find type '{scriptName}'");
    }

    return Activator.CreateInstance(scriptType);
}

var script = GetScript(arguments[1]);
var folder = script.GetType().Namespace.Replace("Dispatch.Scripts.DevKit.", "").Replace(".", "\\");
var baseline = arguments.Length == 4
    ? GetScript(arguments[2])
    : null;
var scriptData = arguments.Length == 4
    ? Environment.GetCommandLineArgs()[3]
    : Environment.GetCommandLineArgs()[2];

var iterations = Enumerable.Repeat(true, 100);
var shouldLog = iterations.Count() == 1;
var logger = shouldLog
    ? loggerFactory.CreateLogger<Program>()
    : (ILogger)NullLogger.Instance;

var forceRerunMapSheet = false;
if (forceRerunMapSheet)
{
    Console.WriteLine("Using 'forceRerunMapSheet', this will impact performance comparison since it doesn't use the 'cached' typed objects and reconstructs them using Reflection.");
}

if (script is IOrderUpdateScript orderUpdateScript)
{
    using var fileStream = File.OpenRead($"{folder}\\{scriptData}");
    var json = await JsonSerializer.DeserializeAsync<JsonNode>(fileStream);
    var scriptDebugWrapper = new ScriptDebugWrapper(json!, logger);
    var scriptDataProvider = new MockScriptDataProvider(scriptDebugWrapper, forceRerunMapSheet);

    // Run once for warmup
    await new OrderUpdateScriptContainer(orderUpdateScript).OnOrderUpdate(new MockOrderUpdater(scriptDebugWrapper, logger), scriptDataProvider, NullLogger.Instance);

    var stopwatch = Stopwatch.StartNew();
    await Parallel.ForEachAsync(iterations, async (_, _) => await new OrderUpdateScriptContainer(orderUpdateScript)
        .OnOrderUpdate(new MockOrderUpdater(scriptDebugWrapper, logger), scriptDataProvider, logger));
    Console.WriteLine($"[{orderUpdateScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds/iterations.Count()}ms)");

    if (baseline is not null && baseline is IOrderUpdateScript baselineScript)
    {
        stopwatch.Restart();
        await Parallel.ForEachAsync(iterations, async (_, _) => await new OrderUpdateScriptContainer(baselineScript)
            .OnOrderUpdate(new MockOrderUpdater(scriptDebugWrapper, logger), scriptDataProvider, logger));
        Console.WriteLine($"[{baselineScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds/iterations.Count()}ms)");
    }
}

if (script is IExtraFeeScript extraFeeScript)
{
    using var fileStream = File.OpenRead($"{folder}\\{scriptData}");
    var json = await JsonSerializer.DeserializeAsync<JsonNode>(fileStream);
    var scriptDebugWrapper = new ScriptDebugWrapper(json!, logger);
    var scriptDataProvider = new MockScriptDataProvider(scriptDebugWrapper, forceRerunMapSheet);
    var orderScriptInfo = scriptDebugWrapper.GetOrderScriptInfo()!;

    // Run once for warmup
    await new ExtraFeeScriptContainer(extraFeeScript).GetExtraFeePriceInfo(orderScriptInfo, scriptDataProvider, NullLogger.Instance);

    var stopwatch = Stopwatch.StartNew();
    await Parallel.ForEachAsync(iterations, async (_, _) =>
    {
        var result = await new ExtraFeeScriptContainer(extraFeeScript)
            .GetExtraFeePriceInfo(orderScriptInfo, scriptDataProvider, logger);

        if (result is not null)
        {
            logger.LogInformation($"Result: Qty: {result.Quantity} UnitPrice: {result.UnitPrice}");
        }
        else
        {
            logger.LogInformation($"Result is null");
        }
    });
    Console.WriteLine($"[{extraFeeScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms");

    if (baseline is not null && baseline is IExtraFeeScript baselineScript)
    {
        stopwatch.Restart();
        await Parallel.ForEachAsync(iterations, async (_, _) =>
        {
            var result = await new ExtraFeeScriptContainer(baselineScript)
                .GetExtraFeePriceInfo(orderScriptInfo, scriptDataProvider, logger);

            if (result is not null)
            {
                logger.LogInformation($"Result: Qty: {result.Quantity} UnitPrice: {result.UnitPrice}");
            }
            else
            {
                logger.LogInformation($"Result is null");
            }
        });
        Console.WriteLine($"[{baselineScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms");
    }
}

Console.WriteLine("Script run completed");
Console.ReadLine();