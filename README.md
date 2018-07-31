# Master Server for DESYSIA
========

```
    ___                              _                   
   /   |  _____________  ____  _____(_____  ____         
  / /| | / ___/ ___/ _ \/ __ \/ ___/ / __ \/ __ \        
 / ___ |(__  / /__/  __/ / / (__  / / /_/ / / / /        
/_/ _|_/____/\___/\___/_/ /_/____/_/\____/_/_/_/         
   / | / ___  / /__      ______  _____/ /__(_____  ____ _
  /  |/ / _ \/ __| | /| / / __ \/ ___/ //_/ / __ \/ __ `/
 / /|  /  __/ /_ | |/ |/ / /_/ / /  / ,< / / / / / /_/ /
/_/ |_/\___/\__/ |__/|__/\____/_/  /_/|_/_/_/ /_/\__, /  
                                                /____/   
¸,ø¤º°`°º¤ø,¸¸,ø¤º° [я๏ţţ€ɲ ˅ɨ$ɨ๏ɲ$ 2015] °º¤ø,¸¸,ø¤º°`°º¤ø,¸
```

## Homepage

http://rottenvisions.com

## What is Master Server?

![Main](https://raw.githubusercontent.com/RottenVisions/masterserver/master/readme-src/main-window.png)

![Output1](https://raw.githubusercontent.com/RottenVisions/masterserver/master/readme-src/output-1.png)

![Output2](https://raw.githubusercontent.com/RottenVisions/masterserver/master/readme-src/output-2.png)

A server with a console instance that serves a list to all clients who request it. Servers automatically register with the Master Server based upon chosen parameters upon server creation. If the server is set to public, it will be saved in a list that can be queried by any client. This allows all players to connect to any registered server available.

Server has a special windows console instance that allows for commands to be used.

## Instructions

Turn on **.NET 2.0** in Unity (do not use *.NET 2.0 Subset*). The console requires it. This is found in Player Settings

Turn on **'Run in Background'**. It is needed for the server to run while not in focus. This is found in Player Settings.

Replace console & tools with their latest versions.

## Commands

```
<<<Available Commands>>>
<-------------------------->
NOTE: [command2 | command3 | command4] signifies alternate methods of commands
Example: command1 will also do the same as the main command, as will command2
<-------------------------->
help - Shows the Help Menu [cmds | commands]
start - Starts / Initializes the Master Server [initialize]
reset - Resets the Master Server
quit - Stops the Master Server
list - Lists all the registered servers of the Master Server [stop]
setport - Sets the listening port of the Master Server
listport - Lists the current listening port of the Master Server [port]
```
