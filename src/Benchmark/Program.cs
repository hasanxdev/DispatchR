using Benchmark;
using Benchmark.Notification;
using Benchmark.SendRequest;
using Benchmark.StreamRequest;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromTypes([
    typeof(MediatRVsDispatchBenchmark),
    typeof(MediatRVsDispatchWithPipelineRBenchmark),
    typeof(StreamMediatRVsDispatchBenchmark),
    typeof(StreamMediatRVsDispatchWithPipelineRBenchmark),
    typeof(NotificationBenchmarks),
]).Run(args, ManualConfig.Create(DefaultConfig.Instance)
    .AddColumn(new OperationsColumn())
);
    
namespace Benchmark
{
    public class OperationsColumn : IColumn
    {
        public string Id => nameof(OperationsColumn);
        public string ColumnName => "OpsCount";
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Custom;
        public int PriorityInCategory => -10;
        public bool IsNumeric => true;
        public UnitType UnitType => UnitType.Dimensionless;
        public string Legend => "Number of operations per invoke";

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            return benchmarkCase.Descriptor.WorkloadMethod
                .GetCustomAttributes(typeof(BenchmarkAttribute), false)
                .Cast<BenchmarkAttribute>()
                .FirstOrDefault()?.OperationsPerInvoke.ToString() ?? "1";
        }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
            => GetValue(summary, benchmarkCase);

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
        {
            return true;
        }

        public bool IsAvailable(Summary summary) => true;
        public bool IsDefault(Summary summary) => false;
    }
}