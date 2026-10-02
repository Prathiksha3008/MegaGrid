# MegaGrid
A prototype Unity game featuring minimalist platforming with a color twist of action

Members: John Tu, Yen (Robert) Chiang, Prathiksha Ravibabu Nijamkari

# Overview and How to Play:
Given a grid level of platforms of any size, the goal is to survive and dodge incoming waves of red platforms until the timer runs out. Once the timer runs out, a golden platform will appear anywhere, and the player can touch that platform to finish the level.

When the incoming wave of red platforms will appear, any row or column will have the blue platforms highlighted yellow to warn the player of the incoming wave. As the timer is about to approach zero, more than one row and/or column can be selected at a time, making the level more difficult.

The player starts with 3 lives, and each time the player touches a red platform, one life is decreased. Once all lives run out, the player gets a game over, and the level restarts.

The color codes of all platforms are listed below as followed, and whether it is safe or not for the player to cross:
- Green: safe; randomly placed anywhere on the grid, and cannot be replaced by red platforms
- Blue: safe; any row or column will be replaced by incoming waves of red platforms
- Red: unsafe; must be avoided at all costs
- Yellow: safe; warns the player about the incoming red platform wave
- Gold: goal; finishes the level

## Game Controls:
WASD and/or arrow keys to move

Spacebar to jump
