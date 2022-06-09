### **Roadmap:**
 * Equipment
    * One weapon slot and one armor slot. Add armored hearts, which are essentially just normal hearts but regenerate over time
        * Framework is set up for this, not implemented yet
    * Update the player glyph based on equipment
    * Equipment can be revamped later to be more interesting, for now only have armored hearts and damage
        * Such as different weapon types functioning differently, armor can regen at different speeds
        * Don't want it to be a boring "bigger numbers better" game with limited inventory

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
