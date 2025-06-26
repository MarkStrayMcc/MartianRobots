using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Commands;
using Martian.Robots.Models;
using Moq;
using NUnit.Framework;

namespace Martian.Robots.Tests.Core.Commands
{
    [TestFixture]
    public class MoveForwardCommandTests
    {
        private MoveForwardCommand _command;
        private Mock<IWorld> _worldMock;

        [SetUp]
        public void Setup()
        {
            _command = new MoveForwardCommand();
            _worldMock = new Mock<IWorld>();
            _worldMock.Setup(w => w.IsPositionValid(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
        }

        #region Valid Movements
        [Test]
        public void Execute_North_IncrementsY()
        {
            var position = new Position(1, 1, Orientation.N);
            var result = _command.Execute(position, _worldMock.Object);

            Assert.That(result, Is.EqualTo(position with { Y = 2 }));
        }

        [Test]
        public void Execute_East_IncrementsX()
        {
            var position = new Position(1, 1, Orientation.E);
            var result = _command.Execute(position, _worldMock.Object);

            Assert.That(result, Is.EqualTo(position with { X = 2 }));
        }

        [Test]
        public void Execute_South_DecrementsY()
        {
            var position = new Position(1, 1, Orientation.S);
            var result = _command.Execute(position, _worldMock.Object);

            Assert.That(result, Is.EqualTo(position with { Y = 0 }));
        }

        [Test]
        public void Execute_West_DecrementsX()
        {
            var position = new Position(1, 1, Orientation.W);
            var result = _command.Execute(position, _worldMock.Object);

            Assert.That(result, Is.EqualTo(position with { X = 0 }));
        }
        #endregion

        #region Boundary Conditions
        [Test]
        public void Execute_AtWorldEdge_ValidPosition_DoesNotMarkLost()
        {
            _worldMock.Setup(w => w.IsPositionValid(1, 2)).Returns(true);
            var position = new Position(1, 1, Orientation.N);

            var result = _command.Execute(position, _worldMock.Object);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsLost, Is.False);
                _worldMock.Verify(w => w.TryRecordMarker(1, 1, Orientation.N), Times.Never);
            });
        }

        [Test]
        public void Execute_OutsideWorld_MarksAsLostAndRecordsScent()
        {
            _worldMock.Setup(w => w.IsPositionValid(1, 2)).Returns(false);
            _worldMock.Setup(w => w.TryRecordMarker(1, 1, Orientation.N)).Returns(true);
            var position = new Position(1, 1, Orientation.N);

            var result = _command.Execute(position, _worldMock.Object);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsLost, Is.True);
                _worldMock.Verify(w => w.TryRecordMarker(1, 1, Orientation.N), Times.Once);
            });
        }

        [Test]
        public void Execute_OutsideWorld_WithExistingScent_IgnoresMove()
        {
            _worldMock.Setup(w => w.IsPositionValid(1, 2)).Returns(false);
            _worldMock.Setup(w => w.TryRecordMarker(1, 1, Orientation.N)).Returns(false); // Scent exists
            var position = new Position(1, 1, Orientation.N);

            var result = _command.Execute(position, _worldMock.Object);

            Assert.That(result, Is.EqualTo(position)); // Position unchanged
        }
        #endregion

        #region Property Tests
        [Test]
        public void Symbol_ReturnsF()
        {
            Assert.That(_command.Symbol, Is.EqualTo('F'));
        }
        #endregion
    }
}