### **Roadmap:**
 * Equipment
    * Update the player glyph based on weapon type. In the future we can add combinations of weapon and armor

 * Add a speed stat to balance armors
    * Otherwise all armor is functionally identical. The armor with more defense is strictly better, all the time
    * Each in game "tick" will increase a counter for each creature, once their counter reaches their turn delay then they will take a turn and reset their counter.
    * This isn't super clear to players why enemies sometimes take an extra turn every so often, particularly without animations, if they move two tiles for example it will look like a bug.

 * Add very simple animations
    * If the player can see the current creatures turn, wait a set amount of time before they act.
    * Add a "wait" field in main that decrements until it hits 0 before proceeding.

 * Screen to inspect creatures, items
    * Right click them
    * Other subscreens: Help screen, better start screen

 * Ranged Attacks
    * Mousing over an enemy will draw a line to them, clicking them will attack them
    * Clicking tiles should path you towards them until an enemy is in sight (then just move you 1 tile at a time)

 * An actual win condition
    * Have a start and end room in the dungeon
    * Reflavour the dungeon as an actual dungeon, guards trying to keep you in, you win when you escape
    * Food should be harder to come by

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
