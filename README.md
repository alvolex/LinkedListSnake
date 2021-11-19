# LinkedList snake

School assignment to create a Snake game using our own implementation of a Linked List. 

# Gameplay mechanics

## Normal Game Mode

In the "Standard" mode we play a regular game of snake. We're allowed to move through the edges to get to the other side of the map. Every 5th food we eat will spawn in two random walls that will make the play area smaller and more difficult to traverse. We also get a slight speed-up every time we eat food.

## Pathfinding Game Mode

In this mode the Snake moves by itself using the A* pathfinding algorithm to find a path to where the food has spawned. Once the food has been taken we check for a new path to wherever the food re-spawned at. If there is no valid path then we get a game-over screen.

Future improvements for this mode would be to keep moving even if there is no path as long as there are open tiles where we can move to, then continuously check if a path to the food becomes available.
