### **Roadmap:**
 * Keep track of the last enemy you attacked. Should display them in the main interface unless you mouse over a different creature
   * Target screen should start with them selected

 * Interface bugs
   * Text in the creature screen can overlap on small screen sizes, 3+ lines of description and critical/parry chance will overlap.
   * Gatekeeper icon will overlap with equipment in creature screen due to long name
   * Warden's plate has the icon overlap the side of the screen
   * Target screen should give priority to targetable enemies, right now it is just by distance. Can also overlap the bottom of the screen
   * A long player name breaks a lot of subscreens in the smallest screen size.
   * Messages are overflowing on small screen sizes again

 * Minor fixes
   * Have item notifications not stop auto movement
   * See if you can fix `AL lib: (EE) alc_cleanup: 1 device not closed`, it looks like its not an issue though?
   * Do we want PlayerGlyph loading all of the possible glyphs into memory and leaving them all there, or do we want to load them when it changes?
   * It becomes really slow later and laggy in the game after sound effects were added, not sure what is up. They are all stored in memory but total < 600KB, way less than a single music file. Seems to come up after a shout is issued to a lot of enemies
   * Have pigs and mice not play the alarm sound effect when they make noises
   * It appears loading is broken if you attempt to load after returning to the main menu (game end, probably not a problem since the save should be deleted here)
   * Messages arent cleared if you start a new game from the end screen
   * Index in history screen doesnt work

 * Release prep:
   * Fix all @todo tags
   * Make sure that scrolling through game history actually works
   * Make sure closing the game saves it
    * If you are at 0 HP, save history and delete the game save
   * Dying or winning the game should delete your save
   * See if we can clean up app builds to include no unnecessary dlls without breaking
   * Test persistent settings and savegames across different devices
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.
   * Make sure all in game terminology is correct
   * Screenshots, GameJolt page, Itch page, Itch+GJ profile, RogueBasin page

 * **Release!**
