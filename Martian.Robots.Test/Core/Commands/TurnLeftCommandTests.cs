using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Commands;
using Martian.Robots.Models;
using Moq;
using NUnit.Framework;

namespace Martian.Robots.Tests.Core.Commands
{
    [TestFixture]
    public class TurnLeftCommandTests
    {
        private TurnLeftCommand _command;
        private Mock<IWorld> _worldMock;

        [SetUp]
        public void Setup()
        {
            _command = new TurnLeftCommand();
            _worldMock = new Mock<IWorld>(); // World mock isn't used but required by interface
        }

        #region Rotation Tests
        [Test]
        public void Execute_NorthTurnsWest()
        {
            var position = new Position(0, 0, Orientation.N);
            var result = _command.Execute(position, _worldMock.Object);
            Assert.That(result.Orientation, Is.EqualTo(Orientation.W));
        }

        [Test]
        public void Execute_WestTurnsSouth()
        {
            var position = new Position(0, 0, Orientation.W);
            var result = _command.Execute(position, _worldMock.Object);
            Assert.That(result.Orientation, Is.EqualTo(Orientation.S));
        }

        [Test]
        public void Execute_SouthTurnsEast()
        {
            var position = new Position(0, 0, Orientation.S);
            var result = _command.Execute(position, _worldMock.Object);
            Assert.That(result.Orientation, Is.EqualTo(Orientation.E));
        }

        [Test]
        public void Execute_EastTurnsNorth()
        {
            var position = new Position(0, 0, Orientation.E);
            var result = _command.Execute(position, _worldMock.Object);
            Assert.That(result.Orientation, Is.EqualTo(Orientation.N));
        }
        #endregion

        #region Position Integrity
        [Test]
        public void Execute_DoesNotChangeCoordinates()
        {
            var position = new Position(1, 2, Orientation.N);
            var result = _command.Execute(position, _worldMock.Object);

            Assert.Multiple(() =>
            {
                Assert.That(result.X, Is.EqualTo(1));
                Assert.That(result.Y, Is.EqualTo(2));
            });
        }

        [Test]
        public void Execute_DoesNotAffectLostStatus()
        {
            var position = new Position(0, 0, Orientation.N) { IsLost = true };
            var result = _command.Execute(position, _worldMock.Object);
            Assert.That(result.IsLost, Is.True);
        }
        #endregion

        #region Edge Cases
        [Test]
        public void Execute_FullRotation_ReturnsToOriginalOrientation()
        {
            var position = new Position(0, 0, Orientation.N);

            var result = position;
            for (int i = 0; i < 4; i++) // 4 left turns = 360°
            {
                result = _command.Execute(result, _worldMock.Object);
            }

            Assert.That(result.Orientation, Is.EqualTo(position.Orientation));
        }

        [Test]
        public void Execute_InvalidOrientation_DefaultsToCurrent()
        {
            var position = new Position(0, 0, (Orientation)99); // Invalid enum value
            var result = _command.Execute(position, _worldMock.Object);
            Assert.That(result.Orientation, Is.EqualTo(position.Orientation));
        }
        #endregion

        #region Command Properties
        [Test]
        public void Symbol_ReturnsL()
        {
            Assert.That(_command.Symbol, Is.EqualTo('L'));
        }
        #endregion

        #region World Interaction
        [Test]
        public void Execute_IgnoresWorldState()
        {
            var position = new Position(0, 0, Orientation.N);
            _worldMock.Setup(w => w.IsPositionValid(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            var result = _command.Execute(position, _worldMock.Object);

            _worldMock.Verify(w => w.IsPositionValid(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            Assert.That(result.Orientation, Is.EqualTo(Orientation.W));
        }
        #endregion
    }
}