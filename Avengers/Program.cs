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

        var assembler = new SimpleAssembler(sequences.Select(x => x.Data.Characters), kmerSize);
        var result = assembler.Assemble();

        SequenceFileWriter.WriteToInterleavedFile(Sequence.Parse(result), outputFileName, 60);
    }
}