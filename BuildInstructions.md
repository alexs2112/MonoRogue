### Build Instructions
 - Make sure to change the output type in `MonoRogue.csproj` from `Exe` to `WinExe`. It is currently set to `Exe` so we can get debugging statements in the terminal.
 - Run `dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`
    - https://docs.monogame.net/articles/packaging_games.html
    - This generates a ton of extra dlls. See if there is a way to make this smaller.
