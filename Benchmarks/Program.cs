using BenchmarkDotNet.Running;
using Benchmarks.Suites;

// Specific case benchmarking
// BenchmarkRunner.Run<SplitMeasurementLineStrategiesBenchmarks>();
// BenchmarkRunner.Run<ReadStrategiesBenchmarks>();
// BenchmarkRunner.Run<MapTryGetOperationsStrategiesBenchmarks>();
// BenchmarkRunner.Run<MapHashingOperationsStrategiesBenchmarks>();
// BenchmarkRunner.Run<FloatParsingStrategiesBenchmarks>();

// Overall benchmarks
BenchmarkRunner.Run<OverallStrategyBenchmarks>();