### **Roadmap:**
 * Dungeon balance overhaul
    * When standing over an item, write that item under your player stats so you can compare
    * Make items dropped on the ground try not to overwrite the tile they land on, attempt to place it on any adjacent tile first
    * Enchanted weapons? Give bonus damage and are a different colour, to bridge the gap between depths if that feels unbalanced
    * Add more creatures, I think 4 for each dungeon tier will be fine
    * Add a win condition, standing on the exit in the last room and pressing space will win you the game
      * Make a boss enemy that also spawns in the end room that holds a key, you cant leave without the key
      * The most hearts that will fit on the main interface is 9... The player should have 5 by the end of the game

 * Music and sound effects

 * Other subscreens:
    * Game Loss and Victory
    * Settings screen, volume settings and player character colour. Settings should persist in a text file somewhere
    * Keybindings screen in settings
    * Screen to inspect tiles on right click
    * Map screen to show a much larger portion of explored tiles that you can scroll around. Center it on what we can see without giving hints as to what region of the full world you are in.

 * Animate projectiles
    * Same as movement, could have multiple projectile sprites based on what is shooting them
    * Might not be doable since it locks you out of input for a handful of frames, worth looking into

 * Tab to auto attack nearest enemy in range, to avoid having to repeatedly use the mouse when ranged
    * A fire key of some sort ([f]?) to let you select an enemy from available enemies in sight to shoot at, default to pointing at the one you last attacked, or the nearest one with the lowest health
    * Right side of the fire screen should just show a list of enemies in sight with their health. You can cycle through them with direction input (or just a click)

 * Screenshots, GameJolt page, Itch page, Itch+GJ profile

 * Release prep:
   * It looks like monogame keeps the actual window size independent of Constants.ScreenWidth/ScreenHeight. See if we can scale the window in settings.
   * Rename the game as "Escape of the Blue Man" or something. See if you can change the player colour in settings to rename the window and main menu.
   * See if we can clean up app builds to include no unnecessary dlls
   * Test persistent settings
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.

 * **Release!**
