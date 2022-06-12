### **Roadmap:**
 * Dungeon design upgrade
    * Have some rooms be rocky and part of a cave, change up tiles to look different and cooler
    * Essentially a cave with random stone rooms
    * Rooms can store what type they are, so we can spawn different enemies based by room
    * Need a way to make this natural looking
    * Algorithm:
        * Mark each partition as either CAVE or ROOM
        * Randomly place floor and wall tiles. ROOM partitions should be biased towards walls, CAVE partitions should be biased towards floors
        * Run cellular automata to smooth each CAVE partition. Draw a border of walls around the map after each smooth iteration so we don't accidentally leave a gap.
        * Run a flood fill algorithm to determine where the resulting rooms are. Fill rooms that are too small
        * Handle the ROOM partitions the same way we are doing now. Instead of marking just floors, we will need to mark floors and the wall surrounding it. Need to keep track of where rooms are so we can draw those tiles as the dungeon tiles, instead of cave tiles.
        * Draw hallways the same way we are doing now. Hallways that draw between two ROOM partitions should also be marked to use dungeon tiles. Otherwise use cave tiles (perhaps with a handful of random dungeon floors).
    * Doors: Cannot be closed again, can handle it as a tile that turns into a floor when bumped into by the player

 * Dungeon balance overhaul
    * Keep track of each room as room objects
    * Have a start and end room in the dungeon. Rooms should have a depth stat to keep track of how deep they are, so you can scale the difficulty and loot the deeper into the dungeon you go
    * Reflavour the dungeon as an actual dungeon, guards trying to keep you in, you win when you escape
    * We can probably estimate damage by depth and spawn food at points where it will be needed

 * Animations for movement, combat and deaths
    * Movement and attacks can store an X and Y offset to creatures, when an animation is playing you just rapidly change the offset
    * This requires redoing the worldview
    * Don't take player input while an animation is playing, an option to turn it off in the settings (disable animations)

 * Refine dungeon generation
    * Locked doors that require keys to open
    * You should never get stuck without any keys but with more locked doors
    * Coloured keys, need to get 3 keys in order to leave essentially, each key is held by a tougher guard

 * Resize the screen size, although the 800 x 480 terminal is growing on me

 * Music and sound effects

 * Settings screen with persistent settings

 * Screenshots, Itch page

 * **Release!**
