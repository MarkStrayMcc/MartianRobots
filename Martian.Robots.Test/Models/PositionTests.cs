using Martian.Robots.Models;

namespace Martian.Robots.Test.Models
{
    [TestFixture]
    public class PositionTests
    {
        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var x = 1;
            var y = 2;
            var orientation = Orientation.E;
            var position = new Position(x, y, orientation);
            var expectedResult = $"{x} {y} {orientation}";

            // Act
            var result = position.ToString();

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange 
            var x = 1;
            var y = 3;
            var orientation = Orientation.E;

            // Act
            var position = new Position(x, y, orientation);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(position.X, Is.EqualTo(x));
                Assert.That(position.Y, Is.EqualTo(y));
                Assert.That(position.Orientation, Is.EqualTo(orientation));
            });
        }

        [Test]
        public void Constructor_MaxCoordinates_DoesNotThrow()
        {
            // Arrange & Act
            var position = new Position(int.MaxValue, int.MaxValue, Orientation.E);

            // Assert
            Assert.Pass("Position created with max coordinates");
        }

        [Test]
        public void WithExpression_CreatesNewPosition()
        {
            // Arrange
            var x = 1;
            var y = 1;
            var orientation = Orientation.N;
            var original = new Position(x, y, orientation);
            var xMod = 2;

            // Act
            var modified = original with { X = xMod };

            // Assert
            Assert.That(modified.X, Is.EqualTo(xMod));
            Assert.That(modified.Y, Is.EqualTo(y));
            Assert.That(modified.Orientation, Is.EqualTo(orientation));
            Assert.That(modified, Is.Not.SameAs(original));
        }
    }
}