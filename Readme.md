# SimUniversity

*Variance of the board game Settlers of Catan, with a University/Campus theme.*

Idea from the project in COMP1711 UNSW CSE computing course in 2002, lecturer Richard Buckland.

### Difference from the original Settlers of Catan game:
* No player trading (yet)
* No robber
* No development cards, instead players can found startup companies with a 20% success rate

### Difference from the original COMP1711 SimUniversity game
* Resource are Wood, Brick, Ore, Grain and Sheep (as student degrees are too hard to remember)
* Acquire students from the 2nd campus in the setup phase (same as Catan)


### TODO:
* Not to generate AllMoves in TurnInfo, as well as from the AIs
* Write tests for the game play
* Put all reference DLLs into a lib folder, instead of using NuGet?
* Enable deep copy of game state (so AI cannot break the actual game)

### Tidy up:
* Remove game state hashing
