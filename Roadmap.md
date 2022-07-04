### **Roadmap:**
 * Balance Update:
   * Change enemy equipment so that they don't constantly drop the same thing. More varied equipment that doesn't provide huge buffs and spawns more often
   * Reduce the rate at which food and items drop (since items will be on enemies)
   * Have more enemies speak

 * Other subscreens:
    * The remaining help screens, multiple menus to swap through to see info about the game
      * Make sure to adjust chars per line for screen size
    * Similar to the fire screen, a way to view tiles without using the mouse

 * Persistent settings
    * Settings screen, volume settings and player character colour. Settings should persist in a text file somewhere
      * Changing player colour changes game name. Set gender as well. "Escape of the Yellow Person"
    * Keybindings screen in settings <- Not sure if we have enough keybindings for this lol, just map all of the movement controls
    * Pretty up the window resize screen. Maybe disable screen sizes that are too big if possible, seems to mess things up if you set it to bigger than your physical screen and in fullscreen mode. If we can't get monitor size (niche case of external monitor) maybe add a warning like "This window size is greater than your detected monitor size of ___, proceed with caution"
      * Maybe a "Fit to Screen" button that will match it to our resultion and enable full screen (which can then be turned off but might be ugly)

 * Music and sound effects
    * Arcady sound effects for food and stuff?

 * Look into enabling saving and persistency
    * I think we can just copy down the world seed, regenerate the entire world. Copy the list of remaining creatures and items and their locations and equipment and stuff and then place them where needed. Could be easy, don't waste too much time on this if it won't work

 * Add ways for vaults to specify tiles to force extra enemies and items to spawn. Can add tiles to prevent enemies and items from spawning.

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
   * Test persistent settings
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.
   * Make sure all in game terminology is correct
   * Screenshots, GameJolt page, Itch page, Itch+GJ profile, RogueBasin page

 * **Release!**
