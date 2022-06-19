### Build Instructions
 - Make sure to change the output type in `MonoRogue.csproj` from `Exe` to `WinExe`. It is currently set to `Exe` so we can get debugging statements in the terminal.
 - Run `dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`
    - https://docs.monogame.net/articles/packaging_games.html
    - This generates a ton of extra dlls. See if there is a way to make this smaller.
 - Should end up in `bin\Release\netcoreapp3.1\win-x64\publish\`, zip this folder


**Notes**
 
 Attempting to trim the resulting builds. Changed the trimmer in [MonoRogue.csproj](MonoRogue.csproj) from this:

 `<TrimmerRootAssembly Include="Microsoft.Xna.Framework.Content.ContentTypeReader" Visible="false" />`

 To this

 `<TrimmerRootAssembly Include="MonoGame.Framework" Visible="false" />`

 And `PublishTrimmed` should be set to `true`

 - https://docs.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained

Alternatively, we can publish it as a single file by setting `PublishSingleFile` to be true. This has its own issues, documented [here](https://docs.monogame.net/articles/packaging_games.html#singlefilepublish) although this is likely the best option? If we can get a trimmed build to actually work, we should deploy both the zip and the single executable. Setting PublishTrimmed to true literally cuts the file size in half, but also doesn't run so we will need to figure out a way to get that working.
