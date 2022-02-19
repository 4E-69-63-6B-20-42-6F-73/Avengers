using Avengers;
using Cocona;
using Xyaneon.Bioinformatics.FASTA;
using Xyaneon.Bioinformatics.FASTA.IO;

class Program
{
    static void Main(string[] args) => CoconaApp.Run<Program>(args);

    public void Assemble([Argument]string filePath, [Argument]int kmerSize, [Argument]string outputFileName)
    {
        Console.WriteLine($"Assembling {filePath}"); // TODO Convert to ILogger;

        var sequences = SequenceFileReader.ReadMultipleFromFile(filePath);

        var data = sequences.Select(x => x.Data.Characters).ToList();

        var assembler = new SimpleAssembler(data, kmerSize);
        var result = assembler.Assemble();

        File.WriteAllLines(outputFileName, result);
    }

    public void Analyze([Argument] string filePath, [Argument] int kmerSize)
    {
        Console.WriteLine($"Analyzing {filePath}"); // TODO Convert to ILogger;
        Console.WriteLine($"Reading reads");

        var reads = SequenceFileReader.ReadMultipleFromFile(filePath).Select(x => x.Data.Characters).ToList();

        Console.WriteLine($"Reads count  : {reads.Count()}");
        Console.WriteLine($"Total bases  : {reads.Sum(x => x.Length)}");
        Console.WriteLine($"average bases: {reads.Average(x => x.Length)}");

        Console.WriteLine($"Generating Kmers");
        var kmers = reads.AsParallel().SelectMany(x => new KmerGenerator().Generate(x, kmerSize)).ToList();

        Console.WriteLine($"Grouping Kmers");
        var kmersCounts = kmers.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());


        //Console.WriteLine($"Kmer \t Count");
        //foreach (var item in kmersCounts)
        //{
        //    Console.WriteLine($"{item.Key} \t {item.Value}");
        //}

        Console.WriteLine($"Totaal diffrent Kmers  : {kmersCounts.Count()}");
        Console.WriteLine($"Totaal Kmers generated : {kmersCounts.Sum(x => x.Value)}");
        Console.WriteLine($"Average Kmers occurence: {kmersCounts.Average(x => x.Value)}");
        
        var filteredKmers = kmersCounts.Where(x => x.Value > 95).Select(x => x.Key);

        var assembler = new SimpleAssembler(reads, kmerSize);
        var result = assembler.Assemble(filteredKmers).ToList();

        File.WriteAllLines("test", result);
    }
}