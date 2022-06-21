### **Roadmap:**
 * Music and sound effects

 * Other subscreens:
    * Settings screen, volume settings and player character colour. Settings should persist in a text file somewhere
    * Keybindings screen in settings
    * Screen to inspect tiles on right click
    * Map screen to show a much larger portion of explored tiles that you can scroll around. Center it on what we can see without giving hints as to what region of the full world you are in.
      * Highlight regions by difficulty here

 * Animate projectiles
    * Same as movement, could have multiple projectile sprites based on what is shooting them

 * Screenshots, GameJolt page, Itch page, Itch+GJ profile

 * Release prep:
   * Fix all @todo tags
   * It looks like monogame keeps the actual window size independent of Constants.ScreenWidth/ScreenHeight. See if we can scale the window in settings.
   * Rename the game as "Escape of the Blue Man" or something. See if you can change the player colour in settings to rename the window and main menu.
   * See if we can clean up app builds to include no unnecessary dlls
   * Test persistent settings
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.
   * Make sure all in game terminology is correct

 * **Release!**
