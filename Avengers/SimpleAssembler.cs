using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.ConnectedComponents;
using QuikGraph.Graphviz;
using System.Text;

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
        /// Assemble the reads into a list of contigs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Assemble(IEnumerable<string>? Kmers = null)
        {

            Console.WriteLine("Creating DeBruijnGraph");
        
            var graph = CreateDeBruijnGraph(Kmers);

            Console.WriteLine("Finding Subgraphs");
            var subgraphs = FindSubgraphs(graph);

            var i = 0;
            Console.WriteLine("Assembling into contigs");
            foreach (var subgraph in subgraphs)
            {
                i++;

                var graphviz = new GraphvizAlgorithm<string, Edge<string>>(subgraph);
                string outputFilePath = graphviz.Generate(new FileDotEngine(), "subgraph"+i.ToString());

                if (graph.OddVertices().Count() % 2 == 1) 
                {
                    Console.WriteLine($"WARNING: subgraph {i} has uneven odd vertices.");
                }

                var orderedKmers = FindOrderedKmers(subgraph);

                if (orderedKmers.Count() == 0)
                {
                    Console.WriteLine($"WARNING: Cannot assemble subgraph {i}");
                    yield return string.Empty;
                }
                else 
                {
                    yield return AssembleKmersIntoContig(orderedKmers);
                }
            }
        }

        private static IEnumerable<string> FindOrderedKmers(BidirectionalGraph<string, Edge<string>> subgraph)
        {
            // The EulerianTrailAlgorithm calculates the Euler trail not path!
            // The diffrence between euler trail and path is that the trail must start and end on same node.
            // Path not.
            // So we add a new edge from a sin to first (no indegree) so a euler trail can be created.
            subgraph.AddEdge(new Edge<string>(subgraph.Sinks().Last(), subgraph.Roots().First()));

            var thing = new EulerianTrailAlgorithm<string, Edge<string>>(subgraph);
            thing.Compute();

            var orderedKmers = thing.Circuit.Select(x => x.Source);
            return orderedKmers;
        }

        private static BidirectionalGraph<string, Edge<string>>[] FindSubgraphs(BidirectionalGraph<string, Edge<string>> graph)
        {
            var connectedComponents = new WeaklyConnectedComponentsAlgorithm<string, Edge<string>>(graph);
            connectedComponents.Compute();

            return connectedComponents.Graphs;
        }

        private BidirectionalGraph<string, Edge<string>> CreateDeBruijnGraph(IEnumerable<string>? Kmers = null)
        {
            var kmers = Kmers ?? reads.SelectMany(x => this.kmerGenerator.Generate(x, k));

            var graph = new QuikGraph.BidirectionalGraph<string, Edge<string>>();

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

            return graph;
        }

        private string AssembleKmersIntoContig(IEnumerable<string> input)
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