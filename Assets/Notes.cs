//NOTES DOCUMENT FOR CODING
//Last Updated: 12/1/25


//Link below is what I used for the "Resident Evil" Inventory System.
//https://www.youtube.com/watch?v=2ajD1GDbEzA&t=4604s

//when using scale with screen size for canvas - sometimes the image when testing is not what you actually see when built and playing
//when changing sizes of sprites - change from sprite edtior NOT object scale size.

//INVENTORIES
//two types - Player and Shelf/Tank Inventories

//Objects and Prefabs:

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

//SHOP

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




//TO DO NEXT:

//address tanks close to each other (inventory GUI line up)

//day mechanic

//“If a person has decided to buy something from the store, 
//they line up behind the cash register and have to be interacted with. 
//If the person directly in front of the register has not been interacted within 40 seconds, 
//they leave the store (and the thing they have decided to buy get put back)”

//implement music

//add more items to the shop




