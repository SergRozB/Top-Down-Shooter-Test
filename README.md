There is not an executable, you need to open the project in the unity hub to run it, but here is a link to an unlisted video showcasing the game (content warning: mild pixelated blood after enemies are killed): https://youtu.be/WcmCo1NooA4.

The enemy agents can either patrol set points, wander across random points within a certain range or travel to the last known location of the player. The last known location is decided either by the last location the player was seen at, or the last location the player fired a weapon within range. The agent can also look around the room, either upon reaching the last known location or randomly while wandering. Pathfinding is done using an A star pathfinding plugin.

I created all the assets myself (using Aseprite), including animations. After dying, enemy sprites are removed and replaced with a ragdoll sprite.
