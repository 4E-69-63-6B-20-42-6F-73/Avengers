using Avengers;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Test.Avengers
{
    public class SimpleAssemblerTests
    {
        [Fact]
        public void CanAssembleSimulatedReads()
        {
            // Arrange
            var input = new List<string>()
            {
                "ATGGCGT",
                "GGCGTGC",
                "CGTGCAA",
                "TGCAATG",
                "CAATGGC",
                //"ATGGCGT",
            };

            var expected = "ATGGCGTGCAATGG";

            var assembler = new SimpleAssembler(input, 5);

            // Act
            var actual = assembler.Assemble();

            // Assert
            actual.Single().Should().Be(expected);
        }

        [Fact]
        public void CanAssembleSingleRead()
        {
            // Arrange
            var input = new List<string>()
            {
                "ATGGCGT",
            };

            var expected = "ATGGCGT";

            var assembler = new SimpleAssembler(input, 3);

            // Act
            var actual = assembler.Assemble();

            // Assert
            actual.Single().Should().Be(expected);
        }
    }
}