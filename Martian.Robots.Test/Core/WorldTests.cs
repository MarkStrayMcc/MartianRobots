using Martian.Robots.Core;
using Martian.Robots.Core.Abstractions;
using Martian.Robots.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Test.Core
{
    [TestFixture]
    public class WorldTests
    {
        private const int TestMaxX = 5;
        private const int TestMaxY = 3;

        [Test]
        public void Constructor_ValidParameters_SetsProperties()
        {
            // Act
            var world = new World(TestMaxX, TestMaxY);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(world.MaxX, Is.EqualTo(TestMaxX));
                Assert.That(world.MaxY, Is.EqualTo(TestMaxY));
                Assert.That(world, Is.InstanceOf<IWorld>());
            });
        }

        [Test]
        public void Constructor_ExceedsMaxCoordinates_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new World(51, 3));
            Assert.Throws<ArgumentException>(() => new World(5, 51));
        }

        [Test]
        public void IsPositionValid_WhenWithinBounds_ReturnsTrue()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY);

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(world.IsPositionValid(0, 0), Is.True);
                Assert.That(world.IsPositionValid(TestMaxX, TestMaxY), Is.True);
                Assert.That(world.IsPositionValid(TestMaxX / 2, TestMaxY / 2), Is.True);
            });
        }

        [Test]
        public void IsPositionValid_WhenOutsideBounds_ReturnsFalse()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY);

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(world.IsPositionValid(-1, 0), Is.False);
                Assert.That(world.IsPositionValid(0, -1), Is.False);
                Assert.That(world.IsPositionValid(TestMaxX + 1, TestMaxY), Is.False);
                Assert.That(world.IsPositionValid(TestMaxX, TestMaxY + 1), Is.False);
            });
        }

        [Test]
        public void TryRecordMarker_NewMarker_ReturnsTrueAndAddsMarker()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY);
            const int x = 1, y = 1;
            var orientation = Orientation.N;

            // Act
            var result = world.TryRecordMarker(x, y, orientation);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                // Verify marker exists by trying to add again
                Assert.That(world.TryRecordMarker(x, y, orientation), Is.False);
            });
        }

        [Test]
        public void TryRecordMarker_DuplicateMarker_ReturnsFalse()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY);
            const int x = 2, y = 2;
            var orientation = Orientation.S;

            // First add
            world.TryRecordMarker(x, y, orientation);

            // Act
            var result = world.TryRecordMarker(x, y, orientation);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRecordMarker_ConcurrentAccess_ThreadSafe()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY);
            const int x = 3, y = 1;
            var orientation = Orientation.E;
            var results = new ConcurrentBag<bool>();

            // Act
            Parallel.For(0, 1000, _ =>
            {
                results.Add(world.TryRecordMarker(x, y, orientation));
            });

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(results.Count(r => r), Is.EqualTo(1)); // Only one should succeed
                Assert.That(results.Count(r => !r), Is.EqualTo(999)); // Others should fail
            });
        }

        [Test]
        public void TryRecordMarker_InvalidPosition_StillRecordsMarker()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY);
            const int invalidX = TestMaxX + 1;
            const int y = 1;
            var orientation = Orientation.W;

            // Act
            var result = world.TryRecordMarker(invalidX, y, orientation);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void World_Implements_IWorld_Contract()
        {
            // Arrange
            var world = new World(TestMaxX, TestMaxY) as IWorld;

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(world.MaxX, Is.EqualTo(TestMaxX));
                Assert.That(world.MaxY, Is.EqualTo(TestMaxY));
                Assert.That(() => world.IsPositionValid(0, 0), Throws.Nothing);
                Assert.That(() => world.TryRecordMarker(0, 0, Orientation.N), Throws.Nothing);
            });
        }
    }
}
