using DataProcessor.Processors;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Constants
const string fullDatasetFilepath = "Data/measurements.txt";
const string subsetDatasetFilepath = "Data/measurements_subset.txt";

// Dependencies
// If any?

// Processor Initialisations
// var measurements = new Naive(fullDatasetFilepath);
// var measurements = new NaiveSpans(fullDatasetFilepath);
var measurements = new NaiveSpansLemire(fullDatasetFilepath);

// Run the processing
var watch = System.Diagnostics.Stopwatch.StartNew();
measurements.Process();
watch.Stop();
Console.WriteLine($"Read {measurements.MeasurementsMap.Count} measurements - Finished in {watch.Elapsed.TotalMinutes}m");