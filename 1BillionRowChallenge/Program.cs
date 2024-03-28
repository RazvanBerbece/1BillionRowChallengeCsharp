using FileManager;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Dependencies
var fileManager = new StreamFileManager();

// Step 1 - Reading input measurement data
var watch = System.Diagnostics.Stopwatch.StartNew();
var measurements = fileManager.ReadTextFromFile("Data/measurements.txt");
watch.Stop();
Console.WriteLine($"Read {measurements.Count} measurements - Finished in {watch.Elapsed.TotalSeconds}s");

// Step 2 - Reduce measurement data into desired output
// TODO

// Step Stage 3 - Save the output to a file on disk
// TODO