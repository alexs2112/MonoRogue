### **Roadmap:**
 * Bugs:
   * Music doesn't go to the next song again...
   * Bug with loading saved games due to cooldowns, hard to reproduce
   * Compile time takes forever?

 * Some Fixes:
   * Make enemy count equal to the log of room size, vs some weird static value we have now
      * System.Math.Log(value, base=4)
   * History should prioritize victories
   * Keep track of game version in the save, display it on the start screen

 * Save on End:
   * Make sure closing the game saves it
   * If you are at 0 HP, save history and delete the game save
     * Escape screen should possibly reflect this, you shouldn't be able to navigate to escape when dead though
   * Dying or winning the game should delete your save
     * Do this in main, so we don't need to rely on moving to another screen
     * Need to make sure this doesn't save the game again, since it will call `OnExiting` in main with `Player` being defined.
        * A hack is to just remove the player after game history is set in main

 * Release prep:
   * See if we can clean up app builds to include no unnecessary dlls without breaking
   * Test persistent settings and savegames across different devices
   * Look into mac and linux deploys
   * Compile the user manual into a pdf to distribute
   * Make sure all in game terminology is correct
   * Screenshots, GameJolt page, Itch page, Itch+GJ profile, RogueBasin page

 * **Release!**
