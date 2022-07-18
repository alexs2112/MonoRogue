### **Roadmap:**
 * AI Improvements:
    * Have enemies "shout" when they see you or when you hit them, alerting other nearby enemies of your position (including through walls)
    * Have more enemies speak and interact with the player, change it from "Pig: Oink" to "The pig oinks"

 * Other subscreens:
    * Similar to the fire screen, a way to view tiles without using the mouse
    * Actual game over screen
    * A help screen that details the different difficulties
    * Ability to select difficulty in the main menu

 * Game history and stats
    * Keep track of score, time taken, difficulty
    * Score can simply be increased by the difficulty of enemies defeated multiplied by some value, with an addition if you escape or not

 * Music and sound effects
    * Changing settings in various settings screens
    * Attacks (start of animations/attack if no animations)
    * Doors
    * Food
    * Winning
    * Things dying (maybe overwrite the attack sound)

 * Add ways for vaults to specify tiles to force extra enemies and items to spawn. Can add tiles to prevent enemies and items from spawning.
   * Currently it is technically possible that the exit spawns in an impossible to reach location
   * A hack is just making a wall tile that looks like a floor tile, to block movement and things spawning in (but the player should never be able to move here anyway)

 * Interface bugs
   * Text in the creature screen can overlap on small screen sizes, 3+ lines of description and critical/parry chance will overlap.
   * Gatekeeper icon will overlap with equipment in creature screen due to long name
   * Warden's plate has the icon overlap the side of the screen
   * Target screen should give priority to targetable enemies, right now it is just by distance. Can also overlap the bottom of the screen
   * A long player name breaks a lot of subscreens in the smallest screen size.

 * Minor fixes
   * Have item notifications not stop auto movement
   * See if you can fix `AL lib: (EE) alc_cleanup: 1 device not closed`, it looks like its not an issue though?
   * Have the fire screen automatically select the last guy you targeted
     * Display this creature in the main interface as though you are mousing over it
   * Do we want PlayerGlyph loading all of the possible glyphs into memory and leaving them all there, or do we want to load them when it changes?

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
