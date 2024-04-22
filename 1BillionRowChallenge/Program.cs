using BenchmarkDotNet.Running;
using DataProcessor.Benchmarks;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Run benchmarks
// BenchmarkRunner.Run<SplitMeasurementLineStrategiesBenchmarks>();
BenchmarkRunner.Run<ReadStrategiesBenchmarks>();
// BenchmarkRunner.Run<MapOperationsStrategiesBenchmarks>();
// BenchmarkRunner.Run<FloatParsingStrategiesBenchmarks>();

/*// Dependencies
var fileManager = new StreamFileManager();

// Run actual solution code on full dataset
var watch = System.Diagnostics.Stopwatch.StartNew();
var measurements = fileManager.ReadTextFromFileInCustomStruct("Data/measurements.txt");
watch.Stop();
Console.WriteLine($"Read {measurements.Count} measurements - Finished in {watch.Elapsed.TotalMinutes}m");
*/