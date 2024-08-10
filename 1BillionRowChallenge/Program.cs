using DataProcessor.Processors;
using DataProcessor.Processors.Bytes;
using DataProcessor.Processors.Naive;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Constants
const string fullDatasetFilepath = "Data/measurements.txt";
const string subsetDatasetFilepath = "Data/measurements_subset.txt";

// Dependencies
// If any?

// Processor Initialisations
// NAIVE READER
// var measurements = new Naive(fullDatasetFilepath);
// var measurements = new NaiveSpans(fullDatasetFilepath);
// var measurements = new NaiveSpansLemire(fullDatasetFilepath);
// var measurements = new NaiveSpansLemireMapMarshall(fullDatasetFilepath);

// BYTES READER
// var measurements = new BytesSpansLemire(fullDatasetFilepath);
var measurements = new BufferedBytesSpansLemire(fullDatasetFilepath);
// var measurements = new BufferedBytesSpansLemireMapMarshall(fullDatasetFilepath);

// Run the processing
var watch = System.Diagnostics.Stopwatch.StartNew();
measurements.Process();
watch.Stop();
Console.WriteLine($"Read {measurements.MeasurementsMap.Count} measurements - Finished in {watch.Elapsed.TotalMinutes}m");