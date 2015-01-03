INonSharedRegion Module -  ability to control flocks of prims within an OpenSim scene.

To build from source

Add OpenSimBirds source tree under opensim/addon-modules

./runprebuild.sh against opensim root to build this module into the solution
then xbuild, or build within Visual Studio / Monodevelop to produce the binaries.
Remember you need an .ini file in bin/addon-modules/OpenSimBirds/config/

OpenSimBirds has no external dependencies other than the dlls currently included in opensim.
The project generates a single dll - OpenSimBirds.Module.dll which is copied into opensim/bin as part of the build step


Configuration

To become active, the module needs enabled in the ini file. Otherwise it does nothing on startup.
If you are running multiple regions on one simulator you can have different Birds
settings per region in the configuration file, in the exact same way you can
customize per Region setting in Regions.ini

The configuration file for this module should be placed in:

bin/addon-modules/OpenSimBirds/config/OpenSimBirds.ini

and follows the similar format as a Regions.ini file, where you specify setting for
each region using the [Region Name] section heading. There is an example .ini file
provided which should be edited and copied to the correct place above.

Here is an example config:

	;; Set the Birds settings per named region

	[Test Region 1]

		BirdsEnabled = True         ;set to false to disable the module in this region	
		BirdsFlockSize = 50         ;the number of birds to flock
		BirdsMaxFlockSize = 100     ;the maximum flock size that can be created (keeps things sane)
		BirdsMaxSpeed = 3           ;how far each bird can travel per update
		BirdsMaxForce = 0.25        ;the maximum acceleration allowed to the current velocity of the bird
		BirdsNeighbourDistance = 25 ;max distance for other birds to be considered in the same flock as us
		BirdsDesiredSeparation = 20 ;how far away from other birds we would like to stay
		BirdsTolerance = 5          ;how close to the edges of things can we get without being worried
		BirdsBorderSize = 5         ;how close to the edge of a region can we get?
		BirdsMaxHeight = 256        ;how high are we allowed to flock
		BirdsUpdateEveryNFrames = 1 ;update bird positions every N simulator frames
		BirdsPrim = seagull01       ;By default the module will create a flock of plain wooden spheres, 
		                            ;however this can be overridden to the name of an existing prim that
		                            ;needs to already exist in the scene - i.e. be rezzed in the region.	



Various runtime commands control the flocking module behaviour - described below. These can either be invoked
from the Console or in world by directing them to a chat channel either from the client's Local Chat or via a script.
You can specify which channel to use in the .ini:

		BirdsChatChannel = 118 	the chat channel to listen for Bird commands on



Runtime Commands

The following commands, which can be issued on the Console or via in-world chat or scripted chat on the BirdsChatChannel
to control the birds at runtime:

	birds-stop or /118 stop                         ;stop all birds flocking 
	birds-start or /118 start                       ;start all birds flocking
	birds-enable or /118 enable                     ;enable the flocking simulation if disabled
	birds-disable or /118 disable                   ;stop all birds and remove them from the scene
	birds-prim <name> or /118 prim <name>           ;change the bird prim to a prim already rezzed in the scene
	birds-stats or /118 stats                       ;show some statistics (needs work)
	birds-framerate <num> or /118 framerate <num>   ;only update the flock positions every <num> frames
	                                                ;only really useful for photography and debugging bird
	                                                ;behaviour


These commands are great for playing with the flock dynamics in real time:

	birds-size <num> or /118 size <num>             ;change the size of the flock
	birds-speed <num> or /118 speed <num>           ;change the maximum velocity each bird may achieve
	birds-force <num> or /118 force <num>           ;change the maximum force each bird may accelerate
	birds-distance <num> or /118 distance <num>     ;change the maximum distance that other birds are to be considered in the same flock as us
	birds-separation <num> or /118 separation <num> ;sets how far away from other birds we would like to stay
	birds-tolerance <num> or /118 tolerance <num>   ;sets how close to the edges of things can we get without being worried

Of course if distance is less than separation then the birds will never flock. The other way around and they will always
eventually form one or more flocks.

Bird prims

Any currently rezzed in-scene-object can be used as the bird prim. However fps is very much affected by the
complexity of the entity to use. It is easier to throw a single prim (or sculpty) around the scene than it is to
throw the constituent parts of a 200 linked prim dragon.

Tests show that <= 500 single prims can be flocked effectively - depending on system and network	
However maybe <= 300 simple linksets can perform as well.

Network Traffic:

I tested the amount of network traffic generated by bird updates. 20 birds (each with 4 linked prims) takes up about 300kbps
in network position updates. 50 of the same birds generates about 750kbps traffic.
Each bird uses roughly 15kbps of network traffic. This is all measured using an update framerate of 1, i.e. birds' position
is updated every simulator frame.


Please Note 

This module is currently only tested against opensim master. 

Licence: all files released under a BSD licence
If you have any question please contact Jak Daniels, jak@ateb.co.uk
