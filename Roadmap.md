### **Roadmap:**
 * Balance Update:
   * Change enemy equipment so that they don't constantly drop the same thing. More varied equipment that doesn't provide huge buffs and spawns more often
   * Reduce the rate at which food and items drop (since items will be on enemies)
   * Have more enemies speak

 * Other subscreens:
    * The remaining help screens, multiple menus to swap through to see info about the game
      * Make sure to adjust chars per line for screen size
      * Keybindings
      * Basic game help
      * Essentially rehash the user manual but in game with icons
    * Similar to the fire screen, a way to view tiles without using the mouse
    * Volume settings screen
    * Player settings screen
      * Changing player colour changes game name. Set gender as well. "Escape of the Yellow Person"
    * Pretty up the window resize screen. Increase screen resolution options but limit it on detected screen size
    * Let the start screen go to settings and help screens

 * Music and sound effects
    * Arcady sound effects for food and stuff?

 * Look into enabling saving and persistency
    * I think we can just copy down the world seed, regenerate the entire world. Copy the list of remaining creatures and items and their locations and equipment and stuff and then place them where needed. Could be easy, don't waste too much time on this if it won't work

 * Add ways for vaults to specify tiles to force extra enemies and items to spawn. Can add tiles to prevent enemies and items from spawning.
   * Currently it is technically possible that the exit spawns in an impossible to reach location

 * Minor fixes
   * Have item notifications not stop auto movement
   * Broken walls are a bit messed up, check all 4 sides instead of 2 in Tile
   * Have KeyboardTrack compare keys with other keys to determine direction as a static method, instead of having it check to see if a directional key was pressed in itself (for subscreens)
   * See if you can fix `AL lib: (EE) alc_cleanup: 1 device not closed`, it looks like its not an issue though?
   * Turns out we don't need to overload every constructor, we can set default arguments
   * Have the fire screen automatically select the last guy you targeted
   * Have every enemy that can see an enemy that you hit know where you are

 * Release prep:
   * Fix all @todo tags
   * See if we can clean up app builds to include no unnecessary dlls without breaking
   * Test persistent settings and savegames across different devices
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.
   * Make sure all in game terminology is correct
   * Screenshots, GameJolt page, Itch page, Itch+GJ profile, RogueBasin page

 * **Release!**
