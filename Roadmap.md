### **Roadmap:**
 * Other subscreens:
    * Map screen to show a much larger portion of explored tiles that you can scroll around to view.
      * Highlight regions by difficulty here
    * The remaining help screens, multiple menus to swap through to see info about the game

 * Persistent settings
    * Settings screen, volume settings and player character colour. Settings should persist in a text file somewhere
      * Changing player colour changes game name. Set gender as well. "Escape of the Yellow Person"
    * Keybindings screen in settings <- Not sure if we have enough keybindings for this lol, just map all of the movement controls
    * Pretty up the window resize screen. Maybe disable screen sizes that are too big if possible, seems to mess things up if you set it to bigger than your physical screen and in fullscreen mode. If we can't get monitor size (niche case of external monitor) maybe add a warning like "This window size is greater than your detected monitor size of ___, proceed with caution"
      * Maybe a "Fit to Screen" button that will match it to our resultion and enable full screen (which can then be turned off but might be ugly)

 * Animate projectiles
    * Same as movement, could have multiple projectile sprites based on what is shooting them

 * Make messages not overflow the interface if we get a lot of them, maybe consolidate attack messages? "X, Y, and Z attack you for <combined> damage"

 * Dungeon features and fancier rooms, make the dungeon look a little better...
    * Load pregenerated dungeon vaults when generating the dungeon, this could get a little tricky with hallways
    * Mousing over tiles will show their description in the main interface, similar to items
    * Add more features

 * Music and sound effects
    * Main menu music
    * Main game music
    * Subscreen music
    * Don't think we need sound effects, they seem tacky in a roguelike with no animations

 * Look into enabling saving and persistency
    * I think we can just copy down the world seed, regenerate the entire world. Copy the list of remaining creatures and items and their locations and equipment and stuff and then place them where needed. Could be easy, don't waste too much time on this if it won't work

 * Release prep:
   * Fix all @todo tags
   * See if we can clean up app builds to include no unnecessary dlls without breaking
   * Test persistent settings
   * Look into mac and linux deploys
   * Make sure each item and creature have non-temp description text
   * Make sure the user manual is up to date and has corresponding pages that can be viewed from the help menu in game. Probably compile it into a pdf when distributing.
   * Make sure all in game terminology is correct
   * Screenshots, GameJolt page, Itch page, Itch+GJ profile

 * **Release!**
