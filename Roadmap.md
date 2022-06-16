### **Roadmap:**
 * Dungeon balance overhaul
    * Armor types hinder movement and attack speed the heavier they get
      * Set armor to display as something that isn't blue hearts, to make it a little more dynamic. Blue hearts plus bonus ticks or something?
    * Include a heart crystal (bloodstone?) that gives you an additional 4 HP when you pick it up. These should spawn at the last depth of the early dungeon and the last depth of the mid game dungeon
    * Make items dropped on the ground try not to overwrite the tile they land on, attempt to place it on any adjacent tile first
    * Enchanted weapons? Give bonus damage and are a different colour, to bridge the gap between depths if that feels unbalanced

 * Music and sound effects

 * Other subscreens:
    * Game Loss and Victory
    * Screen after the [esc] key has been pressed, to not immediately kill the game
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

 * Music and sound effects

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
