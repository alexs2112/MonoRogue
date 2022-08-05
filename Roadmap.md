### **Roadmap:**
 * Center the main display on the player, don't let them move to the far left or right of the screen based on world size
   * Gives away your relative position in the dungeon and looks really bad on bigger screen sizes

 * Flavour Changes:
   * Change colour of Thug once they start to run away to make it more clear
   * Set summoned imps to have the "ability" that they die when their summoner is killed
   * Add a * next to the score in history if that run was an escape
      * Sort history by Greatest Score > Lowest Time > Lowest Date
   * Set note in user manual about the flame icons next to an enemies name to indicate difficulty

 * Minor fixes
   * See if you can fix `AL lib: (EE) alc_cleanup: 1 device not closed`, it looks like its not an issue though?
   * It appears loading is broken if you attempt to load after returning to the main menu (game end, probably not a problem since the save should be deleted here)
   * Messages arent cleared if you start a new game from the end screen
   * If the player tries to pathfind far away the game crashes due to the pathfinding limits, disable the pathfinding max for the player
   * Let all menus wrap around

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
