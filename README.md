# MonoRogue
A work in progress simple turn based roguelike written in monogame to learn how to use C#

Not very exciting or interesting as of now.

## Build Instructions:
 - `dotnet run program.cs` in the terminal will compile and run the game for testing. `--debug` option to enter debug mode.
 - Make sure to change the output type in `MonoRogue.csproj` from `Exe` to `WinExe`. It is currently set to `Exe` so we can get debugging statements in the terminal.
 - Run `dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`
    - https://docs.monogame.net/articles/packaging_games.html
    - This generates a ton of extra dlls. See if there is a way to make this smaller.

## Algorithms in use:
 - Dungeon generation is done by first using Binary Space Partitioning to place non-overlapping rooms. Then uses Kruskal's Algorithm with DFS to construct a minimum spanning tree between those rooms to generate hallways.
