using Martian.Robots.Core;
using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Commands;
using Martian.Robots.Core.Services;
using Martian.Robots.Models;
using Moq;
using NUnit.Framework;
using System;

namespace Martian.Robots.Tests.Core
{
    [TestFixture]
    public class RobotTests
    {
        private Mock<IWorld> _worldMock;
        private Mock<ICommandRegistry> _commandRegistryMock;
        private const int TestX = 1;
        private const int TestY = 1;
        private const Orientation TestOrientation = Orientation.N;

        [SetUp]
        public void Setup()
        {
            _worldMock = new Mock<IWorld>();
            _worldMock.Setup(w => w.IsPositionValid(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            _worldMock.Setup(w => w.TryRecordMarker(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>())).Returns(true);

            _commandRegistryMock = new Mock<ICommandRegistry>();
            SetupDefaultCommands();
        }

        private void SetupDefaultCommands()
        {
            _commandRegistryMock.Setup(c => c.GetCommand('L')).Returns(new TurnLeftCommand());
            _commandRegistryMock.Setup(c => c.GetCommand('R')).Returns(new TurnRightCommand());
            _commandRegistryMock.Setup(c => c.GetCommand('F')).Returns(new MoveForwardCommand());
        }

        #region Constructor Tests
        [Test]
        public void Constructor_ValidParameters_CreatesRobotWithCorrectInitialState()
        {
            // Arrange
            _worldMock.Setup(w => w.IsPositionValid(TestX, TestY)).Returns(true);

            // Act
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.CurrentPosition.X, Is.EqualTo(TestX));
                Assert.That(robot.CurrentPosition.Y, Is.EqualTo(TestY));
                Assert.That(robot.CurrentPosition.Orientation, Is.EqualTo(TestOrientation));
                Assert.That(robot.ToString(), Is.EqualTo($"{TestX} {TestY} {TestOrientation}"));
            });
        }

        [Test]
        public void Constructor_InvalidPosition_ThrowsArgumentException()
        {
            // Arrange
            _worldMock.Setup(w => w.IsPositionValid(TestX, TestY)).Returns(false);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object));
        }

        [Test]
        public void Constructor_NegativeCoordinates_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Robot(-1, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object));
        }
        #endregion

        #region Movement Tests
        [Test]
        public void ProcessInstructions_ForwardCommand_UpdatesPositionCorrectly()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);
            _worldMock.Setup(w => w.IsPositionValid(TestX, TestY + 1)).Returns(true);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.That(robot.CurrentPosition.Y, Is.EqualTo(TestY + 1));
        }

        [Test]
        public void ProcessInstructions_ForwardOffGrid_FirstTime_MarksAsLost()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);
            _worldMock.Setup(w => w.IsPositionValid(TestX, TestY + 1)).Returns(false);
            _worldMock.Setup(w => w.TryRecordMarker(TestX, TestY, TestOrientation)).Returns(true);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.CurrentPosition.IsLost, Is.True);
                Assert.That(robot.ToString(), Does.EndWith("LOST"));
            });
        }

        [Test]
        public void ProcessInstructions_ForwardOffGrid_WithExistingScent_IgnoresCommand()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);
            _worldMock.Setup(w => w.IsPositionValid(TestX, TestY + 1)).Returns(false);
            _worldMock.Setup(w => w.TryRecordMarker(TestX, TestY, TestOrientation)).Returns(false);

            // Act
            robot.ProcessInstructions("F");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.CurrentPosition.IsLost, Is.False);
                Assert.That(robot.CurrentPosition.Y, Is.EqualTo(TestY)); // Position unchanged
            });
        }

        [Test]
        public void ProcessInstructions_AfterLost_IgnoresSubsequentCommands()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);
            _worldMock.SetupSequence(w => w.IsPositionValid(TestX, TestY + 1))
                .Returns(false)  // First F will be invalid
                .Returns(true);  // This should never be reached

            // Act
            robot.ProcessInstructions("FF");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(robot.CurrentPosition.IsLost, Is.True);
                Assert.That(robot.CurrentPosition.Y, Is.EqualTo(TestY)); // Second F was ignored
            });
        }
        #endregion

        #region Rotation Tests
        [Test]
        public void ProcessInstructions_LeftCommand_RotatesCorrectly()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);

            // Act
            robot.ProcessInstructions("L");

            // Assert
            Assert.That(robot.CurrentPosition.Orientation, Is.EqualTo(Orientation.W));
        }

        [Test]
        public void ProcessInstructions_RightCommand_RotatesCorrectly()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);

            // Act
            robot.ProcessInstructions("R");

            // Assert
            Assert.That(robot.CurrentPosition.Orientation, Is.EqualTo(Orientation.E));
        }

        [Test]
        public void ProcessInstructions_MultipleRotations_ReturnsToOriginalOrientation()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);

            // Act
            robot.ProcessInstructions("RRRR");

            // Assert
            Assert.That(robot.CurrentPosition.Orientation, Is.EqualTo(TestOrientation));
        }
        #endregion

        #region Instruction Validation Tests
        [Test]
        public void ProcessInstructions_EmptyString_DoesNothing()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);

            // Act
            robot.ProcessInstructions("");

            // Assert
            Assert.That(robot.CurrentPosition, Is.EqualTo(new Position(TestX, TestY, TestOrientation)));
        }

        [Test]
        public void ProcessInstructions_InvalidCommand_ThrowsInvalidOperationException()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);
            _commandRegistryMock.Setup(c => c.GetCommand('X')).Throws<InvalidOperationException>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => robot.ProcessInstructions("X"));
        }

        [Test]
        public void ProcessInstructions_ExceedsMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var robot = new Robot(TestX, TestY, TestOrientation, _worldMock.Object, _commandRegistryMock.Object);
            var longInstructions = new string('F', Robot.MaxInstructionLength + 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => robot.ProcessInstructions(longInstructions));
        }
        #endregion

        #region Complex Scenarios
        [Test]
        public void ProcessInstructions_SampleInput1_CorrectOutput()
        {
            // Arrange
            var world = new World(5, 3);
            var commandRegistry = new CommandRegistry();
            commandRegistry.Register(new TurnLeftCommand());
            commandRegistry.Register(new TurnRightCommand());
            commandRegistry.Register(new MoveForwardCommand());

            var robot = new Robot(1, 1, Orientation.E, world, commandRegistry);

            // Act
            robot.ProcessInstructions("RFRFRFRF");

            // Assert
            Assert.That(robot.ToString(), Is.EqualTo("1 1 E"));
        }

        [Test]
        public void ProcessInstructions_SampleInput2_CorrectOutput()
        {
            // Arrange
            var world = new World(5, 3);
            var commandRegistry = new CommandRegistry();
            commandRegistry.Register(new TurnLeftCommand());
            commandRegistry.Register(new TurnRightCommand());
            commandRegistry.Register(new MoveForwardCommand());

            var robot = new Robot(3, 2, Orientation.N, world, commandRegistry);

            // Act
            robot.ProcessInstructions("FRRFLLFFRRFLL");

            // Assert
            Assert.That(robot.ToString(), Is.EqualTo("3 3 N LOST"));
        }
        #endregion
    }
}