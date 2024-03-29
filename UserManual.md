# MonoRogue

**Welcome to the dungeon!**

Escape of the Blue Man is a lightweight and short classic roguelike. Your goal is to explore the randomly generated world, collecting items, defeating enemies, and eventually making it to the exit. This user manual is a help document to explain some of the concepts that you will need to know in order to survive. The game is meant to be short and simple, for beginners of the roguelike genre.

## Controls:
 - Arrow keys, Left Click, Numpad, or Vi Keys to move and attack
 - Spacebar or click yourself to interact with items on the ground
 - Right click creatures or items to see their stats
 - `[s]` to see your own stats
 - `[r]` to rest and repair armor
 - `[.]` to wait one turn
 - `[f]` to fire your weapon at visible enemies in range
 - `[c]` to close all adjacent, unbroken doors
 - `[m]` to view your world map, regions are colour coded by difficulty
 - `[x]` to examine specific tiles without using the mouse
 - `[/]` or `[?]` to show the help menu
 - `[esc]` to quit the game or exit subscreens

## Creature Stats:
Each creature has several stats that are important to understand.
* Health: Takes the form of red hearts. Each red heart is equal to 4 health. When this reaches 0 you die!
* Defense: Takes the form of blue hearts and is granted by Armor (some creatures have innate defense). It acts as bonus health that will regenerate out of combat. Pressing `[r]` will let you wait and regenerate your Defense while enemies are out of sight.
* Attack Delay: How many in-game ticks it takes before you can make another action after attacking. The higher the value, the slower your attacks.
* Movement Delay: Same as attack delay, but how long it takes to make another action after moving. A creature with movement delay 5 will get to move two tiles for every one tile of a creature with movement delay 10.

Enemies also have a difficulty rating, depicted by a flame icon next to their name when you mouse over them. The difficulty of the dungeon increases as you progress deeper into it.

## Food:
Food is an item type that can be found in the dungeon, or dropped by pigs. Pressing `[space]` or clicking your character when standing on food will consume it, regenerating your Health equal to the Yellow Heart value of the food.

Food is the only way to heal your character's health in the dungeon.

## Weapons:
There are 6 different weapon types that you can equip. Each functions slightly differently to add some variety to gameplay.
* Swords have a chance to parry incoming attacks. Parry chance decreased by armor weight.
* Spears have increased range. Moving towards an enemy in range lets you "lunge" to attack them for bonus damage.
* Axes hit all enemies adjacent to you whenever you attack. They regenerate your defense for each enemy struck beyond the first.
* Daggers lower your attack delay and can critically hit. Critical hit chance decreased by armor weight.
* Maces deal bonus damage to enemies with defense and deal bonus damage on subsequent attacks.
* Bows have extended range but attack slower than other weapons.

## Armor:
Armor has three stats: Defense, Weight, and Block. 

* Defense serves to increase player survivability in the form of Blue Hearts. Blue hearts function similarly to regular hearts, but they regenerate outside of combat. 

* Armor Weight increases your move delay and attack delay, slowing you down. Heavier armor has higher defense, but also tends to have higher weight.

* Block decreases the amount of damage you take by a flat amount. Only found on heavy armor.

There are three types of armor:
 
* Light Armor: Has no weight but does not provide much protection. Best used with bows and daggers as the lack of weight allows you to run away from melee enemies and make multiple attacks against slower ones.

* Medium Armor: Has a little bit of weight but provides moderate protection. Best used with swords and spears as melee weapons that want the defense, but don't want too much weight for parrying attacks or kiting enemies.

* Heavy Armor: Has the greatest weight, but the highest defense. Lowers the amount of damage enemies will deal to you with Block. Best used with axes and maces as heavy hitting melee weapons that benefit the most from being in close combat.

## Saving the Game and Settings:
Settings can be set through the `[escape]` menu and will save once you press `Continue`. These will be saved in a file called `BlueSettings.settings` that should be in the same folder as the game executable.

The game will be saved on exit. This will also be in the same folder as the game executable in a file called `BlueSave.savegame`.

Game history will be saved in a file called `BlueHistory.morgue`. This keeps track of certain stats from previous games, to record your scores.

If you move the game executable somewhere where it is not in the same folder as the save file and the settings file, it will replace each one with a fresh file.

## Enemies:
This section will give a bit of info about each enemy. Contains spoiler material if you have not made it to the end of the game yet.
<details>
<summary>Rat</summary>
Weak early game creature, moves and attacks quickly.
</details>

<details>
<summary>Pig</summary>
Not aggressive until it is attacked. Sometimes drops food on death.

Oinks
</details>

<details>
<summary>Spawn</summary>
A basic early game enemy, has similar speed to the player and attacks without a weapon.
</details>

<details>
<summary>Undead</summary>
A mildly tanky enemy that moves and attacks very slowly.
</details>

<details>
<summary>Grunt</summary>
A basic enemy in the middle of the dungeon. Moves relatively quickly compared to its brethren.
</details>

<details>
<summary>Imp</summary>
Mystical creature. It is ranged, but deals little damage.
</details>

<details>
<summary>Thug</summary>
An armored enemy with a polearm. When it runs out of defense it will attempt to run away from the player if the player does not have a ranged weapon.
</details>

<details>
<summary>Brute</summary>
An enemy covered with spines. It deals damage to you when you hit it with a melee weapon and it still has defense.
</details>

<details>
<summary>Haunt</summary>
A late game ranged enemy. It deals moderate damage at range, but moves and attacks very slowly.
</details>

<details>
<summary>Gatekeeper</summary>
Late game melee enemy that spawns with equipment. Can be looted after death.
</details>

<details>
<summary>Tank</summary>
A slow enemy with plenty of health and defense. It will pull you towards it if you are nearby.
</details>

<details>
<summary>Cultist</summary>
Late game enemy that can summon up to two imps. Killing it will kill off its summons.
</details>

<details>
<summary>Warden</summary>
Holds the golden key and guards the exit of the dungeon. When he sees you he can alert nearby dungeon enemies of your location, drawing them to stop your escape.
</details>

## Known Bugs
Occasionally the soundtrack will go silent when it changes songs. This can be fixed by entering a menu with `esc` before returning to the main game screen.

## Credits:
Developed by myself, Urist2112. https://urist2112.itch.io

Written using the C# MonoGame framework. https://www.monogame.net/

A large portion of the tileset is from https://www.kenney.nl

Background soundtracks from https://opengameart.org/content/game-game
