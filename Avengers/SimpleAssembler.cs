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
            var graph = new AdjacencyGraph<string, Edge<string>>();

           
            foreach (var read in reads)
            {
                var kmers = this.kmerGenerator.Generate(read, k);

                foreach (var kmer in kmers)
                {
                    if (!graph.ContainsVertex(kmer)) 
                    {
                        graph.AddVertex(kmer);
                    }
                    
                    /// A[TC] -> [TC]T
                    var overlappingKmer = graph.Vertices.FirstOrDefault(x => x.StartsWith(kmer[1..]));
                    if (overlappingKmer != null)
                    {
                        graph.AddEdge(new Edge<string>(kmer, overlappingKmer));
                    }

                    /// [AT]C -> G[AT]
                     overlappingKmer = graph.Vertices.FirstOrDefault(x => x.EndsWith(kmer[..^1]));
                    if (overlappingKmer != null)
                    {
                        graph.AddEdge(new Edge<string>(overlappingKmer, kmer));
                    }
                }
            }

            // The EulerianTrailAlgorithm calculates the Euler trail not path!
            // The diffrence between euler trail and path is that the trail must start and end on same node.
            // Path not.
            // So we add a new edge from last to first so a euler trail can be created.
            graph.AddEdge(new Edge<string>(graph.Vertices.Last(), graph.Vertices.First()));

            var thing = new EulerianTrailAlgorithm<string, Edge<string>>(graph);
            thing.Compute(graph.Vertices.First());

            var orderedKmers = thing.Circuit.Select(x => x.Source);

            return AppendWithOverlap(orderedKmers);
        }

        private string AppendWithOverlap(IEnumerable<string> input)
        {
            // Expected overlap = string lengt - 1. 

            var stringBuilder = new StringBuilder(input.First());

            foreach (var @string in input.Skip(1))
            {
                stringBuilder.Append(@string.Last());
            }

            return stringBuilder.ToString();
        }
    }
}
