# Exposed
*The First Ceiling Tile Type Horror Game*
___

## High concept statement
You are hired to do maintenance on an abandon building at night. The ceiling tiles keep falling out and you need to keep fixing them before they fall out of the ceiling. Can you keep the ceiling whole, even with when what comes out of it might haunt you?

## Gameplay loop
Whack-a-mole but with ceiling tiles. On later nights, some ceiling tiles will not be repairable and enemies will come out of the ceiling. You have to manage these enemies as well as the ceiling falling down. 

## Enemy types
### Raccoon
The Raccoon will slowly approach the trashcan in phases. It's last phase will be the shortest, so the player will have to pay closer attention to the Raccoon when it is in its second to last phase. If the raccoon jumps into the trashcan, the trashcan will fall over, hitting a light switch on the way down, making it impossible to see the ceiling tiles or do your job.

Raccoon level represents how quickly it progresses through these phases.

### Cat
The cat falls from the ceiling, and will randomly pounce on exposed ceiling tiles. As a cat is hanging on a ceiling tile, it will fall in half the time.

Cat level represents how early the cat spawns and how often it will pounce on tiles.

### Goose
Kind of crazy idea, but baby geese have a chance of falling from the ceiling, if this happens, the mother goose will fly up and catch the baby, breaking all ceiling tiles below it. This is an insane idea and idk if I will implement it lol.

## Difficulty
Inspired by FNAF, each animal will have a difficulty parameter that can range from 0-20, 0 denoting not present and 20 denoting maximum difficulty.
The ambient level itself will also have a difficulty value that represents how frequent ceiling tiles fall. 