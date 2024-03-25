using FileManager;

Console.WriteLine("The 1 Billion Row Challenge - C# Edition");

// Stage 1 - Reading input measurement data
var fileManager = new NaiveFileManager();
var measurements = fileManager.ReadFromFile("../Data/measurements.txt");
Console.WriteLine(measurements);

// Stage 2 - Reduce measurement data into desired output
// TODO

// Optional Stage 3 - Save the output to a file on disk
// TODO