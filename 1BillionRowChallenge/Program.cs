using BenchmarkDotNet.Running;
using DataProcessor.Benchmarks;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Run Benchmarks
BenchmarkRunner.Run<SplitMeasurementLineStrategiesBenchmarks>();
// BenchmarkRunner.Run<ReadStrategiesBenchmarks>();

/*// Dependencies
var fileManager = new StreamFileManager();

// WIP
var watch = System.Diagnostics.Stopwatch.StartNew();
var measurements = fileManager.ReadTextFromFileInCustomStruct("Data/measurements.txt");
watch.Stop();
Console.WriteLine($"Read {measurements.Count} measurements - Finished in {watch.Elapsed.TotalMinutes}m");
*/