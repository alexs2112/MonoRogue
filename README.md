# MonoRogue
A work in progress simple turn based roguelike written in monogame to learn how to use C#

Not very exciting or interesting as of now.

## Run Instructions:
 - `dotnet run program.cs` in the terminal will compile and run the game for testing. `--debug` option to enter debug mode.

## Algorithms in use:
 - Dungeon generation is done by first using Binary Space Partitioning to place non-overlapping rooms. Then uses Kruskal's Algorithm with DFS to construct a minimum spanning tree between those rooms to generate hallways.
 - Field of View is done through the Bresenham line algorithm, storing seen tiles to differentiate between explored and unexplored parts of the dungeon.
 - A* pathfinding algorithm for hostile enemies.
