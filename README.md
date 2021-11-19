# LinkedList snake

School assignment to create a Snake game using our own implementation of a Linked List. 

# Gameplay mechanics

## Normal Game Mode

In the "Standard" mode we play a regular game of snake. We're allowed to move through the edges to get to the other side of the map. Every 5th food we eat will spawn in two random walls that will make the play area smaller and more difficult to traverse. We also get a slight speed-up every time we eat food.

## Pathfinding Game Mode

In this mode the Snake moves by itself using the A* pathfinding algorithm to find a path to where the food has spawned. Once the food has been taken we check for a new path to wherever the food re-spawned at. If there is no valid path then the snake will try to find an empty tile and move towards it. Currently it does this in order LEFT, RIGHT, UP, DOWN. If no path is available we get a game-over screen.

Future improvements on this would be to try and figure out a better way for the snake to move when it can't get a direct path to the food. The current implementation of trying to go Left, right, up or down is kind of "dirty" in the sense that there are definitely a better path than to just randomly go in these directions.

To change the movement speed of the snake go to the SnakeAstar object in the scene / prefab and change the "Time between moves". The snake gets gradually faster, so if you want to lock it to a slower speed also change the "Min time between moves" to the same value. I'd recommend not going under 0.03 as I've noticed some issues with triggering the "OnTriggerEnter" on the foods at these speeds.

## In editor

To see a visualization of the path and the blocked nodes you can go to "Scene"-mode while the pathfninding is running. Green nodes are all the walkable tiles, red nodes are the blocked tiles (where the tail is) and grey nodes is the current path.

![image](https://user-images.githubusercontent.com/33944159/142617754-b3620999-737b-4e42-8206-33e9da9a3f23.png)
