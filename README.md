# Umbilicus: Ascension
Re-mixed from [Umbilicus: Descent Team Delta](http://globalgamejam.org/2015/games/umbilicus-%E2%80%93%C2%A0descent-team-delta), 
made for the 2015 Global Game Jam.

## About Umbilicus: Ascension

> You are stranded at the bottom of a pit with limited oxygen.

> You need to cooperate to make your way out.

> But only one of you can be saved.

![Screenshot](<https://raw.githubusercontent.com/sweetcarolinagames/FarscapeFreefall/master/screenshot.png>)

In _Umbilicus: Ascension_ you are part of a team that must escape a pit. You must cooperate
and coordinate with each other (out of game) to carefully maneuver to the top. However,
only one player can actually win the game. 

The entire team is sharing one oxygen tank that depletes over time. Scattered powerups will
replenish your oxygen tank. In addition, each player has a unique ability they can leverage
to help the entire team advance. However, any player can decide to cut the oxygen cord, 
which detaches everyone but also leaks oxygen at a much faster rate. 

How will you reach the top?

### Play Information
This game requires at least two players. If present, this game will attempt to use two gamepads.
By default, the game uses WASD and IJKL controls.

- Horizontal: Move sideways
- Up: Jump
- Action 1: Activate power (E on WASD controls, O on IJKL controls, A on Xbox 360 Controls)
- Action 2: Cut Oxygen Tube (Q on WASD controls, U on IJKL controls, B on Xbox 360 Controls)

## <a name="refOSS">Referenced Open Source Projects</a>
1. [InControl](https://github.com/pbhogan/InControl), an input manager for Unity that
tames the cross-platform controller beast. InControl is not included in this repo.

2. [Umbilicus: Descent Team Delta, n&eacute;e FarscapeFreefall](https://github.com/YoriKv/FarscapeFreefall), 
the code-base we started with. [This game was submit to the 2015 Global Game Jam](http://globalgamejam.org/2015/games/umbilicus-%E2%80%93%C2%A0descent-team-delta).

Note: The original code base depended on [2D Toolkit](http://www.unikronsoftware.com/2dtoolkit/), 
which costs $75. We refactored the code to use [Unity-native 2D tools](http://unity3d.com/pages/2d-power), which are free.

## How the game has changed

As mentioned, _Umbilicus: Ascension_ is a remix of the game [Umbilicus: Descent Team Delta](http://globalgamejam.org/2015/games/umbilicus-%E2%80%93%C2%A0descent-team-delta). Here's a summary of what we changed:

### Changes in gameplay

- In the original game, the game was cooperative amongst all players and the objective was to get as far as possible. In our game, the objective is to reach a specific location and is cooperative up to a point; it requires players to essentially turn against each other far enough from the goal to avoid others getting there first, but close enough to the goal to reach it without dying.
    - We also inverted the direction of travel (the original game requires descent, our game requires ascent) 
- We introduced the oxygen tank mechanic which depletes over time and which begins depleting at a faster rate when either: a) you cut the oxygen cord, or b) you lose someone from the team (which cuts the cord in a different way). When the oxygen runs out, all players lose.
- We changed the physics of the environment to give the world a weaker gravity, which requires more precise movement and (as a consequence) more off-screen coordination.
- Existing in-game powerups were modified to serve the new oxygen tank mechanic. Whereas before they used to just give you points, they now replenish your oxygen tank.
- We removed in-game geysers (which blasted random gusts of wind, throwing players off) and crumbling blocks (standing too long on a block causes it to decay) from the original in an attempt to make the game more tactical.

### Other changes

- In the original game, you couldn't select the astronaut you wanted to use since it was assigned as part of your player number (player 1 is always the White-suit astronaut, player 2 is always the Yellow-suit astronaut, player 3 is always the Blue-suit astronaut, and player 4 is always the Red-suit astronaut). In our game, you can pick which astronaut you'd like to play as.
- Tried to add a bit of narrative framing to motivate and set the tone of the overall gameplay.
- Added free/creative-commons sound effects as well as UI elements to support the interactions with the new game mechanics.
- As mentioned in the [Referenced Open Source Projects section](#refOSS), we removed/refactored references to the [2D Toolkit](http://www.unikronsoftware.com/2dtoolkit/) in favor of using [Unity's 2D tools](http://unity3d.com/pages/2d-power).

## Team

* [Sweet Carolina Games](http://sweetcarolinagames.com)
* Developed by:
  * [Ian Coleman](http://twitter.com/iancoleman) - Game Design & Development
  * [Rogelio E. Cardona-Rivera](http://twitter.com/recardona) - Game Design & Development

## License

All original content developed is Copyright 2015 Sweet Carolina Games (Ian Coleman & Rogelio E. Cardona-Rivera)

## Known Issues

- High Scores don't update correctly. 
