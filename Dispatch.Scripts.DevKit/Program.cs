// See https://aka.ms/new-console-template for more information
using Dispatch.Scripts;
using Dispatch.Scripts.Abstractions;
using Dispatch.Scripts.DevKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

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

string GetScriptPathFromClass(Type classType)
{
    var path = classType.Namespace!.Replace("Dispatch.Scripts.DevKit.", "").Replace(".", "\\");

    if (!Path.Exists(path))
    {
        throw new Exception($"Could not find path for debug data (Path: {path}). Make sure the namespace is the same as the directory.");
    }

    return path;
}

var script = GetScript(arguments[1]);
if (script is null)
{
    throw new Exception($"Couldn't create instance of type '{arguments[1]}'");
}

var path = GetScriptPathFromClass(script.GetType());
var baseline = arguments.Length == 4
    ? GetScript(arguments[2])
    : null;
var scriptData = arguments.Length == 4
    ? Environment.GetCommandLineArgs()[3]
    : Environment.GetCommandLineArgs()[2];

var iterations = Enumerable.Repeat(true, 1);
var isBenchmarking = iterations.Count() > 1;
var shouldLog = !isBenchmarking;
var logger = shouldLog
    ? loggerFactory.CreateLogger<Program>()
    : (ILogger)NullLogger.Instance;

var forceRerunMapSheet = true;
if (forceRerunMapSheet)
{
    Console.WriteLine("Using 'forceRerunMapSheet', this will impact performance comparison since it doesn't use the 'cached' typed objects and reconstructs them using Reflection.");
}

using var fileStream = File.OpenRead($"{path}\\{scriptData}");
var json = await JsonSerializer.DeserializeAsync<JsonNode>(fileStream);
var scriptDebugWrapper = new ScriptDebugWrapper(json!, logger);
var scriptDataProvider = new MockScriptDataProvider(scriptDebugWrapper, forceRerunMapSheet);

if (script is IOrderUpdateScript orderUpdateScript)
{
    var orderUpdater = new MockOrderUpdater(scriptDebugWrapper, NullLogger.Instance);

    if (isBenchmarking)
    {
        // Run once for warmup
        await new OrderUpdateScriptContainer(orderUpdateScript).OnOrderUpdate(orderUpdater, scriptDataProvider, NullLogger.Instance);
    }

    orderUpdater.UpdateLogger(logger);

    var stopwatch = Stopwatch.StartNew();
    await Parallel.ForEachAsync(iterations, async (_, _) => await new OrderUpdateScriptContainer(orderUpdateScript)
        .OnOrderUpdate(orderUpdater, scriptDataProvider, logger));
    stopwatch.Stop();
    Console.WriteLine($"[{orderUpdateScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds/iterations.Count()}ms)");

    if (baseline is not null && baseline is IOrderUpdateScript baselineScript)
    {
        stopwatch.Restart();
        await Parallel.ForEachAsync(iterations, async (_, _) => await new OrderUpdateScriptContainer(baselineScript)
            .OnOrderUpdate(orderUpdater, scriptDataProvider, logger));
        stopwatch.Stop();
        Console.WriteLine($"[{baselineScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds/iterations.Count()}ms)");
    }
}

if (script is IExtraFeeScript extraFeeScript)
{
    var orderScriptInfo = scriptDebugWrapper.GetOrderScriptInfo()!;

    if (isBenchmarking)
    {
        // Run once for warmup
        await new ExtraFeeScriptContainer(extraFeeScript).GetExtraFeePriceInfo(orderScriptInfo, scriptDataProvider, NullLogger.Instance);
    }

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
    stopwatch.Stop();
    Console.WriteLine($"[{extraFeeScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds / iterations.Count()}ms)");

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
        stopwatch.Stop();
        Console.WriteLine($"[{baselineScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds / iterations.Count()}ms)");
    }
}

if (script is IOrderRuleScript orderRuleScript)
{
    var orderReader = new MockOrderUpdater(scriptDebugWrapper, NullLogger.Instance).OrderReader;

    if (isBenchmarking)
    {
        // Run once for warmup
        await new OrderRuleScriptContainer(orderRuleScript).EvaluateRule(orderReader, scriptDataProvider, NullLogger.Instance);
    }

    var stopwatch = Stopwatch.StartNew();
    await Parallel.ForEachAsync(iterations, async (_, _) =>
    {
        var result = await new OrderRuleScriptContainer(orderRuleScript)
            .EvaluateRule(orderReader, scriptDataProvider, logger);

        logger.LogInformation($"Result is {result}");
    });
    stopwatch.Stop();
    Console.WriteLine($"[{orderRuleScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds / iterations.Count()}ms)");

    if (baseline is not null && baseline is IOrderRuleScript baselineScript)
    {
        stopwatch.Restart();
        await Parallel.ForEachAsync(iterations, async (_, _) =>
        {
            var result = await new OrderRuleScriptContainer(baselineScript)
                .EvaluateRule(orderReader, scriptDataProvider, logger);

            logger.LogInformation($"Result is {result}");
        });
        stopwatch.Stop();
        Console.WriteLine($"[{baselineScript.GetType().Name}] Executed {iterations.Count()} iterations in {stopwatch.Elapsed.TotalMilliseconds}ms (Avg: {stopwatch.Elapsed.TotalMilliseconds / iterations.Count()}ms)");
    }
}

Console.WriteLine("Script run completed");
Console.ReadLine();