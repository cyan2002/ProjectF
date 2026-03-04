// =======================
// === OVERVIEW ===
// =======================

//This document is for denoting script functionality and highlighting dependencies. I have a mechanic document that offers more broad descriptions in the shared drive.

// =======================
// === INVENTORY STUFF ===
// =======================

//Link below is what I used for the "Resident Evil" Inventory System.
//https://www.youtube.com/watch?v=2ajD1GDbEzA&t=4604s

//when using scale with screen size for canvas - sometimes the image when testing is not what you actually see when built and playing
//when changing sizes of sprites - change from sprite edtior NOT object scale size.

//OPENING INVENTORY: Inventories are opened and closed by the shelf inventory Toggle script. These scripts live on the actual shelf/tank objects themselves.
//there is a dependency of that grid that the toggle script is controlling. If I plan on letting the player pick up and place tanks/shelfs this is the only main thing I need to be aware of.

//ACCESSING INVENTORY: each shelf/tank prefab is attached to a grid which you can specify the width and height of each grid. The Item Grid Script contains this.
//there is also a Grid Interact script that sets the selected grid. On the item Grid script you also specify what type of grid it is (dry vs wet)
//the grid interact script finds the inventory controller and makes the selected grid that grid when you hover your mouse over it (and it's currently opened via the player)

//ITEM GRID SCRIPT: the item grid script contains all the information of the items stored on the grid. It also handles items being put into the grid and taken out
//Additionally, items (inventoryItem script/object that contain ItemData) are stored under this during gameplay when items are added to the grid

//GRID INTERACTION SCRIPT: tells the inventory controller if the mouse is over its grid and makes the selected grid variable its own grid in the inventory controller

//INVENTORY CONTROLLER SCRIPT: handles input player and tells item grid script to add items or remove them based on input
//Also tells highlight what to do

//INVENTORY HIGHLIGHT: highlights the spots over the mouse when there is an item present. handled by inventory controller.

//INVENTORY ITEM SCRIPT: Place on item prefab that contains item data (but it's not just the item data). Multiple copies are created when purchasing items and placed onto grids.

//ITEM DATA SO: scriptable object that contains information on each data.


// =======================
// === OBJECT AND PREFABS ===
// =======================

//-Fish Tank
//DEPENDENCIES: Need to specify which grid it controls (parent object, not child)

//-Pegboard
//-Inventory Grid
//-Item - holds the item data (of type InventoryItem)
//DEPENDENCIES: Need to specify whuch grid it controls (parent object, not child)

//Grid Invetory
//must tell size (width and height)
//must tell Grid Type (Dry or weight currently)
//must dictate sprites (different types for wet vs dry or etc)

//VISUAL NOTES
//each thin white line box is a "meter" or one unity unit
//If I have a sprite that is a 32x32 pixels and I make it 32 pixels per units, I get a 1 Unity unit or 1 meter sprite in the scene/game
//formula is: World Size (units) = Pixel Size / Pixels Per Unit

// =======================
// === SHOP STUFF ===
// =======================

//Object and Prefabs:

//-Shop button
//DEPENDENCY: need to state which itemdata the button holds

//customer purchase logic: 
//customer spawns in
//customer wanders around store randomly.
//whenever the customer gets near a tank, notes down which things they want to buy
//when customer is ready to check out - goes to register with list of things to buy and buys them if attended to
//customer leaves

//things to decide:
//chance of buying things/preferences of customer
//how long to spend in the store
//how many things to buy
//how long to wait in line for

// =======================
// === NPC MOVEMENT ===
// =======================

//NPCs have multiple stages that they enter which varies based on their shopping experience in the store. Details of each stage is highlighted in the document.

//AStarManager Script:

//Node Script:

//NPC Controller Script: 

// =======================
// === AUDIO ===
// =======================

//audio works by calling to the Audio Manager (with both SFX and music sound control). Audio manager is a singleton that is created in every scene. 
//audio information (settings of sound) is saved via Unity's default saving (PlayerPref) class.
//There is a prefab called "audioboot" that create an audio manager in every scene before loading. 
//All sorts of objects can access the audio manager to make noises and etc
//also noises are Data structures (scriptable objects) and therefore will need to be passed as such.

// =======================
// === SCENE CHANGES ===
// =======================

//I've added a master scene that contains objects that will be used throughout the whole game.
//current list of things:
//Scene Loader
//Player (with dependables, see below)
//managers (Money and Music)
//Global lights (as opposed to scene lights which I'll add directly by scene)
//Player canvas (inventory and highlighter)
//Dragging canvas (for dragging inventory things to and from other inventories - ensuring items are always above the canvas and we can see them)

//player related stuff goes onto the Master Scene
//objects that are connected between player (dependenables) in the shop are assigned automatically. If nothing can be found null is applied and nothing happens.
//also, since this is a singleton, when objects are awake they can access the variables here and assign them. For instance, the shop canvas is assigned via the shop manager in the shop scene when it is awake
//the shop manager has the shop canvas because it lives in the same scene and I set it to that via drag and drop.
//for now the Player had all the dependables and they've been taken care of individually. It wasn't too much of a mess actually

//I mainly used chatGPT for the purposes of play testing the scenes. There are two main scripts that deal with heavy scene loading and etc.
//"PlayFromMaster.cs" contains a boolean that you must change in order to allow for playtesting.
//When I refer to play testing I mean loading that master scene before the current scene loads so that the player and time and etc shows up.
//true is playtesting, false is real deal.
//"sceneloader.cs" is a bit more complicated and I'll dive into at some point, but the gist is that it helps load past masters and handles fade in and fade out transitions. 
//other scripts such as SceneTransition actually hold an obect in the scene and use the scene loader to change scenes with fade in and fade out.



// =======================
// === TO DO NEXT ===
// =======================

//finish making street and home sprites

//once transitions are done, figure out saving

// =======================
// === NOTE FOR NEXT VISIT ===
// =======================





