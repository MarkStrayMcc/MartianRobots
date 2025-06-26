﻿# Martian.Robots 🤖
A C# console app that simulates robot movement on a Martian world (the world being a grid!). Robots follow turn/forward instructions until complete or the robot has fallen off the grid or attempts to hit a marker that indicates that another robot has fallen off, the robots final position is reported. Includes unit tests and clean architecture.

### Solution Structure

	Martian.Robots.sln
	├── Martian.Robots                 # Console Application
	│   ├── Core/
	│   │   ├── Abstractions/				# Interfaces
	│   │   │   ├── ICommand.cs				# Command pattern contract
	│   │   │   ├── ICommandRegistry.cs     # Interface for the command registry
	│   │   │   ├── IWorld.cs				# World interactions
	│   │   │   └── IWorldFactory.cs		# World creation
	│   │   ├── Commands/					# Command implementations
	│   │   │   ├── MoveForwardCommand.cs
	│   │   │   ├── TurnLeftCommand.cs
	│   │   │   └── TurnRightCommand.cs
	│   │   ├── Services/
	│   │   │   ├── CommandRegistry.cs		# Command dispatcher
	│   │   │   └── WorldFactory.cs			# World builder (config used here?)
	│   │   ├── Robot.cs					# Robot behavior
	│   │   └── World.cs					# Grid/scent logic
	│   ├── Models/
	│   │   ├── Orientation.cs         # N/E/S/W enum
	│   │   └── Position.cs            # Immutable record
	│   ├── Program.cs                 # DI configuration
	│   └── Application.cs             # entry point
	│
	└── Martian.Robots.Tests           # NUnit tests
		├── Core/
		│   ├── Commands/				# Command tests
		│   ├── RobotTests.cs          # Robot behavior (Moq)
		│   ├── WorldTests.cs          # Grid validation
		│   └── CommandTests/          # Command-specific tests
		└── Models/
			└── PositionTests.cs         

### Notes on technical decisions made
#### General approach
 - I want a world that a robot can enter or exist on and move withinthe limits that the world sets, I am so big and you can only move so far. There is additional log to ensure that any previous robot that falls off the planet will leave a marker and this will stop the other robots falling off and being lost but they will stay where they are and stop processing instructions. 

#### Orientation Enum (Orientation.cs)
 - Represents cardinal directions (North, East, South, West)
 - Could be extended with intercardinal directions (NE, SE, etc.) if needed
 - Matches standard compass notation
	
#### Position Record (Position.cs)
 - Immutability: Prevents unintended state changes
 - Value Semantics: Two positions are equal if their properties match
 - Non-destructive Updates: Uses with expressions for movement
 - Clean Formatting: Built-in ToString() matches required output format
	

#### The Command Registry (CommandRegistry.cs)
 - Open/Closed Principle: New commands can be added without modifying existing code (just register new ICommand implementations).
 - Robot Class Doesn't Change: The Robot.ProcessInstructions() method stays simple—it doesn't need updates when new commands are introduced.
 - Dynamic Command Registration: Commands can be added/removed at runtime (e.g., for modding or experimental features)
 - Testability
	- Isolated Unit Tests: Each command can be tested independently
	- Easy Mocking: The registry can be mocked to return test doubles

#### World Class (World.cs)
 - Single Responsibility: Manages only grid rules and scent markers.
 - Immutable Dimensions: Grid size cannot change after creation.
 - Thread-Safety: Uses ConcurrentDictionary for scent markers to support potential concurrent robot simulations.
 - Boundary Enforcement: Encapsulates grid dimensions (MaxX, MaxY) and validates positions.
 - Scent marker system has atomic operations via TryAdd prevent race conditions.
 - Interface Implementation: Implements IWorld for testability and future extensibility (e.g., different planet types). 
	
#### Robot Class (Robot.cs)
 - Testability: Easy to mock IWorld and ICommandRegistry
 - Domain Clarity: Matches problem requirements precisely
 - Robustness: Impossible to corrupt state mid-operation
 - Extensibility: New commands require zero robot modifications
	
### Future Enhancements
 - UI, lacks a UI
 - Async Processing: For handling multiple robots concurrently
 - Configuration: Customizable grid sizes and rules

### Running the app
Prerequisites: .NET 6+ SDK

#### command line
Build: dotnet build
Run: dotnet run (from Martian.Robots directory)
Test: dotnet test (from Martian.Robots.Tests directory)

#### visual studio
Build: Hit F6
Run: Hit F5

#### sample Input
5 3 # creates world grid

1 1 E
RFRFRFRF

3 2 N
FRRFLLFFRRFLL

0 3 W
LLFFFLFLFL

Expected Output
1 1 E

3 3 N LOST

2 3 S 
