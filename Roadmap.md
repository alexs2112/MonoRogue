### **Roadmap:**
 * Other subscreens:
    * Similar to the fire screen, a way to view tiles without using the mouse
    * Volume settings screen
    * Game settings screen
      * Changing player colour changes game name. Set gender as well. "Escape of the Yellow Person"
        * Escape of the Yellow Person is too long for the main, end, stats screens...
        * This needs to be able to save and load properly
      * Enable/Disable animations here
      * Save these settings, redo saving settings to use json
    * Let the start screen go to settings and help screens
    * Actual game over screen

 * Game history and stats
    * Keep track of score, time taken, difficulty
    * Score can simply be increased by the difficulty of enemies defeated multiplied by some value, with an addition if you escape or not

 * Music and sound effects
    * Arcady sound effects for food, attacks, doors and stuff?
    * Changing settings in various settings screens

 * Add ways for vaults to specify tiles to force extra enemies and items to spawn. Can add tiles to prevent enemies and items from spawning.
   * Currently it is technically possible that the exit spawns in an impossible to reach location
   * A hack is just making a wall tile that looks like a floor tile, to block movement and things spawning in (but the player should never be able to move here anyway)

 * Interface bugs
   * Text in the creature screen can overlap on small screen sizes, 3 lines of description and critical/parry chance will overlap.
   * Gatekeeper icon will overlap with equipment due to long name
   * Warden's plate has the icon overlap the side of the screen
   * Target screen should give priority to targetable enemies, right now it is just by distance. Can also overlap the bottom of the screen
   * Armor subscreen should show hearts, not numbers for defense
   * Creature screen needs to scale by window
   * It is possible that the player can have too many hearts on easy if they have the Warden's Armor (4 blue hearts + 1 blue heart + 5 red hearts)

 * Minor fixes
   * Have item notifications not stop auto movement
   * Have KeyboardTrack compare keys with other keys to determine direction as a static method, instead of having it check to see if a directional key was pressed in itself (for subscreens)
   * See if you can fix `AL lib: (EE) alc_cleanup: 1 device not closed`, it looks like its not an issue though?
   * Turns out we don't need to overload every constructor, we can set default arguments
   * Have the fire screen automatically select the last guy you targeted
     * Display this creature in the main interface as though you are mousing over it
   * Have every enemy that can see an enemy that you hit know where you are
   * Have more enemies speak
   * Do we want PlayerGlyph loading all of the possible glyphs into memory and leaving them all there, or do we want to load them when it changes?
   * Loading a game when the player has a key still allows them to win but does not show the correct exit glyph
   * Reimplement an fps cap, for animations and other systems. Add it to the settings. (Main.TargetElapsedTime)
   * Items need to not be able to drop on the exit...

 * Release prep:
   * Fix all @todo tags
   * Make sure closing the game saves it
   * Dying or winning the game should delete your save
   * See if we can clean up app builds to include no unnecessary dlls without breaking
   * Test persistent settings and savegames across different devices
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.
   * Make sure all in game terminology is correct
   * Screenshots, GameJolt page, Itch page, Itch+GJ profile, RogueBasin page

 * **Release!**
