using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using ChatGPT.Plugins.Github.Benchmarks.Benchmarks;

namespace ChatGPT.Plugins.Github.Benchmarks;

internal class Program
{
    public static void Main()
    {
        var config = new ManualConfig
        {
            Options = ConfigOptions.DisableOptimizationsValidator
        };

        config.AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
        config.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
        config.AddColumnProvider(
            DefaultColumnProviders.Descriptor,
            DefaultColumnProviders.Job,
            DefaultColumnProviders.Params,
            DefaultColumnProviders.Metrics
        );

        config.AddColumn(
            StatisticColumn.Min,
            StatisticColumn.Median,
            StatisticColumn.P90,
            StatisticColumn.P95,
            StatisticColumn.Max
        );

        BenchmarkRunner.Run<GithubFilesEnumeratorBenchmark>(config);

        Console.ReadKey();
    }
}
