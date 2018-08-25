 ## TileMap experiments
 This project will be about leveraging a lot of what I've learned in the past to move from a past of 3rd party sprite based AStar to using a pathing system that will be more generic, and at the end we will be able to use it in any other project we want.

 ### Goals
 - Leverage Unity's Hexagon Tilemap eventually
 - Integrate an AStar system that will be sufficiently generic to reuse in other projects.
 - Help anyone else who struggles with this. 
    - As I was learning Astar it was very hard to find enough examples for me to grok it. I want to give back to others.
- Convert the AStar & Tile system to use ECS for extra performance
    -  The tile data is already a simple data class so it should be easy to convert this to an ECS datastructure or a hybrid ECS system




#### Credits
- Quill18 - https://github.com/quill18/MostlyCivilizedHexEngine
- BlueRaja - https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp