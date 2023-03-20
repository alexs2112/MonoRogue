### **Roadmap:**
 * Release prep:
   * Test persistent settings and savegames across different devices
   * Look into mac and linux deploys
   * GameJolt page, Itch page, Itch+GJ profile, RogueBasin page
 * **Release!**

### Seed Bug
 * Define a game session where one session consists of multiple games played to completion (start new game, die, go to main menu, start a new game)
 * Somehow the worlds with the same seed are being generated slightly differently between the first game of a session, and every prior game
   * Each prior game is being generated the same way, the difference is only with the first game of each session, and prior games seem to be dependant on the seed of the first game
 * I can't figure this out. Removing the ability to return to the main menu from the end screen for now.
 * Fixing this is a stretch goal
