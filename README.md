# About the Game
BRO is a multiplayer online survival arena (MOSA).
Up to eight players compete each other during a fast-paced match.
The player's ultimate goal is to survive during the whole match.
Located on a tight pitch, one randomly chosen player gets assigned with a ball.
Given that ball, a beast chases that player until it's in range to kill the player in possession of the ball.
Over time, the beast gets more accurate and faster.
In order to survive, the player has to pass the ball to other players.
The last man standing wins the match. Besides moving on the battlefield, each player can make use of one of the special abilities.
Such an ability is blinking. The player may teleport to any location on the pitch as long as the ability is not on cooldown.

# About the Project

The motivation behind this academic and non-commercial project is to provide a challenging use-case for most recent deep learning techniques such as deep reinforcement learning.
Using this experimental subject, research on these particular AI fields can be conducted.
For this particular reason, an AI framework and a match automation is featured.
More details about the AI framework and some details about the technical implementation can be retrieved from the wiki.

# Credits

An excerpt of the [PowerBot](https://www.assetstore.unity3d.com/en/#!/content/18136) asset is used inside this project and is only available for **non-commercial use**.
Otherwise, purchase the asset on Unity's asset store.

# Setting up the Source Dependency Photon Bolt

BRO does not make use of Unity's networking APIs.
Instead it makes use of the commercial solution [Photon Bolt](https://www.assetstore.unity3d.com/en/#!/content/83233).
A free version of Photon Bolt should become available soon, according to Exit Games.
Without Photon Bolt, the Unity project is not functional.
These are the steps to import and install Bolt to get started using the project inside of Unity:

1. Download the package using the Asset Store
2. Import the package **except**:

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- project.bytes

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- bolt.user.dll

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- bolt.user.dll.mdb

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- BoltPrefabDatabase.asset

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- BoltRuntimeSettings.asset

3. Install Bolt via the menu "/Assets/Bolt/Install"
4. Restart Unity
5. Compile Bolt via the menu "/Assets/Bolt/Compile Assembly"

![Install Bolt](https://github.com/MarcoMeter/Beastly-Rivals-Onslaught/wiki/images/home/boltInstall.png "Install Bolt")
