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
// === TO DO NEXT ===
// =======================

//add more items to the shop

//update sprite

// =======================
// === NOTE FOR NEXT VISIT ===
// =======================

//so inventory system is down while I figure this out. I want to change the highlight and item grab so that it picks up on the right most block.
//I think it revolves around ItemGrid, Inventory Controller and Inventory Highlight scripts. Currently I'm looking at the methods GetTileGridPosition() and GetTileGridPosition(Vector2)
//both are involving the calculation/translation of mouse position to grid position.




