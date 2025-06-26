﻿# Martian.Robots 🤖
A C# console app that simulates robot movement on a Martian world (the world being a grid!). Robots follow turn/forward instructions until complete or the robot has fallen off the grid or attempts to hit a marker that indicates that another robot has fallen off, the robots final position is reported. Includes unit tests and clean architecture.

## Solution Structure

	Martian.Robots.sln
	├── Martian.Robots                 # console app
	│	├── Models/
	│	│   ├── Orientation.cs         # Cardinal direction enum
	│	│   └── Position.cs            # Immutable coordinate record
	│	├── Program.cs                 # Main application entry point
	│	├── README.md                  
	│	└── .gitignore                 
	│
	└── Martian.Robots.Tests           # test suite, uses NUnit
		└── Models/
			└── PositionTests.cs       # Tests for position functionality           

## Notes on technical decisions made
### Orientation Enum (Orientation.cs)
 - Represents cardinal directions (North, East, South, West)
 - Could be extended with intercardinal directions (NE, SE, etc.) if needed
 - Matches standard compass notation
	
### Position Record (Position.cs)
 - Immutability: Prevents unintended state changes
 - Value Semantics: Two positions are equal if their properties match
 - Non-destructive Updates: Uses with expressions for movement
 - Clean Formatting: Built-in ToString() matches required output format