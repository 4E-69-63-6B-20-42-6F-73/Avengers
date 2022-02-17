using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avengers
{
    public class SimpleAssembler
    {
        private readonly IEnumerable<string> reads;
        private readonly int k;
        private readonly KmerGenerator kmerGenerator;

        public SimpleAssembler(IEnumerable<string> reads, int k, KmerGenerator? kmerGenerator = null)
        {
            this.reads = reads;
            this.k = k;

            this.kmerGenerator = kmerGenerator ?? new KmerGenerator();
        }

        /// <summary>
        /// Assemble the reads.
        /// </summary>
        /// <returns></returns>
        public string Assemble() 
        {
            var graph = new AdjacencyGraph<string, ValueEdge<string,string>>();

           
            foreach (var read in reads)
            {
                var kmers = this.kmerGenerator.Generate(read, k);

                foreach (var kmer in kmers)
                {
                    var thing1 = kmer[..^1];
                    var thing2 = kmer[1..];

                    graph.AddVertex(thing1);
                    graph.AddVertex(thing2);

                    graph.AddEdge(new ValueEdge<string, string>(kmer ,thing1, thing2));
                }
            }

            var thing = new EulerianTrailAlgorithm<string, ValueEdge<string, string>>(graph);
            thing.Compute();

            var orderedKmers = thing.Circuit.Select(x => x.Value);

            return AppendWithOverlap(orderedKmers);
        }

        private string AppendWithOverlap(IEnumerable<string> input)
        {
            // Expecter overlap = string lengt - 1. 

            var stringBuilder = new StringBuilder(input.First());

            foreach (var @string in input.Skip(1))
            {
                stringBuilder.Append(@string.Last());
            }

            return stringBuilder.ToString();
        }
    }
}
