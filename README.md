# AI Steering & Procedural Content Generation Simulation

A basic simulation of AI navigating through a procedurally generated field of obstacles using steering behaviour. Properties of the simulation are changeable (such as # of agents and objects generated) by modifying the value fields in the Obstacle Generator and Game Manager game object fields. Properties about the agents are modifiable in their Prefabs.

Red agents are travellers and attempt to navigate from the blue doorway on the right to one of the orange doorways on the left. If they cannot reach their destination in a specified amount of time they switch target doorways.

Green agents are Wanderers and will randomly wander the level. Should a red agent come near, they will try to intercept the red agent's path by blocking them from their destination.

Yellow agents are socialites. They currently only randomly wander the level and do not interact with/are not influenced by other agents.
