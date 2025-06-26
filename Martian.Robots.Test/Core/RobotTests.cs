using Martian.Robots.Core;
using Martian.Robots.Models;
using Moq;
using NUnit.Framework;
using System;

namespace Martian.Robots.Test.Core
{
    [TestFixture]
    public class RobotTests
    {
        private Mock<IWorld> _worldMock;
        private const int TestMaxX = 5;
        private const int TestMaxY = 3;
        private const int ValidX = 1;
        private const int ValidY = 1;

        [SetUp]
        public void Setup()
        {
            _worldMock = new Mock<IWorld>();
            _worldMock.SetupGet(w => w.MaxX).Returns(TestMaxX);
            _worldMock.SetupGet(w => w.MaxY).Returns(TestMaxY);

            _worldMock.Setup(w => w.IsPositionValid(It.IsAny<int>(), It.IsAny<int>()))
                     .Returns(true);

            _worldMock.Setup(w => w.TryRecordMarker(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>()))
                     .Returns(true);
        }

        #region Constructor Tests
        [Test]
        public void Constructor_ValidPosition_CreatesRobotWithCorrectPosition()
        {
            // Act
            var robot = new Robot(ValidX, ValidY, Orientation.N, _worldMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.CurrentPosition.X, Is.EqualTo(ValidX));
                Assert.That(robot.CurrentPosition.Y, Is.EqualTo(ValidY));
                Assert.That(robot.CurrentPosition.Orientation, Is.EqualTo(Orientation.N));
            });
        }

        [Test]
        public void Constructor_InvalidPosition_ThrowsArgumentException()
        {
            // Arrange
            _worldMock.Setup(w => w.IsPositionValid(ValidX, ValidY)).Returns(false);

            // Act & Assert
            Assert.That(() => new Robot(ValidX, ValidY, Orientation.N, _worldMock.Object),
                Throws.ArgumentException
                .With.Message.Contain("out of world boundaries"));
        }

        [Test]
        public void Constructor_NullWorld_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => new Robot(ValidX, ValidY, Orientation.N, null),
                Throws.ArgumentNullException
                .With.Message.Contain("world"));
        }
        #endregion

        #region Movement Tests
        [Test]
        public void MoveForward_ValidMove_UpdatesPosition()
        {
            // Arrange
            var robot = new Robot(ValidX, ValidY, Orientation.E, _worldMock.Object);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.That(robot.CurrentPosition.X, Is.EqualTo(ValidX + 1));
        }

        [Test]
        public void MoveForward_AtWorldEdgeWithNoScent_MarksAsLostAndRecordsMarker()
        {
            // Arrange
            _worldMock.Setup(w => w.IsPositionValid(TestMaxX + 1, ValidY)).Returns(false);
            var robot = new Robot(TestMaxX, ValidY, Orientation.E, _worldMock.Object);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.ToString(), Does.EndWith("LOST"));
                _worldMock.Verify(w => w.TryRecordMarker(TestMaxX, ValidY, Orientation.E), Times.Once);
            });
        }

        [Test]
        public void MoveForward_AtWorldEdgeWithExistingScent_IgnoresCommand()
        {
            // Arrange
            _worldMock.Setup(w => w.IsPositionValid(TestMaxX + 1, ValidY)).Returns(false);
            _worldMock.Setup(w => w.TryRecordMarker(TestMaxX, ValidY, Orientation.E)).Returns(false);
            var robot = new Robot(TestMaxX, ValidY, Orientation.E, _worldMock.Object);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.CurrentPosition.X, Is.EqualTo(TestMaxX));
                Assert.That(robot.ToString(), Does.Not.EndWith("LOST"));
            });
        }
        #endregion

        #region Rotation Tests
        [Test]
        [TestCase("L", Orientation.W, TestName = "TurnLeft_FromNorth_FacesWest")]
        [TestCase("R", Orientation.E, TestName = "TurnRight_FromNorth_FacesEast")]
        [TestCase("LL", Orientation.S, TestName = "TurnLeftTwice_FromNorth_FacesSouth")]
        public void RotationCommands_ChangeOrientationCorrectly(string commands, Orientation expected)
        {
            // Arrange
            var robot = new Robot(ValidX, ValidY, Orientation.N, _worldMock.Object);

            // Act
            robot.ProcessInstructions(commands);

            // Assert
            Assert.That(robot.CurrentPosition.Orientation, Is.EqualTo(expected));
        }
        #endregion

        #region Instruction Processing Tests
        [Test]
        public void ProcessInstructions_EmptyString_DoesNothing()
        {
            // Arrange
            var robot = new Robot(ValidX, ValidY, Orientation.N, _worldMock.Object);

            // Act
            robot.ProcessInstructions("");

            // Assert
            Assert.That(robot.CurrentPosition, Is.EqualTo(new Position(ValidX, ValidY, Orientation.N)));
        }

        [Test]
        public void ProcessInstructions_InvalidCommand_ThrowsInvalidOperationException()
        {
            // Arrange
            var robot = new Robot(ValidX, ValidY, Orientation.N, _worldMock.Object);

            // Act & Assert
            Assert.That(() => robot.ProcessInstructions("FX"),
                Throws.InvalidOperationException
                .With.Message.Contain("Invalid instruction"));
        }

        [Test]
        public void ProcessInstructions_LongValidSequence_ExecutesCorrectly()
        {
            // Arrange
            var sequence = new MockSequence();
            _worldMock.InSequence(sequence)
                .Setup(w => w.IsPositionValid(ValidX, ValidY + 1)).Returns(true); // N
            _worldMock.InSequence(sequence)
                .Setup(w => w.IsPositionValid(ValidX + 1, ValidY + 1)).Returns(true); // E
            _worldMock.InSequence(sequence)
                .Setup(w => w.IsPositionValid(ValidX + 1, ValidY)).Returns(true); // S
            _worldMock.InSequence(sequence)
                .Setup(w => w.IsPositionValid(ValidX, ValidY)).Returns(true); // W

            var robot = new Robot(ValidX, ValidY, Orientation.N, _worldMock.Object);

            // Act
            robot.ProcessInstructions("FRFRFRF");

            // Assert
            Assert.That(robot.CurrentPosition, Is.EqualTo(new Position(ValidX, ValidY, Orientation.W)));
        }
        #endregion

        #region State Representation Tests
        [Test]
        public void ToString_WhenNotLost_ReturnsPositionOnly()
        {
            // Arrange
            var robot = new Robot(ValidX, ValidY, Orientation.S, _worldMock.Object);

            // Act & Assert
            Assert.That(robot.ToString(), Is.EqualTo($"{ValidX} {ValidY} S"));
        }

        [Test]
        public void ToString_WhenLost_ReturnsPositionWithLostMarker()
        {
            // Arrange
            _worldMock.Setup(w => w.IsPositionValid(ValidX + 1, ValidY)).Returns(false);
            var robot = new Robot(ValidX, ValidY, Orientation.E, _worldMock.Object);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.That(robot.ToString(), Is.EqualTo($"{ValidX} {ValidY} E LOST"));
        }
        #endregion
    }
}