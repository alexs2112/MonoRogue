# MonoRogue
A work in progress simple turn based roguelike written in monogame to learn how to use C#

## Run Instructions:
 - `dotnet run Program.cs` in the terminal will compile and run the game for testing.
 - Optional Parameters:
    - `--debug` option to enter debug mode
    - `--seed <seed>` to input a seed to use for world generation
    - `--messages` to print player notifications to the terminal
    - `--invincible` to stop the player from modifying their health
    - `--difficulty <1|2|3>` to set the rate of enemy and item spawn. 1 is easiest, 3 is hardest (default)

## Algorithms in use:
 - Dungeon generation uses a combination of Binary Space Partitioning, Cellular Automata, Depth First Search, and Kruskal's Algorithm to create world of Caves and Rooms connected by hallways. [DungeonGeneration.cs](World/DungeonGeneration.cs)
 - Using a pseudo dynamic-routing algorithm to find start and end regions that are furthest from each other, and assign each other dungeon region a level of difficulty based on how far they are from the start.
 - Field of View is done through the Bresenham line algorithm, storing seen tiles to differentiate between explored and unexplored parts of the dungeon.
 - A* pathfinding algorithm for hostile enemies.

## Current Roadmap:
 - [Roadmap](Roadmap.md)

## [User Manual](UserManual.md)
