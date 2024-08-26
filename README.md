This is my first version of a base engine in MonoGame.

It contains structures such as GameObjects, Colliders, Layering, Scenes, Aseprite Tilemap support, as well as an automatic tilemap collider creator.

It's far from being good, and bugs are sure to arise still, but for me, it still serves as a good starting point, from which I can persoanlly continue to work and improve on.

How it's set up:
GameObjects form the base of all rendered objects. It holds all the important components, you would need in order to create any interacting class, like a player, boxes, traps or enemies.
The most important components inside the GameObject, are the Sprite component, as well as the Collider component. The Sprite component ensures that the GameObject is actually visible, while the Collider component handles the collision part for it.

How to render an Aseprite tilemap, with coherent colliders:
Firstly, you need to add an Aseprite tilemap to the mgcb content manager. With that, you can use the Map class, in order to load the file. Examples of how to render and create levels with that, are provided inside the folder Game/Levels.
There are two different types of applying colliders for your created tilemap. The map class checks the names of the layers created. Be wary, that it's not allowed to have the same name for two different layers.
If the layer name contains "(Collider)", it will create a single, rectangle collider covering the whole tile.
Else, if the name contains "(PerfCollider)", it will create a pixel-perfect collider for that tile, which is a mix of multiple colliders.
For all other cases, it will just be rendered normally, with no colliders at all.

Layering:
Right now, the GameObject with the higher position, will be rendered ontop of the other object. As it takes the actual position of the object, which is currently at the top left of the rendered sprite, instead of the position of the feet, which makes it will look a bit off still.
To correct that, one could use the bottomCollider's position instead, rendering the layers based on the position of the feet instead, resulting in a more logical layering.
This is also something I plan on adding in the future, as soon as I put this engine to actual use.
