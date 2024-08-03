using DataProcessor;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Dependencies
var fileManager = new StreamFileManager();

// Run actual solution code on full dataset
var watch = System.Diagnostics.Stopwatch.StartNew();
var measurements = fileManager.ReadTextFromFileInCustomStruct("Data/measurements.txt");
watch.Stop();
Console.WriteLine($"Read {measurements.Count} measurements - Finished in {watch.Elapsed.TotalMinutes}m");   