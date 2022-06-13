# MonoRogue
A work in progress simple turn based roguelike written in monogame to learn how to use C#

## Run Instructions:
 - `dotnet run program.cs` in the terminal will compile and run the game for testing.
 - Optional Parameters:
    - `--debug` option to enter debug mode
    - `--seed <seed>` to input a seed to use for world generation

## Algorithms in use:
 - Dungeon generation uses a combination of Binary Space Partitioning, Cellular Automata, Depth First Search, and Kruskal's Algorithm to create world of Caves and Rooms connected by hallways. [DungeonGeneration.cs](World/DungeonGeneration.cs)
 - Field of View is done through the Bresenham line algorithm, storing seen tiles to differentiate between explored and unexplored parts of the dungeon.
 - A* pathfinding algorithm for hostile enemies.

## Current Roadmap:
[Roadmap](Roadmap.md)
