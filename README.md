# UniApplicationGame

I have spent a couple of months trying to create a game from scratch, this is currently my latest version. It was a bit of a rough journey resulting in (if I'm to be honest) a rough current build. The fundamentals are all there and work E.g. the combat system, free roam and enemy A.I/detections, however there are some balancing issues in regards to some class sets being far more effective than others. Also while the A.I do track and chase the player as intended, they lack any knowledge of their surroundings and would likely jump in a hole if it were between them and the player.

The project was originally going to be a space themed RPG turn based game. It would have the player be a ship sprite (custom animated in photoshop) flying around in the overwhelming expanse of space. The player would have to avoid enemy ships (pirates, space militias and low life thugs) as well as the occasional meteor storm, finding safety only in form of:

Space stations (which would offer the player ways in which to heal).

Local planets whereby the player could switch from a space setting to a on the ground theme. with a space marine being the new sprite, and having the combat remain as a turn based against other space marines.

Safety clusters where the player could recruit other ships to the cause (using a party system of ships).

The main aim of this game would be for the player to last as long as they could in a cruel and unforgiving galaxy. Each space station would only be a one time use for healing, forcing the player to take risks in regards to fellow squamates health. Should I heal squamate B and pray that I can find/reach the next station without having to engage another enemy while squamate A is also low on health.

The player could increase their chances for survival by acquiring upgrades from their planet raiding adventures. Each planet fight won would see the player gain combat experience, when their XP bar reached full they would be bale to upgrade the :

Health of the ship and ground unit damage of the ship and ground unit special attacks of the ship and ground unit powerful heavy attacks of the ship and ground unit speed of the ship evasion of the ship and player unit healing bonus from space stations

After a month of trial and error with my space game adventures I had completed my:

Sprites Animations U.I Combat system Available attacks Healing from stations Space setting (canvas) A.I detection of the player and subsequent chasing of said player

I was yet to complete:

planetary free roam

XP system with dedicated buffs to chosen field such as health or damage so on

Five separate lists of available enemies the player could encounter (space pirates, space marines, space militias, opposing military ships and boss versions of all of the above)

The players ability to recruit allies and have a small squad system with them.

It was at this point I had realised I was dreaming too big. I was doing what Bungie had done with Halo 2, I'd come up with so many ideas I wanted to do. I would spend ages trying to get a piece of code to work while at the same time side tracking myself after thinking of something else that would be cool to implement and then going off to try it. Much like Bungie I was coming up with plenty of ideas, but would then realise that I didn't know how to do it the way I was envisioning.

I decided to dream smaller and more manageable. I scrapped the project and started again. This time I would be free roaming on the ground, I was to create a RPG turn based game in a local village. The village would be under constant besiege from bandits in the woods to the West and powerful gangs from the slums to the East. The player would have a squad of characters ranging from the typical honour bound brave knight, to the blood stained veteran warriors which only money can buy.

It started off well, I still had my player controller and Combat system from my previous project attempt. I would recycle these into something more fitting for the new game, I also brought over my detection and chase gizmos that the enemy A.I would have. Not all of my ideas from my previous project were out of reach for my current project. I would keep my XP system, the idea of healing being scarce and the general idea of the game to be seeing how long the player can survive. I got to work with creating a new battlefield canvas and UI. Where my space game took inspiration for it's UI from a game called Faster than light, my current project found inspiration from Pokémon. Once the Battlefield canvas was finished as was my UI I began to test the combat system code to make sure the turn based combat was still working. I then got to work programming the UI and stats for my base character. I would also begin with creating my list of NPC's to fight, making a template system to quickly create characters with unique attacks and stats.

I later got stuck trying to find a way to have the stats for each character to be calculated as well as how to set up a rock paper scissors system in regards to the combat. After many trial and errors I found a thread discussing a similar system to what I was aiming for, even though it's quite different to my end result and aim it was enough to help me figure out a working solution.

This also led me to discover a way to use arrays to make a chart with all of the class sets and dictate what is effective against what. I now had a working combat system, UI and characters with different stats and attacks. Next up was XP which I had to change from my previous vision of it (where it acted more like a perk system) instead it would behave like a standard level up system, giving a boost to all of the characters stats as well as leading to unlocking level locked abilities. I wanted it so that whenever the character levelled up it would also get a small HP heal. I was able to get the healing to work at a basic level, it heals by 10f each time a level up occurs.

All that remained code wise was the companions system and the separate enemy spawn lists. The most challenging code to make was the separate enemy spawn lists. I tried at first to copy basically everything I had done to create standard characters, by renaming all of the variables. However the two lists would keep clashing, when I figured out it was due to my game controller not changing state so that the boss script (and its enemies list) could take over and run its code I created a new game controller just for the boss script. However now the 2 game controllers were fighting for control and it was quickly becoming a mess. 4-5 days of this back and forth, lets try making another list under my map controller and have it so that when I stand on ground B and not A it starts the boss list... no such luck. Okay then I will try having sperate begin fight classes in my combat system class, still no such luck however I felt as though I was on the right track with this last attempt. Finally I decided to assign a list attached directly to the boss trigger zones in the inspector. This still didn't work properly however it was the closest I'd been, finally I had it working by assigning a separate companions list for my boss detector zones and having the begin boss fight classes still in my combat system class. The downside is however that the A.I will send them out in order of the list, and once they are all "wounded" the player can still begin a fight but will be soft locked as there are no more healthy targets.

I still had to scale back on multiple ideas, I did want more than two separate enemy lists however I am not sure how I would do so without making my combat system script triple in its current size. I am also embarrassed to admit I had several avoidable set backs where my work would become corrupted. The first instance was early on and was from me miss saving and breaking the pathway, more recently I started my PC up and was faced with the dreaded blue screen. I was able to pull what I needed onto a external HDD but still lost some progress. This made me paranoid to the point where I would create a back up of my project before starting for the day, and another when I was halfway through the day. I also created prototype projects where I would make a copy of my current work, and test new ideas on it before implementing onto my current build.

If you have not done so yet, the game can be played via a link on my UCAS submission. I have also attached 2 images showing a bug from me exporting my game to be played via website. For some reason when played via HTML5 some of my buildings lose their texture and barriers allowing the player to escape parts of the map and makes buildings look a bit broken.

Next to the bar at the top, near where it says main, you will find a blue limk titled "UniApplicationGame". If you click on it you shall be taken to my scripts folders.

Please enjoy.
