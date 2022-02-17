using Avengers;
using FluentAssertions;
using System.Collections.Generic;
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
                "ATGGCGT",
            };

            var expected = "ATGGCGTGCAATGG";

            var assembler = new SimpleAssembler(input, 5);

            // Act
            var actual = assembler.Assemble();

            // Assert
            actual.Should().Be(expected);
        }
    }
}