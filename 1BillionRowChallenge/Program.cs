using BenchmarkDotNet.Running;
using FileManager;
using FileManager.Benchmarks;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Run Benchmarks
// BenchmarkRunner.Run<SplitMeasurementLineStrategiesBenchmarks>();
BenchmarkRunner.Run<ReadStrategiesBenchmarks>();

/*// Dependencies
var fileManager = new StreamFileManager();

// Step 1 - Reading input measurement data
// WIP
var watch = System.Diagnostics.Stopwatch.StartNew();
var measurements = fileManager.ReadTextFromFileInCustomStruct("Data/measurements.txt");
watch.Stop();
Console.WriteLine($"Read {measurements.Count} measurements - Finished in {watch.Elapsed.TotalMinutes}m");

// Step 2 - Reduce measurement data into desired output
// TODO

// Step Stage 3 - Save the output to a file on disk
// TODO*/