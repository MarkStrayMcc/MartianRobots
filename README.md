﻿# Martian.Robots 🤖
A C# console app that simulates robot movement on a Martian world (the world being a grid!). Robots follow turn/forward instructions until complete or the robot has fallen off the grid or attempts to hit a marker that indicates that another robot has fallen off, the robots final position is reported. Includes unit tests and clean architecture.

### Solution Structure

	Martian.Robots.sln
	├── Martian.Robots                 # console app
	│	├── Core/
	│	│   ├── IWorld.cs              # Interface for world interactions
	│	│   ├── Robot.cs               # Robot movement and instruction processing
	│	│   └── World.cs               # Grid boundaries and scent marker logic
	│	├── Models/
	│	│   ├── Orientation.cs         # Cardinal direction enum
	│	│   └── Position.cs            # Immutable coordinate record
	│	├── Program.cs                 # Main application entry point
	│	├── README.md                  
	│	└── .gitignore                 
	│
	└── Martian.Robots.Tests           # test suite, uses NUnit
		├── Core/
		│   ├── RobotTests.cs          # Tests for robot behavior (uses Moq)
		│   └── WorldTests.cs          # Tests for world logic
		└── Models/
			└── PositionTests.cs       # Tests for position functionality           

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

#### World Class (World.cs)
 - Single Responsibility: Manages only grid rules and scent markers.
 - Immutable Dimensions: Grid size cannot change after creation.
 - Thread-Safety: Uses ConcurrentDictionary for scent markers to support potential concurrent robot simulations.
 - Boundary Enforcement: Encapsulates grid dimensions (MaxX, MaxY) and validates positions.
 - Scent marker system has atomic operations via TryAdd prevent race conditions.
 - Interface Implementation: Implements IWorld for testability and future extensibility (e.g., different planet types). 
	
#### Robot Class (Robot.cs)
 - Dependency Injection: Accepts IWorld to decouple from concrete implementations.
 - Immutable Position Updates: Uses with expressions to create new Position records instead of modifying state.
 - Instruction Processing:
	- Validates commands (L/R/F only).
	- Short-circuits if robot is lost.
 - State Isolation:
	- _isLost is private; external access only via ToString().
	- Position changes are atomic.
 - Clear Behavior: Each method handles one logical operation (turning/moving).
 - Fail-Fast Validation: Throws exceptions for invalid initial positions or commands.
 - Thread-Safe Design: No shared state between robots.
