using Martian.Robots.Models;
using NUnit.Framework;
using System;

namespace Martian.Robots.Tests.Models
{
    [TestFixture]
    public class PositionTests
    {
        private const int TestX = 1;
        private const int TestY = 2;
        private const Orientation TestOrientation = Orientation.N;

        [Test]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Act
            var position = new Position(TestX, TestY, TestOrientation);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(position.X, Is.EqualTo(TestX));
                Assert.That(position.Y, Is.EqualTo(TestY));
                Assert.That(position.Orientation, Is.EqualTo(TestOrientation));
                Assert.That(position.IsLost, Is.False);
            });
        }

        [Test]
        public void ToString_WhenNotLost_ReturnsPositionOnly()
        {
            // Arrange
            var position = new Position(TestX, TestY, TestOrientation);

            // Act & Assert
            Assert.That(position.ToString(), Is.EqualTo($"{TestX} {TestY} {TestOrientation}"));
        }

        [Test]
        public void ToString_WhenLost_ReturnsPositionWithLostMarker()
        {
            // Arrange
            var position = new Position(TestX, TestY, TestOrientation) { IsLost = true };

            // Act & Assert
            Assert.That(position.ToString(), Is.EqualTo($"{TestX} {TestY} {TestOrientation} LOST"));
        }

        [Test]
        [TestCase(0, 0, Orientation.N, false, "0 0 N")]
        [TestCase(5, 3, Orientation.S, true, "5 3 S LOST")]
        [TestCase(50, 50, Orientation.W, false, "50 50 W")]
        public void ToString_VariousPositions_ReturnsExpectedFormat(int x, int y, Orientation orientation, bool isLost, string expected)
        {
            // Arrange
            var position = new Position(x, y, orientation) { IsLost = isLost };

            // Act & Assert
            Assert.That(position.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void WithExpression_CanSetIsLost()
        {
            // Arrange
            var original = new Position(TestX, TestY, TestOrientation);

            // Act
            var modified = original with { IsLost = true };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(modified.IsLost, Is.True);
                Assert.That(modified.X, Is.EqualTo(TestX));
                Assert.That(modified.Y, Is.EqualTo(TestY));
                Assert.That(modified.Orientation, Is.EqualTo(TestOrientation));
            });
        }

        [Test]
        public void WithExpression_CanModifyAllProperties()
        {
            // Arrange
            var original = new Position(TestX, TestY, TestOrientation);

            // Act
            var modified = original with
            {
                X = 5,
                Y = 3,
                Orientation = Orientation.E,
                IsLost = true
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(modified.X, Is.EqualTo(5));
                Assert.That(modified.Y, Is.EqualTo(3));
                Assert.That(modified.Orientation, Is.EqualTo(Orientation.E));
                Assert.That(modified.IsLost, Is.True);
            });
        }

        [Test]
        public void WithExpression_LeavesOriginalUnchanged()
        {
            // Arrange
            var original = new Position(TestX, TestY, TestOrientation);
            var originalHash = original.GetHashCode();

            // Act
            var modified = original with { IsLost = true };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(original.IsLost, Is.False);
                Assert.That(original.GetHashCode(), Is.EqualTo(originalHash));
            });
        }

        [Test]
        public void Equals_ConsidersIsLost()
        {
            // Arrange
            var position1 = new Position(TestX, TestY, TestOrientation);
            var position2 = new Position(TestX, TestY, TestOrientation) { IsLost = true };

            // Act & Assert
            Assert.That(position1.Equals(position2), Is.False);
        }

        [Test]
        public void GetHashCode_ConsidersIsLost()
        {
            // Arrange
            var position1 = new Position(TestX, TestY, TestOrientation);
            var position2 = new Position(TestX, TestY, TestOrientation) { IsLost = true };

            // Act & Assert
            Assert.That(position1.GetHashCode(), Is.Not.EqualTo(position2.GetHashCode()));
        }

        [Test]
        public void EqualityOperator_ConsidersIsLost()
        {
            // Arrange
            var position1 = new Position(TestX, TestY, TestOrientation);
            var position2 = new Position(TestX, TestY, TestOrientation) { IsLost = true };

            // Act & Assert
            Assert.That(position1 == position2, Is.False);
        }

        [Test]
        public void Constructor_MaxCoordinates_DoesNotThrow()
        {
            // Arrange & Act
            var position = new Position(int.MaxValue, int.MaxValue, TestOrientation);

            // Assert
            Assert.Pass("Position created with max coordinates");
        }

        [Test]
        public void WithExpression_WithNoChanges_ReturnsEqualButNotSameInstance()
        {
            // Arrange
            var original = new Position(TestX, TestY, TestOrientation);

            // Act
            var modified = original with { };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(modified, Is.EqualTo(original));
                Assert.That(modified, Is.Not.SameAs(original));
            });
        }

        [Test]
        public void Deconstruct_ReturnsCorrectValues()
        {
            // Arrange
            var position = new Position(TestX, TestY, TestOrientation) { IsLost = true };

            // Act
            var (x, y, orientation) = position;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(TestX));
                Assert.That(y, Is.EqualTo(TestY));
                Assert.That(orientation, Is.EqualTo(TestOrientation));
                // Note: IsLost is not part of the deconstruction
            });
        }
    }
}