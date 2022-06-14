### **Roadmap:**
 * Dungeon balance overhaul
    * Reflavour the dungeon as an actual dungeon, guards trying to keep you in, you win when you escape
    * We can probably estimate damage by depth and spawn food at points where it will be needed
    * Doors: Cannot be closed again, can handle it as a tile that turns into a floor when bumped into by the player

 * Refine dungeon generation
    * Locked doors that require keys to open
    * You should never get stuck without any keys but with more locked doors
    * Coloured keys, need to get 3 keys in order to leave essentially, each key is held by a tougher guard

 * Music and sound effects

 * Other subscreens:
    * Game Loss and Victory
    * Screen after the [esc] key has been pressed, to not immediately kill the game
    * Settings screen, volume settings and player character colour. Settings should persist in a text file somewhere
    * Keybindings screen in settings

 * Screenshots, Itch page

 * Release prep:
   * It looks like monogame keeps the actual window size independent of Constants.ScreenWidth/ScreenHeight. See if we can scale the window in settings.
   * Rename the game as "Escape of the Blue Man" or something. See if you can change the player colour in settings to rename the window and main menu.
   * See if we can clean up app builds to include no unnecessary dlls
   * Test persistent settings
   * Look into mac and linux deploys

 * **Release!**
