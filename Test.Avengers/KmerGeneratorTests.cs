using Avengers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.Avengers
{
    public class KmerGeneratorTests
    {
        [Fact]
        public void CanGenerateKmers() 
        {
            // Arrange
            var expected = new List<string>
            {
                "ATGGC", 
                "TGGCG",
                "GGCGT",
            };

            var generator = new KmerGenerator();

            // Act
            var actual = generator.Generate("ATGGCGT", 5);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public void CanGenerateKmers2()
        {
            // Arrange
            var expected = new List<string>
            {
                "ATG",
                "TGG",
                "GGC",
                "GCG",
                "CGT",
            };

            var generator = new KmerGenerator();

            // Act
            var actual = generator.Generate("ATGGCGT", 3);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
