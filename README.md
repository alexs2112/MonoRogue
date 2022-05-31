# MonoRogue
A work in progress simple turn based roguelike written in monogame to learn how to use C#

Not very exciting or interesting as of now.

## Build Instructions:
 - `dotnet run program.cs` in the terminal will compile and run the game for testing.
 - Make sure to change the output type in `MonoRogue.csproj` from `Exe` to `WinExe`. It is currently set to `Exe` so we can get debugging statements in the terminal.
 - Run `dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`
    - https://docs.monogame.net/articles/packaging_games.html
    - This generates a ton of extra dlls. See if there is a way to make this smaller.
