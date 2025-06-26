﻿# Martian.Robots 🤖
A C# console app that simulates robot movement on a Martian world (the world being a grid!). Robots follow turn/forward instructions until complete or the robot has fallen off the grid or attempts to hit a marker that indicates that another robot has fallen off, the robots final position is reported. Includes unit tests and clean architecture.

## Solution Structure

	Martian.Robots.sln
	├── Martian.Robots                 # console app
	│	├── Program.cs                 # Main application entry point
	│	├── README.md                  
	│	└── .gitignore                 
	│
	└── Martian.Robots.Tests           # test suite, uses NUnit
		└── UnitTest1.cs               
