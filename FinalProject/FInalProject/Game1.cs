//Author : Paarth Sharma
//File Name : Game1.cs
//Project Name : Final Project
//Creation Date : 17 December 2024
//Modification Date : 17 January 2024
//Description : A game about a Nordic boy seeking redemption for his village 
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;


namespace FInalProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //creating game states 
        const int MAINMENU = 0;
        const int HIGHSCORE = 1;
        const int FIRST_LEVEL = 2;
        const int SECOND_LEVEL = 3;
        const int FINAL_LEVEL = 4;
        const int ENDGAME = 5;
        const int HOUSE = 6;
        const int BACKPACK = 7;
        const int CRAFTING = 8;
        const int CHEST = 9;
        const int FAILED = 10;

        //creating the house states (changes based on the house rectangle the player intersects with)
        const int HOUSE1 = 0;
        const int HOUSE2 = 1;
        const int HOUSE3 = 2;
        const int HOUSE4 = 3;
        const int HOUSE5 = 4;

        // Constants for tile sizes used in the game 
        const int TILEWIDTH = 32;
        const int TILEHEIGHT = 32;
        const int TILESIZE = 32;

        //directions as constants 
        const int STOP = 0;
        const int FORWARD = 1;
        const int BACKWARD = -1;

        //creating sprite states 
        const int IDLE = 0;
        const int MOVERIGHT = 1;
        const int MOVELEFT = 2;
        const int DEATH = 3;
        const int SLASH = 4;
        const int SLICE = 5; //it is only for kight and player 
        const int STAB = 6; // it is for the player knight and witch
        const int DEFEND = 7; //this is only for the player 
        const int HEAVYATTACK = 8; //this is only for the player 
        const int SHOOT = 9; //only for player 

        //creating constants for tracking the position of where a item is being dragged from  
        const int CHECKINVENTORY = 0;
        const int CHECKCHEST = 1;
        const int CHECKBACKPACK = 2;
        const int CHECKCRAFTINGTABLE = 3;

        //setting constants for the enemy 
        const int KNIGHT = 0;
        const int ZOMBIE = 1;
        const int SKELETON = 2;

        //creating variable for storing the current game states 
        int gameState = MAINMENU;

        //creating variable for storing the current house states
        int houseState;

        //creating a variable to store the previous state 
        int prevGameState;

        //creating and storing the target FPS 
        float targetFPS = 60.0f;

        //creating and storing the screen width and screen height
        int screenWidth;
        int screenHeight;

        //direction variables
        int dirX;
        int dirY;

        //creating a storage for where the item is being dragged from 
        int whereFrom = -1;

        //creating storages for locations of where the items are coming from
        int location1D = -1;
        int location2DX = -1;
        int location2DY = -1;

        //creating states for drag and drop 
        bool dropped = false;
        bool pressed = false;

        //creating a temporary storage for the type of item while dragging
        int handItem;

        //creating a array of integers to track items in the inventory 
        int[] inventoryItems = new int[4];

        //creating a array of integers to track items in the inventory 
        int[] inventoryQuantities = new int[4];

        //creating an array of integers inside a list to track items in the chests
        int[,] chestItems = new int[5, 3];

        //creating an array of integers to track items in the backpack 
        int[,] backpackItems = new int[3, 5];

        //creating an array of integers to track items in the crafting table
        int[] craftingItems = new int[3];

        //creating a healthbar for the player 
        int playerHealth;

        //creating a varibale for storing state of the player 
        int playerState;

        //creating a rectanlge for player animation collision rec
        Rectangle playerAnimCollisionRec;

        //creating an array of integers for healthbars of enemies
        int[] knightsHealth = new int[5];
        int[] zombiesHealth = new int[5];
        int[] skeletonsHealth = new int[5];

        //creating an variable for healthbar for witch 
        int witchHealth;

        //creating a array of ints for enemy states 
        int[] knightsState = new int[5];
        bool[] knightWander = new bool[5];
        int[] zombiesState = new int[5];
        bool[] zombieWander = new bool[5];
        int[] skeletonsState = new int[5];
        bool[] skeletonWander = new bool[5];

        //creating a varibale for witch state 
        int witchState;

        Animation[] playerAnims = new Animation[10];
        Animation[,] knightAnims = new Animation[5, 7];
        Animation[,] skeletonAnims = new Animation[5, 5];
        Animation[,] zombieAnims = new Animation[5, 5];
        Animation[] witchAnims = new Animation[7];

        //creating an array of rectangles for colliison recs of the actual enemie(as animations are bigger then the player itself sometimes)
        Rectangle[] knightsRec = new Rectangle[5];
        Rectangle[] zombiesRec = new Rectangle[5];
        Rectangle[] skeletonsRec = new Rectangle[5];

        //creating an array of booleans if the enemy is alive or not 
        bool[] knightsAlive = new bool[5];
        bool[] zombiesAlive = new bool[5];
        bool[] skeletonsAlive = new bool[5];

        //creating a boolean if the player is alive 
        bool playerAlive = true;

        //creating an array of booleans if the witch is alive 
        bool witchAlive = true;

        //creating an array of booleans if the witch is colliding
        bool witchCollision = false;

        //creating a boolean if the player is idle or not 
        bool playerIdle = true;

        //creating a empty layer for displaying player 
        Dictionary<Vector2, int> displayLast;

        // Dictionaries for map layers with tile positions Vector2 and tile types int for map 1
        Dictionary<Vector2, int> map1BgLayer;
        Dictionary<Vector2, int> map1GroundLayer;
        Dictionary<Vector2, int> map1DecorationLayer;
        Dictionary<Vector2, int> map1FenceLayer;
        Dictionary<Vector2, int> map1TreeLayer;
        Dictionary<Vector2, int> map1houseLayer;
        Dictionary<Vector2, int> map1pathLayer;

        // Dictionaries for map layers with tile positions Vector2 and tile types int for map 2
        Dictionary<Vector2, int> map2GroundLayer;
        Dictionary<Vector2, int> map2DecorationLayer;
        Dictionary<Vector2, int> map2StairsLayer;
        Dictionary<Vector2, int> map2MtnLayer;
        Dictionary<Vector2, int> map2RelicLayer;

        // Dictionaries for map layers with tile positions Vector2 and tile types int for map 3
        Dictionary<Vector2, int> map3GroundLayer;
        Dictionary<Vector2, int> map3DecorationLayer;
        Dictionary<Vector2, int> map3StairsLayer;
        Dictionary<Vector2, int> map3MtnLayer;
        Dictionary<Vector2, int> map3RelicLayer;

        //Lists of rectangles for each tile for different features(base, fence, path, tree, house, lamp) on the maps.
        List<Rectangle> firstMapBase;
        List<Rectangle> firstMapFence;
        List<Rectangle> firstMapPath;
        List<Rectangle> firstMapTree;
        List<Rectangle> firstMapHouse;
        List<Rectangle> firstMapLamp;
        List<Rectangle> secondMapBase;
        List<Rectangle> thirdMapBase;
        List<Rectangle> secondMapRelic;

        // List for storing intersecting rectangles 
        List<Rectangle> intersections;

        //creating texture 2Ds for images of tile sets for map 1
        Texture2D map1BaseTilesImg;
        Texture2D houseTileImg;
        Texture2D treeTilesImg;
        Texture2D lampTilesImg;
        Texture2D fenceTilesImg;
        Texture2D villageTilesImg;

        //creating texture 2Ds for images of tile sets for map 2
        Texture2D map2BaseTilesImg;

        //creating texture 2Ds for images of tile sets for map 3
        Texture2D map3BaseTilesImg;

        //creating texture 2Ds for images of unique tile sets for map 2,3
        Texture2D relicTilesImg;

        //texture 2D for house background 
        Texture2D houseBgImg;

        //texture 2Ds for images of chest and crafting table 
        Texture2D craftingTableImg;
        Texture2D chestOpenImg;
        Texture2D chestClosedImg;

        // Texture for white rectangles 
        private Texture2D rectangleTexture;

        //creating a background image for backpack
        Texture2D backpackBgImg;

        //creating a background image for crafting menu 
        Texture2D craftingBgImg;

        //creating a texture 2D for final chest image 
        Texture2D finalChestImg;

        //creating an array of textures for inventory items 
        Texture2D[] itemTextures = new Texture2D[9];

        //creating a texture for player animations 
        Texture2D[] playerAnimImgs = new Texture2D[10];

        //creating an array of textures for knight animations 
        Texture2D[] knightAnimImgs = new Texture2D[9];

        //creating an array of textures for witch animations  
        Texture2D[] witchAnimImgs = new Texture2D[9];

        //creating a texture for zombie animations
        Texture2D[] zombieAnimImgs = new Texture2D[5];

        //creating a texture for skeleton animations
        Texture2D[] skeletonAnimImgs = new Texture2D[5];

        // Camera-related settings including zoom, rotation speed, and camera object.
        Cam2D camera;
        float zoom = 2.5f;
        float zoomSpeed = 0.02f;
        float rotateSpeed = 2f;

        // Keyboard and Mouse state tracking for player input.
        KeyboardState kb;
        KeyboardState prevKb;
        MouseState mouse;

        // World bounds, defining the playable area of the game world.
        Rectangle worldBounds;

        // Player-related variables, including texture, position, and movement speed.
        Texture2D playerImg;
        Rectangle playerRec;
        Vector2 playerPos;
        Vector2 playerScreenLoc;
        float moveSpeed = 25f;

        // enemies related positions  
        Vector2[] knightsPos = new Vector2[5];
        Vector2[] zombiesPos = new Vector2[5];
        Vector2[] skeletonsPos = new Vector2[5];
        Vector2 witchPos;

        // Mouse position tracking in both world and screen coordinates for different interactions.
        Vector2 mouseWorldLoc;
        Vector2 mouseWorldTextLoc;
        Vector2 mouseScreenTextLoc;
        Vector2 playerWorldTextLoc;
        Vector2 playerScreenTextLoc;

        //the max speed of the player              
        float maxPLayerSpeed = 200f;

        //creating and initializing the speed vector for the player
        Vector2 playerSpeed = new Vector2(0, 0);

        //creating a rectangle for crafting table 
        Rectangle craftingTable;

        //creating a rectangle for house background  
        Rectangle houseBgRec;

        //creating a rectangle for chest 
        Rectangle chestClosedRec; //would also be used in last level
        Rectangle chestOpenRec; //would be used in last level 

        //creating a rectangle for crafting table 
        Rectangle craftingTableRec;

        //creating a rectangle for background of inventory 
        Rectangle inventoryBgRec;

        //creating a background rectangle for the chest to display 
        Rectangle chestBgRec;

        //creating a background image rec for the crafting state 
        Rectangle craftingBgRec;

        //creating a background rec for backpack
        Rectangle backpackBgRec;

        //creating a rectangle final chest for the game completion 
        Rectangle finalChestRec;

        //rectangle for changing level
        Rectangle changeLevelRec;

        Rectangle witchRec;

        //creating a rectangle for storing the item while dragging
        Rectangle handItemRec;

        //creating a array of rectangles for houses 
        Rectangle[] houseList = new Rectangle[5];

        //creating a array of rectangles for the relics 
        Rectangle[] relicList = new Rectangle[4];

        //creating a array of rectangles drawing crafting table Rectangles  
        Rectangle[] craftingDrawRecs = new Rectangle[3];

        //creating an array of rectangles for drawing inventory rectangles 
        Rectangle[] inventoryDrawRecs = new Rectangle[4];

        //creating an array of rectangles for drawing backpack rectangles 
        Rectangle[,] backpackDrawRecs = new Rectangle[3, 5];

        //creating a array of rectangles inside a list for drawing rectangles of the chest 
        Rectangle[,] chestDrawRecs = new Rectangle[5, 3];

        //creating boolean variables for relics 
        bool relic1 = true;
        bool relic2 = true;
        bool relic3 = true;
        bool relic4 = true;

        //creating a boolean to track Quest completion for level 1, 2, 3
        bool quest1Status = true;
        bool quest2Status = true;
        bool quest3Status = true;

        //cretaing a boolean for witch wandering state 
        bool witchWander = false;

        //creating an array of points for the player to walk to 
        Vector2[,,] pathPoints = new Vector2[3, 5, 6];

        //creating an array of points for the witch to walk to 
        Vector2[] witchPathPoints = new Vector2[6];

        //creating cue points for all enemies 
        int[] knightsCuePoint = new int[5];
        int[] zombiesCuePoint = new int[5];
        int[] skeletonsCuePoint = new int[5];
        int witchCuePoint;

        //creating a timer so that the enemy attack has a cooldown
        Timer[,] enemyCooldown = new Timer[3, 5];

        //creating a cooldown timer for the witch 
        Timer witchCooldown = new Timer(1000, false);

        //creating a attack cooldown for the player 
        Timer attackCooldown = new Timer(500, false);

        // Arrays to hold projectile data
        Vector2[] projectilePositions = new Vector2[10]; // Assuming max 10 projectiles
        Vector2[] projectileVelocities = new Vector2[10];
        bool[] projectileIsAlive = new bool[10];
        int[] projectileDamage = new int[10]; // The damage the projectile deals

        //creting the variable to store active projectiles 
        int activeProjectiles = 0;

        //creating a texture for the main menu background image 
        Texture2D backgroundImg;

        //creating a rectangle for the background
        Rectangle backgroundRec;

        //creating a rectangle for the start button 
        Rectangle startButton;

        Rectangle exitButton;

        Texture2D endGameBgImg;

        Rectangle endGameBgRec;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //blank layer for the player 
            displayLast = LoadMap("../../../maps/blankLayer.csv");

            //loading in the csv files for map 1 (as dictionaries by using the method to parse values and coordinates) 
            map1BgLayer = LoadMap("../../../maps/map1/firstMap_background1.csv");
            map1GroundLayer = LoadMap("../../../maps/map1/firstMap_background2.csv");
            map1DecorationLayer = LoadMap("../../../maps/map1/firstMap_decoration.csv");
            map1FenceLayer = LoadMap("../../../maps/map1/firstMap_fenceLayer.csv");
            map1TreeLayer = LoadMap("../../../maps/map1/firstMap_treeLayer.csv");
            map1houseLayer = LoadMap("../../../maps/map1/firstMap_houseLayer.csv");
            map1pathLayer = LoadMap("../../../maps/map1/firstMap_pathLayer.csv");

            //loading in the csv files for map 2
            map2GroundLayer = LoadMap("../../../maps/map2/secondMap_ground layer.csv");
            map2DecorationLayer = LoadMap("../../../maps/map2/secondMap_decoration layer.csv");
            map2StairsLayer = LoadMap("../../../maps/map2/secondMap_stairs layer.csv");
            map2MtnLayer = LoadMap("../../../maps/map2/secondMap_mountain layer.csv");
            map2RelicLayer = LoadMap("../../../maps/map2/secondMap_relic layer.csv");

            //loading in the csv files for map 3
            map3GroundLayer = LoadMap("../../../maps/map3/thirdMap_ground layer.csv");
            map3DecorationLayer = LoadMap("../../../maps/map3/thirdMap_decoration layer.csv");
            map3MtnLayer = LoadMap("../../../maps/map3/thirdMap_mountain layer.csv");
            map3StairsLayer = LoadMap("../../../maps/map3/thirdMap_stairs layer.csv");
            map3RelicLayer = LoadMap("../../../maps/map3/thirdMap_relic & chest layer.csv");

            //creating a list of rectangles for map1 tileset image 
            firstMapBase = GenerateRectangles(16, 16, 16, 256);

            //creating a list of rectangles for map2 tileset image
            secondMapBase = GenerateRectangles(16, 16, 16, 256);

            //creating a list of rectangles for map3 tileset image 
            thirdMapBase = GenerateRectangles(16, 16, 16, 256);

            //creating a list of rectangles for map1 village path tile set image 
            firstMapPath = GenerateRectangles(4, 16, 16, 64);

            //creating a list of rectangles for map1 village fence tileset image 
            firstMapFence = GenerateRectangles(3, 16, 16, 6);

            //creating a list of rectangles for map1 house tileset image 
            firstMapHouse = GenerateRectangles(7, 50, 64, 7);

            //creating a list of rectangles for map1 lamp tileset image 
            firstMapLamp = GenerateRectangles(1, 16, 32, 1);

            //creating a list of rectangles for map1 tree tileset image 
            firstMapTree = GenerateRectangles(1, 48, 64, 1);

            //creating a list of rectangles for map2 relic tileset image 
            secondMapRelic = GenerateRectangles(2, 32, 51, 2);

        }

        //pre : loading in the filepath which is a csv file 
        //post : returning a list of vector2s and ints that have the rows and coloumns for the tile and the value of tile itself 
        //desc : is to create a dictionary with vector2s that is the key, stores the row and coloumn information tiles and value which is the value of the tile itself 
        private Dictionary<Vector2, int> LoadMap(string tilemap)
        {
            //creating a result dectionary 
            Dictionary<Vector2, int> result = new();

            //using a stream reader to read in the csv file 
            StreamReader reader = new(tilemap);

            //creating a string to read the grid line by line 
            string line;

            //creating the row index 
            int y = 0;

            // Loop through each line in the CSV file
            while ((line = reader.ReadLine()) != null)
            {
                // Splitting the line by commas to get the individual cell values
                string[] items = line.Split(',');

                //looping through each item in the line 
                for (int x = 0; x < items.Length; x++)
                {
                    //parsing cell value as a integer 
                    if (int.TryParse(items[x], out int value))
                    {
                        //parsing only the existing cell values (not -1)
                        if (value >= 0)
                        {
                            //storing the value of the cell at that point with the key as the coordinates 
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }
                //increasing the row value 
                y++;
            }
            //returning resulting dictionary 
            return result;
        }

        protected override void Initialize()
        {
            //adding the initialization logic 

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / targetFPS);

            graphics.ApplyChanges();

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            intersections = new();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //loading in the image tilemap for map 1 as texture2D
            map1BaseTilesImg = Content.Load<Texture2D>("textureAtlas/seasonal sample (winter)");

            //loading in the image tilemap for map 2 as texture2D
            map2BaseTilesImg = Content.Load<Texture2D>("textureAtlas/seasonal sample (spring)");

            //loading in the image tilemap for map 3 as texture2D
            map3BaseTilesImg = Content.Load<Texture2D>("textureAtlas/seasonal sample (autumn)");

            //loading in the image tilemap for map 1 village tiles  as texture2D
            villageTilesImg = Content.Load<Texture2D>("textureAtlas/path-tilemap");

            //loading in the image tilemap for map 1 fence tiles as texture2D
            fenceTilesImg = Content.Load<Texture2D>("textureAtlas/fence");

            //loading in the image tilemap for map 1 lamp tiles as texture2D
            lampTilesImg = Content.Load<Texture2D>("textureAtlas/lamp-post");

            //loading in the image tilemap for map 1 house tiles as texture2D
            houseTileImg = Content.Load<Texture2D>("textureAtlas/house1");

            //loading in the image tilemap for map 1 tree tiles as texture2D
            treeTilesImg = Content.Load<Texture2D>("textureAtlas/main-tree");

            //test only image as texture2D
            playerImg = Content.Load<Texture2D>("textureAtlas/Mario");

            //loading the image of tilemap for map 2 relic tiles as texture2D
            relicTilesImg = Content.Load<Texture2D>("textureAtlas/relic");

            //loading the image of chest and crafting table as texture2D
            craftingTableImg = Content.Load<Texture2D>("textureAtlas/craftingTable");
            chestOpenImg = Content.Load<Texture2D>("textureAtlas/chestOpen");
            chestClosedImg = Content.Load<Texture2D>("textureAtlas/chestClosed");

            //loading the background image for house as texture2D
            houseBgImg = Content.Load<Texture2D>("textureAtlas/houseBg");

            //loading the final chest image as texture2D
            finalChestImg = Content.Load<Texture2D>("textureAtlas/finalChest");

            //loading the crafting table image as texture 2D
            craftingBgImg = Content.Load<Texture2D>("textureAtlas/craftingTableBg");

            endGameBgImg = Content.Load<Texture2D>("Background/endGameBg");

            //creating and storing the world bounds as a rectangle 
            worldBounds = new Rectangle(0, 0, 1600, 1600);

            //storing the player sprites initial position 
            playerRec = new Rectangle(worldBounds.Width / 2, (worldBounds.Height / 2) - 100, 32, 48);
            playerPos = new Vector2(playerRec.X, playerRec.Y);

            //loading the textures for each item 
            itemTextures[0] = new Texture2D(GraphicsDevice, 1, 1);
            itemTextures[0].SetData(new Color[] { new(255, 255, 255, 0) });
            itemTextures[1] = Content.Load<Texture2D>("items/stick");
            itemTextures[2] = Content.Load<Texture2D>("items/feather");
            itemTextures[3] = Content.Load<Texture2D>("items/log");
            itemTextures[4] = Content.Load<Texture2D>("items/string");
            itemTextures[5] = Content.Load<Texture2D>("items/shield");
            itemTextures[6] = Content.Load<Texture2D>("items/sword");
            itemTextures[7] = Content.Load<Texture2D>("items/arrow");
            itemTextures[8] = Content.Load<Texture2D>("items/bow");

            //loading in player anim image 
            playerAnimImgs[IDLE] = Content.Load<Texture2D>("Anims/vikingAnims/idle");
            playerAnimImgs[MOVELEFT] = Content.Load<Texture2D>("Anims/vikingAnims/move");
            playerAnimImgs[MOVERIGHT] = Content.Load<Texture2D>("Anims/vikingAnims/move");
            playerAnimImgs[STAB] = Content.Load<Texture2D>("Anims/vikingAnims/stab");
            playerAnimImgs[SLASH] = Content.Load<Texture2D>("Anims/vikingAnims/slash");
            playerAnimImgs[SLICE] = Content.Load<Texture2D>("Anims/vikingAnims/slice");
            playerAnimImgs[HEAVYATTACK] = Content.Load<Texture2D>("Anims/vikingAnims/heavyattack");
            playerAnimImgs[SHOOT] = Content.Load<Texture2D>("Anims/vikingAnims/shoot");
            playerAnimImgs[DEATH] = Content.Load<Texture2D>("Anims/vikingAnims/death");
            playerAnimImgs[DEFEND] = Content.Load<Texture2D>("Anims/vikingAnims/defend");

            //loading in skeleton anims image 
            skeletonAnimImgs[IDLE] = Content.Load<Texture2D>("Anims/skeletonAnims/skeleton_row_1");
            skeletonAnimImgs[MOVELEFT] = Content.Load<Texture2D>("Anims/skeletonAnims/skeleton_row_2");
            skeletonAnimImgs[MOVERIGHT] = Content.Load<Texture2D>("Anims/skeletonAnims/skeleton_row_2");
            skeletonAnimImgs[SLASH] = Content.Load<Texture2D>("Anims/skeletonAnims/skeleton_row_3");
            skeletonAnimImgs[DEATH] = Content.Load<Texture2D>("Anims/skeletonAnims/skeleton_row_5");

            //loading in zombie anim image 
            zombieAnimImgs[IDLE] = Content.Load<Texture2D>("Anims/zombieAnims/zombie_row_1");
            zombieAnimImgs[MOVERIGHT] = Content.Load<Texture2D>("Anims/zombieAnims/zombie_row_2");
            zombieAnimImgs[MOVELEFT] = Content.Load<Texture2D>("Anims/zombieAnims/zombie_row_2");
            zombieAnimImgs[DEATH] = Content.Load<Texture2D>("Anims/zombieAnims/zombie_row_3");
            zombieAnimImgs[SLASH] = Content.Load<Texture2D>("Anims/zombieAnims/zombie_row_4");

            //loading in the images for knight animations 
            knightAnimImgs[IDLE] = Content.Load<Texture2D>("Anims/knightAnims/idle");
            knightAnimImgs[MOVERIGHT] = Content.Load<Texture2D>("Anims/knightAnims/walk");
            //there is no move left animation this cell in the array is just kept to keep uniformity of the parallel arrays 
            knightAnimImgs[DEATH] = Content.Load<Texture2D>("Anims/knightAnims/death");
            knightAnimImgs[SLASH] = Content.Load<Texture2D>("Anims/knightAnims/slash");
            knightAnimImgs[SLICE] = Content.Load<Texture2D>("Anims/knightAnims/slice");
            knightAnimImgs[STAB] = Content.Load<Texture2D>("Anims/knightAnims/stab");

            //loading in the images of the witch animations 
            witchAnimImgs[IDLE] = Content.Load<Texture2D>("Anims/WitchAnims/B_Witch_idle");
            witchAnimImgs[MOVERIGHT] = Content.Load<Texture2D>("Anims/WitchAnims/B_witch_run");
            //cell left for uniformity 
            witchAnimImgs[DEATH] = Content.Load<Texture2D>("Anims/WitchAnims/B_witch_death");
            witchAnimImgs[SLASH] = Content.Load<Texture2D>("Anims/WitchAnims/B_witch_attack");
            witchAnimImgs[STAB] = Content.Load<Texture2D>("Anims/WitchAnims/B_witch_charge");

            //loading the background image 
            backgroundImg = Content.Load<Texture2D>("Background/bgImg");

            //initializing the camera object 
            camera = new Cam2D(GraphicsDevice.Viewport,
                            worldBounds,
                            zoom,
                            4.0f,
                            0f,
                            playerRec);

            //creating and storing hollow rectangle texture for debugging  
            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 255, 255, 125) });

            backpackBgImg = new Texture2D(GraphicsDevice, 1, 1);
            backpackBgImg.SetData(new Color[] { new(160, 160, 160, 130) });

            //defining backpack BG rec 
            backpackBgRec = new Rectangle(600, 200, 300, 500);

            //defining the house background rectangle 
            houseBgRec = new Rectangle(0, 0, 1600, 1600);

            //defining and positioning chest closed recs
            chestClosedRec = new Rectangle(270, 965, chestClosedImg.Width, chestClosedImg.Height);

            //defining and positioning crafting table 
            craftingTableRec = new Rectangle(470, 250, (int)(craftingTableImg.Width * 0.5), (int)(craftingTableImg.Height * 0.5));

            //defining chest Background Rectangle 
            chestBgRec = new Rectangle(70, 300, 300, 100);

            //defining the rectangle passing to different level 
            changeLevelRec = new Rectangle((4 * TILEWIDTH) - 10, 2 * TILEHEIGHT, TILEWIDTH, TILEHEIGHT);

            //defining the final chest rectangle 
            finalChestRec = new Rectangle(0, 0, finalChestImg.Width, finalChestImg.Height);

            //Defining the background rectangle for inventory 
            inventoryBgRec = new Rectangle(screenWidth / 2 - 200, screenHeight - 150, 400, 100);

            //defining the rectangle for item while dragging 
            handItemRec = new Rectangle(0, 0, 100, 100);

            //defining the array of integers for tracking inventory items 
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                inventoryItems[i] = 0;
            }

            //defining the array of integers for tracking inventory quantities (for quiver slot)
            for (int i = 0; i < inventoryQuantities.Length; i++)
            {
                inventoryQuantities[i] = 0;
            }

            //defining an array of rectangles for drawing the inventory
            for (int i = 0; i < inventoryDrawRecs.Length; i++)
            {
                inventoryDrawRecs[i] = new Rectangle(inventoryBgRec.X + (i * 100), inventoryBgRec.Y, 100, 100);
            }

            //defining a 2D array of rectangles for drawing the backpack
            for (int i = 0; i < backpackDrawRecs.GetLength(0); i++)
            {
                for (int j = 0; j < backpackDrawRecs.GetLength(1); j++)
                {
                    backpackDrawRecs[i, j] = new Rectangle(backpackBgRec.X + (i * 100), backpackBgRec.Y + (j * 100), 100, 100);
                }
            }

            //defining a 2D array of integers for the backpack items
            for (int i = 0; i < backpackItems.GetLength(0); i++)
            {
                for (int j = 0; j < backpackItems.GetLength(1); j++)
                {
                    backpackItems[i, j] = 0;
                }
            }

            //defining a 2D array of rectangles for drawing the chest
            for (int i = 0; i < chestDrawRecs.GetLength(0); i++)
            {
                for (int j = 0; j < chestDrawRecs.GetLength(1); j++)
                {
                    chestDrawRecs[i, j] = new Rectangle(chestBgRec.X + (j * 100), chestBgRec.Y, 100, 100);
                }
            }

            //defining crafting table items
            for (int i = 0; i < craftingItems.Length; i++)
            {
                craftingItems[i] = 0;
            }

            //defining the background rectangle for crafting table 
            craftingBgRec = new Rectangle(40, 300, 300, 120);

            //creating crafting table slots for drawing them 
            for (int i = 0; i < craftingDrawRecs.Length; i++)
            {
                craftingDrawRecs[i] = new Rectangle(craftingBgRec.X + (i * 100), craftingBgRec.Y + 10, 100, 100);
            }

            //setting up the chest items 
            chestItems[0, 0] = 1;
            chestItems[0, 1] = 1;
            chestItems[0, 2] = 1;
            chestItems[1, 0] = 1;
            chestItems[1, 1] = 1;
            chestItems[1, 2] = 2;
            chestItems[2, 0] = 2;
            chestItems[2, 1] = 2;
            chestItems[2, 2] = 2;
            chestItems[3, 0] = 2;
            chestItems[3, 1] = 3;
            chestItems[3, 2] = 4;
            chestItems[4, 0] = 5;
            chestItems[4, 1] = 6;
            chestItems[4, 2] = 0;

            //defining the health Bar for the player
            playerHealth = 100;

            //defining the health bar for knights 
            for (int i = 0; i < knightsHealth.Length; i++)
            {
                knightsHealth[i] = 100;
            }

            //defining the health bar for zombies 
            for (int i = 0; i < zombiesHealth.Length; i++)
            {
                zombiesHealth[i] = 200;
            }

            //defining the health bar for skeletons 
            for (int i = 0; i < skeletonsHealth.Length; i++)
            {
                skeletonsHealth[i] = 300;
            }

            //defining the health bar for witch
            witchHealth = 1000;

            //creating the positions of the knight 
            for (int i = 0; i < knightsPos.Length; i++)
            {
                knightsPos[i].X = i * 200 + 50;
                knightsPos[i].Y = 300;
            }

            //creating the position of the zombie
            for (int i = 0; i < zombiesPos.Length; i++)
            {
                zombiesPos[i].X = i * 200 + 800;
                zombiesPos[i].Y = 1200;
            }

            //creating the position for the skeletons 
            for (int i = 0; i < skeletonsPos.Length; i++)
            {
                skeletonsPos[i].X = i * 200 + 50;
                skeletonsPos[i].Y = 1200;
            }

            //defining the witch position 
            witchPos = new Vector2(worldBounds.X / 2, worldBounds.Y / 2);

            //defining the animations for knight
            for (int i = 0; i < knightAnims.GetLength(0); i++)
            {
                knightAnims[i, IDLE] = new Animation(knightAnimImgs[IDLE], 7, 1, 7, 0, 0, Animation.ANIMATE_FOREVER, 500, knightsPos[i], true);
                knightAnims[i, MOVERIGHT] = new Animation(knightAnimImgs[MOVERIGHT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, knightsPos[i], true);
                knightAnims[i, MOVELEFT] = new Animation(knightAnimImgs[MOVERIGHT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, knightsPos[i], true);
                knightAnims[i, DEATH] = new Animation(knightAnimImgs[DEATH], 12, 1, 12, 0, 0, 1, 2000, knightsPos[i], false);
                knightAnims[i, SLASH] = new Animation(knightAnimImgs[SLASH], 6, 1, 6, 0, 0, 1, 2000, knightsPos[i], false);
                knightAnims[i, SLICE] = new Animation(knightAnimImgs[SLICE], 6, 1, 6, 0, 0, 1, 2000, knightsPos[i], false);
                knightAnims[i, STAB] = new Animation(knightAnimImgs[STAB], 5, 1, 5, 0, 0, 1, 2000, knightsPos[i], false);
            }

            //defining the animations for zombies
            for (int i = 0; i < zombieAnims.GetLength(0); i++)
            {
                zombieAnims[i, IDLE] = new Animation(zombieAnimImgs[IDLE], 8, 1, 4, 0, 0, Animation.ANIMATE_FOREVER, 500, zombiesPos[i], true);
                zombieAnims[i, MOVERIGHT] = new Animation(zombieAnimImgs[MOVERIGHT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, zombiesPos[i], true);
                zombieAnims[i, MOVELEFT] = new Animation(zombieAnimImgs[MOVELEFT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, zombiesPos[i], true);
                zombieAnims[i, SLASH] = new Animation(zombieAnimImgs[SLASH], 8, 1, 2, 0, 0, 1, 3000, zombiesPos[i], false);
                zombieAnims[i, DEATH] = new Animation(zombieAnimImgs[DEATH], 8, 1, 3, 0, 0, 1, 3000, zombiesPos[i], false);
            }

            //defining the animations for skeletons
            for (int i = 0; i < skeletonAnims.GetLength(0); i++)
            {
                skeletonAnims[i, IDLE] = new Animation(skeletonAnimImgs[IDLE], 8, 1, 3, 0, 0, Animation.ANIMATE_FOREVER, 500, skeletonsPos[i], true);
                skeletonAnims[i, MOVERIGHT] = new Animation(skeletonAnimImgs[MOVERIGHT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, skeletonsPos[i], true);
                skeletonAnims[i, MOVELEFT] = new Animation(skeletonAnimImgs[MOVELEFT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, skeletonsPos[i], true);
                skeletonAnims[i, DEATH] = new Animation(skeletonAnimImgs[DEATH], 8, 1, 6, 0, 0, 1, 2000, skeletonsPos[i], false);
                skeletonAnims[i, SLASH] = new Animation(skeletonAnimImgs[SLASH], 8, 1, 4, 0, 0, 1, 2000, skeletonsPos[i], false);
            }

            //defining the animations for witch
            witchAnims[IDLE] = new Animation(witchAnimImgs[IDLE], 1, 6, 6, 0, 0, Animation.ANIMATE_FOREVER, 500, witchPos, true);
            witchAnims[MOVERIGHT] = new Animation(witchAnimImgs[MOVERIGHT], 1, 8, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, witchPos, true);
            witchAnims[MOVELEFT] = new Animation(witchAnimImgs[MOVERIGHT], 1, 8, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, witchPos, true);
            witchAnims[DEATH] = new Animation(witchAnimImgs[DEATH], 1, 12, 12, 0, 0, 1, 2000, witchPos, false);
            witchAnims[SLASH] = new Animation(witchAnimImgs[SLASH], 1, 5, 5, 0, 0, 1, 2000, witchPos, false);
            witchAnims[STAB] = new Animation(witchAnimImgs[STAB], 1, 9, 9, 0, 0, 1, 2000, witchPos, false);


            //defining the animations for the player 
            playerAnims[IDLE] = new Animation(playerAnimImgs[IDLE], 13, 1, 8, 0, 1, Animation.ANIMATE_FOREVER, 500, playerPos, true);
            playerAnims[MOVERIGHT] = new Animation(playerAnimImgs[MOVERIGHT], 13, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, playerPos, true);
            playerAnims[MOVELEFT] = new Animation(playerAnimImgs[MOVELEFT], 13, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, playerPos, true);
            playerAnims[DEATH] = new Animation(playerAnimImgs[DEATH], 13, 1, 12, 0, 0, 1, 1500, playerPos, false);
            playerAnims[SLASH] = new Animation(playerAnimImgs[SLASH], 13, 1, 4, 0, 0, 1, 500, playerPos, false);
            playerAnims[SLICE] = new Animation(playerAnimImgs[SLICE], 13, 1, 4, 0, 0, 1, 500, playerPos, false);
            playerAnims[STAB] = new Animation(playerAnimImgs[STAB], 13, 1, 4, 0, 0, 1, 500, playerPos, false);
            playerAnims[DEFEND] = new Animation(playerAnimImgs[DEFEND], 13, 1, 5, 0, 0, 1, 500, playerPos, false);
            playerAnims[HEAVYATTACK] = new Animation(playerAnimImgs[HEAVYATTACK], 13, 1, 11, 0, 0, 1, 1000, playerPos, false);
            playerAnims[SHOOT] = new Animation(playerAnimImgs[SHOOT], 13, 1, 12, 0, 0, 1, 1000, playerPos, false);

            //defining the knight states
            for (int i = 0; i < knightsState.Length; i++)
            {
                knightsState[i] = IDLE;
            }

            //defining the zombie states 
            for (int i = 0; i < zombiesState.Length; i++)
            {
                zombiesState[i] = IDLE;
            }

            //defining the skeleton states 
            for (int i = 0; i < skeletonsState.Length; i++)
            {
                skeletonsState[i] = IDLE;
            }

            //defining the knight alive variable 
            for (int i = 0; i < knightsAlive.Length; i++)
            {
                knightsAlive[i] = false;
            }

            //defining the zombie alive variable 
            for (int i = 0; i < zombiesAlive.Length; i++)
            {
                zombiesAlive[i] = false;
            }

            //defining the skeleton alive variable 
            for (int i = 0; i < skeletonsAlive.Length; i++)
            {
                skeletonsAlive[i] = false;
            }

            //defining the knight collision Recs
            for (int i = 0; i < knightsRec.Length; i++)
            {
                knightsRec[i] = new Rectangle(-100, -100, knightAnims[0, IDLE].GetDestRec().Width, knightAnims[0, IDLE].GetDestRec().Height);
            }

            //defining the zombie collision Recs
            for (int i = 0; i < zombiesRec.Length; i++)
            {
                zombiesRec[i] = new Rectangle(-100, -100, zombieAnims[0, IDLE].GetDestRec().Width, zombieAnims[0, IDLE].GetDestRec().Height);
            }

            //defining the rectangles for skeleton 
            for (int i = 0; i < skeletonsRec.Length; i++)
            {
                skeletonsRec[i] = new Rectangle(-100, -100, skeletonAnims[0, IDLE].GetDestRec().Width, skeletonAnims[0, IDLE].GetDestRec().Height);
            }

            //creating a random object for random positions of path points 
            Random random = new Random();

            //defining the minimum and maximum distance between each player 
            float minDistance = 100.0f;
            float maxDistance = 200.0f;


            //defining cue points for the enemies 
            for (int j = 0; j < pathPoints.GetLength(1); j++)
            {
                for (int k = 0; k < pathPoints.GetLength(2); k++)
                {
                    float randomX = knightsPos[j].X + random.Next((int)minDistance, (int)maxDistance);
                    float randomY = knightsPos[j].Y + random.Next((int)minDistance, (int)maxDistance);
                    pathPoints[0, j, k] = new Vector2(randomX, randomY);
                }
            }
            for (int j = 0; j < pathPoints.GetLength(1); j++)
            {
                for (int k = 0; k < pathPoints.GetLength(2); k++)
                {
                    float randomX = zombiesPos[j].X + random.Next((int)minDistance, (int)maxDistance);
                    float randomY = zombiesPos[j].Y + random.Next((int)minDistance, (int)maxDistance);
                    pathPoints[1, j, k] = new Vector2(randomX, randomY);
                }
            }
            for (int j = 0; j < pathPoints.GetLength(1); j++)
            {
                for (int k = 0; k < pathPoints.GetLength(2); k++)
                {
                    float randomX = skeletonsPos[j].X + random.Next((int)minDistance, (int)maxDistance);
                    float randomY = skeletonsPos[j].Y + random.Next((int)minDistance, (int)maxDistance);
                    pathPoints[2, j, k] = new Vector2(randomX, randomY);
                }
            }
            for (int i = 0; i < witchPathPoints.Length; i++)
            {
                float randomX = witchPos.X + random.Next((int)minDistance, (int)maxDistance);
                float randomY = witchPos.Y + random.Next((int)minDistance, (int)maxDistance);
                witchPathPoints[i] = new Vector2(randomX, randomY);
            }

            //defining the cue points for the enemies 
            for (int i = 0; i < knightsCuePoint.Length; i++)
            {
                knightsCuePoint[i] = 0;
            }
            for (int i = 0; i < zombiesCuePoint.Length; i++)
            {
                zombiesCuePoint[i] = 0;
            }
            for (int i = 0; i < skeletonsCuePoint.Length; i++)
            {
                skeletonsCuePoint[i] = 0;
            }

            //Defining the witch cue point 
            witchCuePoint = 0;

            //defining the enemy cooldown timers to prevent attack spamming 
            for (int i = 0; i < enemyCooldown.GetLength(0); i++)
            {
                for (int j = 0; j < enemyCooldown.GetLength(1); j++)
                {
                    enemyCooldown[i, j] = new Timer(1000, false);
                }
            }

            //defining the witch wander state 
            witchWander = false;

            //defining the rectangle for the background 
            backgroundRec = new Rectangle(0, 0, 1000, 1000);

            //defining the start button 
            startButton = new Rectangle(362, 200, 300, 500);

            //defining the exit button 
            exitButton = new Rectangle(410, 680, 200, 60);

            //defining the end game background 
            endGameBgRec = new Rectangle(0, 0, 1000, 1000);

        }


        protected override void Update(GameTime gameTime)
        {

            //Adding the update logic 

            //updating the keyboard and the mouse states 
            prevKb = kb;
            kb = Keyboard.GetState();
            mouse = Mouse.GetState();

            //use the appropriate logic based on game state 
            switch (gameState)
            {
                case MAINMENU:

                    //adding the start game button 
                    if (mouse.LeftButton == ButtonState.Pressed && startButton.Contains(mouse.Position))
                    {
                        //change game state to first level 
                        gameState = FIRST_LEVEL;
                    }

                    break;
                case HIGHSCORE:

                    //checking if player presses the escape button 
                    if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
                    {
                        //going back to main menu 
                        gameState = MAINMENU;
                    }

                    break;
                case FIRST_LEVEL:

                    //changing the previous state to game state 
                    prevGameState = FIRST_LEVEL;

                    //Character movement
                    UpdatePlayerMovement(gameTime, map1FenceLayer);

                    //checking if the player enters a house 
                    HouseCheck(houseList);

                    //checking for players want to open backpack 
                    if (kb.IsKeyDown(Keys.B))
                    {
                        gameState = BACKPACK;
                    }

                    //looping through the house list 
                    foreach (var house in houseList)
                    {
                        //collision detection for the house 
                        ObjectCollisionPlayer(house);
                    }

                    //checking if the player Intersects with the cave rectangle 
                    if (playerRec.Intersects(changeLevelRec) && quest1Status == true)
                    {
                        //Transitioning the game state 
                        gameState = SECOND_LEVEL;
                        playerRec.X = 1000;
                        playerRec.Y = 800;
                    }

                    //updating the quest log 
                    UpdateQuestLog();

                    break;
                case SECOND_LEVEL:

                    //updating the attack cooldown for the player 
                    attackCooldown.Update(gameTime);

                    //changing the previous state to game state 
                    prevGameState = SECOND_LEVEL;

                    //Character movement
                    UpdatePlayerMovement(gameTime, map2MtnLayer);

                    //checking for players want to open backpack 
                    if (kb.IsKeyDown(Keys.B))
                    {
                        gameState = BACKPACK;
                    }

                    //checking if the player wants to go to the next level 
                    if (playerRec.Intersects(changeLevelRec) && quest2Status == true)
                    {
                        gameState = FINAL_LEVEL;
                        playerRec.X = 1000;
                        playerRec.Y = 800;
                    }

                    //checking if the player is alive 
                    if (playerAlive == false)
                    {
                        //if he dies setting the state to failed 
                        gameState = ENDGAME;
                    }

                    //updating the knight states 
                    followerStateUpdate(knightsState, knightWander, knightsRec, pathPoints, knightsPos, 0, knightsCuePoint, knightsPos, knightAnims, gameTime, knightsAlive);

                    //updating the animations for the knights 
                    targetAnimsUpdate(knightAnims, knightsState, knightWander, 0, gameTime);

                    //updating teh zombie states 
                    followerStateUpdate(zombiesState, zombieWander, zombiesRec, pathPoints, zombiesPos, 1, zombiesCuePoint, zombiesPos, zombieAnims, gameTime, zombiesAlive);

                    //updating the zombie animations 
                    targetAnimsUpdate(zombieAnims, zombiesState, zombieWander, 1, gameTime);

                    //updating the skkeleton enemies state 
                    followerStateUpdate(skeletonsState, skeletonWander, skeletonsRec, pathPoints, skeletonsPos, 2, skeletonsCuePoint, skeletonsPos, skeletonAnims, gameTime, skeletonsAlive);

                    //updating teh skeleton Animations 
                    targetAnimsUpdate(skeletonAnims, skeletonsState, skeletonWander, 2, gameTime);

                    //updating the quest log 
                    UpdateQuestLog();

                    break;
                case FINAL_LEVEL:

                    //changing the previous state to game state 
                    prevGameState = FINAL_LEVEL;

                    //Character movement
                    UpdatePlayerMovement(gameTime, map3MtnLayer);

                    //checking for players want to open backpack 
                    if (kb.IsKeyDown(Keys.B))
                    {
                        gameState = BACKPACK;
                    }

                    //checking if the player is alive 
                    if (playerAlive == false)
                    {
                        //going to the failed state if the player is dead 
                        gameState = ENDGAME;
                    }

                    //checking if the player intersects the exit chest 
                    if (playerRec.Intersects(finalChestRec) && quest3Status == true)
                    {
                        //Transitioning to the end game state 
                        gameState = ENDGAME;
                    }

                    //updating the witch's states 
                    witchStateUpdate(gameTime);

                    //updating the witch animations 
                    witchAnimsUpdate(witchAnims, gameTime);

                    //updating the quest log 
                    UpdateQuestLog();

                    break;
                case BACKPACK:

                    //checking if the player wants to exit the backpack
                    if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
                    {
                        //going to the previous state that the player opened backpack in 
                        gameState = prevGameState;
                    }

                    //updating drag for the player 
                    UpdateDrag();

                    break;
                case HOUSE:

                    //character movement 
                    UpdatePlayerMovement(gameTime, displayLast);

                    //enabling going back to level 1
                    if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
                    {
                        gameState = FIRST_LEVEL;
                        playerPos.X = 800;
                        playerPos.Y = 900;
                    }

                    //checking for players want to open backpack 
                    if (kb.IsKeyDown(Keys.B))
                    {
                        //setting the gameState to backpack
                        gameState = BACKPACK;
                    }

                    //checking for the players choice on chest 
                    if (playerRec.Intersects(chestClosedRec) && kb.IsKeyDown(Keys.C))
                    {
                        //setting the gameState to CHEST
                        gameState = CHEST;
                    }
                    //check for player intersection with the crafting table 
                    if (playerRec.Intersects(craftingTableRec) && kb.IsKeyDown(Keys.R))
                    {
                        //setting the gameState to crafting 
                        gameState = CRAFTING;
                    }
                    else
                    {

                        //collision detection for player with chest and crafting table to make them uncrossable objects  
                        ObjectCollisionPlayer(craftingTableRec);
                        ObjectCollisionPlayer(chestClosedRec);

                    }

                    break;
                case CRAFTING:

                    //checking if the player exits the crafting table 
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        gameState = HOUSE;
                        playerRec.X = 800;
                        playerRec.Y = 800;
                    }

                    //updating drag for the player 
                    UpdateDrag();

                    //updating the crafting menu
                    UpdateCrafting();

                    break;
                case CHEST:

                    //cheching if the player exits the chest 
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        gameState = HOUSE;
                        playerRec.X = 800;
                        playerRec.Y = 800;
                    }

                    //updating the player Drag 
                    UpdateDrag();

                    break;

                case ENDGAME:

                    //exiting the game if the player presses the exit button 
                    if (mouse.LeftButton == ButtonState.Pressed && exitButton.Contains(mouse.Position))
                    {
                        Exit();
                    }

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //adding the draw code
            switch (gameState)
            {
                case MAINMENU:
                    spriteBatch.Begin();
                    //Displaying the background 
                    spriteBatch.Draw(backgroundImg, backgroundRec, Color.White);

                    //Displaying the Name of the game 


                    //Displaying the high score 


                    //Displaying the start game button 

                    spriteBatch.End();
                    break;

                case HIGHSCORE:

                    //high score 

                    //highest ultimate score 

                    break;
                case FIRST_LEVEL:

                    //displaying the first layer of the background 
                    DisplayLayer(camera, map1BgLayer, firstMapBase, map1BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the second layer of the background 
                    DisplayLayer(camera, map1GroundLayer, firstMapBase, map1BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the third layer of the background 
                    DisplayLayer(camera, map1pathLayer, firstMapPath, villageTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the fourth layer of the background
                    DisplayLayer(camera, map1FenceLayer, firstMapFence, fenceTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the houses 
                    DisplayHouse(camera, map1houseLayer, firstMapHouse, houseTileImg, 50, 64);

                    //displaying the fifth layer of the background
                    DisplayDecoration(camera, map1DecorationLayer, firstMapLamp, lampTilesImg, 16, 32);

                    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());
                    ////display the visible inventory of the player  
                    //DisplayInventory();

                    ////display the objectives player has to achieve in this level 
                    //DisplayQuestLog();
                    //spriteBatch.End();

                    //displaying the empty layer for the player 
                    DisplayPlayer(camera, displayLast, TILEWIDTH, TILEHEIGHT, mouse);

                    break;
                case SECOND_LEVEL:

                    //displaying the first layer of the background 
                    DisplayLayer(camera, map2GroundLayer, secondMapBase, map2BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the second layer of the background 
                    DisplayLayer(camera, map2DecorationLayer, secondMapBase, map2BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the third layer of the background 
                    DisplayLayer(camera, map2MtnLayer, secondMapBase, map2BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the fourth layer of the background 
                    DisplayLayer(camera, map2StairsLayer, secondMapBase, map2BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //display relic layer
                    DisplayRelic(camera, map2RelicLayer, secondMapRelic, relicTilesImg, 32, 51);

                    //displaying the empty layer for the player 
                    DisplayPlayer(camera, displayLast, TILEWIDTH, TILEHEIGHT, mouse);

                    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());
                    ////display the visible inventory of the player 
                    //DisplayInventory();

                    ////display the objectives that the player has to achieve in this level 
                    //DisplayQuestLog();
                    //spriteBatch.End();

                    //displaying the animations of the enemies 
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

                    DisplayKnightAnimations();
                    DisplaySkeletonAnimations();
                    DisplayZombieAnimations();

                    //displaying projectiles
                    DrawProjectiles(spriteBatch);

                    spriteBatch.End();

                    break;
                case FINAL_LEVEL:

                    //displaying the first layer of the background 
                    DisplayLayer(camera, map3GroundLayer, thirdMapBase, map3BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the second layer of the background
                    DisplayLayer(camera, map3DecorationLayer, thirdMapBase, map3BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the third layer of the background
                    DisplayLayer(camera, map3MtnLayer, thirdMapBase, map3BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the fourth layer of the background
                    DisplayLayer(camera, map3StairsLayer, thirdMapBase, map3BaseTilesImg, TILEWIDTH, TILEHEIGHT);

                    //displaying the relic layer 
                    DisplayRelic(camera, map3RelicLayer, secondMapRelic, relicTilesImg, 32, 51);

                    //displaying the empty layer for the player 
                    DisplayPlayer(camera, displayLast, TILEWIDTH, TILEHEIGHT, mouse);

                    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());
                    ////displaying the visible inventory area of the player 
                    //DisplayInventory();

                    ////display the objectives the player has to achieve in this game 
                    //DisplayQuestLog();
                    //spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

                    DisplayWitch();

                    DrawProjectiles(spriteBatch);

                    spriteBatch.End();

                    break;
                case BACKPACK:

                    spriteBatch.Begin();

                    //displaying the storage area of the backpack 
                    DisplayBackpack();

                    //displaying the visible inventory area of the player 
                    DisplayInventory();

                    //displaying the drag item 
                    DisplayDrag();

                    spriteBatch.End();

                    break;
                case HOUSE:

                    //starting a spriteBatch 
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());

                    //house background 
                    spriteBatch.Draw(houseBgImg, houseBgRec, Color.White);

                    //chest 
                    spriteBatch.Draw(chestClosedImg, chestClosedRec, Color.White);

                    //crafting table 
                    spriteBatch.Draw(craftingTableImg, craftingTableRec, Color.White);

                    //ending the spriteBatch
                    spriteBatch.End();

                    //displaying the player 
                    DisplayPlayer(camera, displayLast, TILEWIDTH, TILEHEIGHT, mouse);


                    break;
                case CHEST:

                    spriteBatch.Begin();

                    //displaying the chest area on the left 
                    DisplayChest();

                    //displaying the backpack area on the right 
                    DisplayBackpack();

                    //displaying the visible player inventory on the bottom 
                    DisplayInventory();

                    //displaying the drag item 
                    DisplayDrag();

                    spriteBatch.End();

                    break;
                case CRAFTING:

                    spriteBatch.Begin();

                    //display the crafting area 
                    DisplayCrafting();

                    //display the backpack area 
                    DisplayBackpack();

                    //displaying the visible player inventory on the bottom 
                    DisplayInventory();

                    //display the drag item 
                    DisplayDrag();

                    spriteBatch.End();

                    break;
                case ENDGAME:

                    spriteBatch.Begin();

                    //Displaying the background 
                    spriteBatch.Draw(endGameBgImg, endGameBgRec, Color.White);

                    //Displaying the Name of the game 


                    //Displaying the high score 


                    //Displaying the start game button 


                    spriteBatch.End();

                    break;
            }

            base.Draw(gameTime);
        }

        //define your subprograms here 

        //pre  :gridWidth, rectangleWidth, rectangleHeight, and numIndices are positive integers 
        //post :Returns a List<Rectangle> containing all the rectangles for the tileset
        //desc :Generates a list of rectangles representing each tile in a tileset. The position of each rectangle is calculated based on the grid width, tile width, and tile height
        private List<Rectangle> GenerateRectangles(int gridWidth, int rectangleWidth, int rectangleHeight, int numIndices)
        {
            List<Rectangle> textureStore = new();

            for (int i = 0; i < numIndices; i++)
            {
                int x = (i % gridWidth) * rectangleWidth;
                int y = (i / gridWidth) * rectangleHeight;
                textureStore.Add(new Rectangle(x, y, rectangleWidth, rectangleHeight));
            }

            return textureStore;
        }

        //pre  :cam is a valid camera object, layer is a dictionary mapping tile positions to tile indices, sourceRectangles is a list of source rectangles, textureAtlas is a valid texture, and tileWidth and tileHeight are positive integers  
        //post :none
        //desc :Draws the background tiles onto the screen from the perspective of the camera. Each tile's destination rectangle is calculated based on its grid position and tile dimensions, and the corresponding source rectangle is drawn from the texture atlas
        private void DisplayLayer(Cam2D cam, Dictionary<Vector2, int> layer, List<Rectangle> sourceRectangles, Texture2D textureAtlas, int tileWidth, int tileHeight)
        {
            //STEP 8: Begin Drawing your World from the camera's perspective
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.GetTransformation());

            //Draw Background
            foreach (var item in layer)
            {
                Rectangle dest = new(
                    (int)item.Key.X * tileWidth,
                    (int)item.Key.Y * tileHeight,
                    tileWidth,
                    tileHeight
                    );
                Rectangle src = sourceRectangles[item.Value];
                spriteBatch.Draw(textureAtlas, dest, src, Color.White);
            }

            //The spriteBatch still ends in the standard way
            spriteBatch.End();
        }

        //pre : cam is a valid camera object, layer is a dictionary mapping tile positions to tile indices, sourceRectangles is a list of source rectangles, textureAtlas is a valid texture, and tileWidth and tileHeight are positive integers 
        //post : none
        //desc :Draws irregular house tiles onto the screen and creates rectangles for each house to track player collisions. Each tile's destination rectangle is calculated based on its grid position and tile dimensions, with scaling applied to account for growth factors
        private void DisplayHouse(Cam2D cam, Dictionary<Vector2, int> layer, List<Rectangle> sourceRectangles, Texture2D textureAtlas, int tileWidth, int tileHeight)
        {
            //STEP 8: Begin Drawing your World from the camera's perspective
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.GetTransformation());

            int growthFactor = TILESIZE / 16;
            int i = 0;

            //Draw Background
            foreach (var item in layer)
            {
                //creating a list of rectangles with similar dimensions as the show rectangle to track player collision with the houses 
                houseList[i] = new Rectangle((int)item.Key.X * TILESIZE,
                    (int)((item.Key.Y * TILESIZE) - ((tileHeight * growthFactor) - TILESIZE)),
                    tileWidth * growthFactor,
                    tileHeight * growthFactor);

                //creating a destination rectangle for the 
                Rectangle dest = new(
                    (int)item.Key.X * TILESIZE,
                    (int)((item.Key.Y * TILESIZE) - ((tileHeight * growthFactor) - TILESIZE)),
                    tileWidth * growthFactor,
                    tileHeight * growthFactor
                    );

                Rectangle src = sourceRectangles[item.Value];
                spriteBatch.Draw(textureAtlas, dest, src, Color.White);
                i++;
            }

            //The spriteBatch still ends in the standard way
            spriteBatch.End();
        }

        //pre : Cam is a valid camera object, layer is a dictionary mapping tile positions to tile indices, sourceRectangles is a list of source rectangles, textureAtlas is a valid texture, and tileWidth and tileHeight are positive integers
        //post : none
        //desc : Draws decorative tiles (such as lamps) onto the screen. Each tile's destination rectangle is calculated based on its grid position and tile dimensions, with scaling applied to account for growth factors
        private void DisplayDecoration(Cam2D cam, Dictionary<Vector2, int> layer, List<Rectangle> sourceRectangles, Texture2D textureAtlas, int tileWidth, int tileHeight)
        {
            //STEP 8: Begin Drawing your World from the camera's perspective
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.GetTransformation());

            int growthFactor = TILESIZE / 16;

            //Draw Background
            foreach (var item in layer)
            {
                //creating a destination rectangle for the 
                Rectangle dest = new(
                    (int)item.Key.X * TILESIZE,
                    (int)((item.Key.Y * TILESIZE) - ((tileHeight * growthFactor) - TILESIZE)),
                    tileWidth * growthFactor,
                    tileHeight * growthFactor
                    );

                Rectangle src = sourceRectangles[item.Value];
                spriteBatch.Draw(textureAtlas, dest, src, Color.White);
            }

            //The spriteBatch still ends in the standard way
            spriteBatch.End();
        }

        //pre : Cam is a valid camera object, layer is a dictionary mapping tile positions to tile indices, sourceRectangles is a list of source rectangles, textureAtlas is a valid texture, and tileWidth and tileHeight are positive integers 
        //post : none
        //desc : Displays irregular relic tiles on the tile grid and creates rectangles to check for player interactions with relics. Displays the final chest when certain conditions are satisfied. The destination rectangles for the relics and chest are calculated based on their grid positions and scaled using a growth factor
        private void DisplayRelic(Cam2D cam, Dictionary<Vector2, int> layer, List<Rectangle> sourceRectangles, Texture2D textureAtlas, int tileWidth, int tileHeight)
        {
            //STEP 8: Begin Drawing your World from the camera's perspective
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.GetTransformation());

            int growthFactor = TILESIZE / 16;
            int i = 0;

            //Draw Background
            foreach (var item in layer)
            {
                //creating a list of rectangles with similar dimensions as the show rectangle to track player collision with the relics 
                if (gameState == SECOND_LEVEL)
                {
                    relicList[i] = new Rectangle
                    (
                    (int)item.Key.X * TILESIZE,
                    (int)((item.Key.Y * TILESIZE) - ((tileHeight * growthFactor) - TILESIZE)),
                    tileWidth * growthFactor,
                    tileHeight * growthFactor
                    );
                }
                else if (gameState == FINAL_LEVEL)
                {
                    relicList[3] = new Rectangle
                    (
                    (int)item.Key.X * TILESIZE,
                    (int)((item.Key.Y * TILESIZE) - ((tileHeight * growthFactor) - TILESIZE)),
                    tileWidth * growthFactor,
                    tileHeight * growthFactor
                    );

                    finalChestRec.X = relicList[3].X - 20;
                    finalChestRec.Y = relicList[3].Y + 10;
                }


                //creating a destination rectangle for the 
                Rectangle dest = new
                (
                    (int)item.Key.X * TILESIZE,
                    (int)((item.Key.Y * TILESIZE) - ((tileHeight * growthFactor) - TILESIZE)),
                    tileWidth * growthFactor,
                    tileHeight * growthFactor
                );

                int itemValue = item.Value;

                if (playerRec.Intersects(dest) && kb.IsKeyDown(Keys.F))
                {
                    relic1 = true;
                }

                if (relic1)
                {
                    itemValue = 1;
                }

                Rectangle src = sourceRectangles[itemValue];

                switch (gameState)
                {
                    case SECOND_LEVEL:

                        spriteBatch.Draw(textureAtlas, dest, src, Color.White);

                        break;
                    case FINAL_LEVEL:
                        if (quest3Status == false)
                        {
                            spriteBatch.Draw(textureAtlas, dest, src, Color.White);
                        }
                        else if (quest3Status == true)
                        {
                            spriteBatch.Draw(finalChestImg, finalChestRec, Color.White);
                        }
                        break;
                }

                i++;
            }

            //The spriteBatch still ends in the standard way
            spriteBatch.End();
        }

        //pre : cam is a valid camera object, layer is a dictionary mapping tile positions to tile indices, tileWidth and tileHeight are positive integers, and mouseState is the current state of the mouse
        //post : none
        //desc : Displays the player on top of the backgrounds. Player animations and debug rectangles are drawn. The world is rendered from the camera's perspective
        private void DisplayPlayer(Cam2D cam, Dictionary<Vector2, int> layer, int tileWidth, int tileHeight, MouseState mouseState)
        {
            //STEP 8: Begin Drawing your World from the camera's perspective
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.GetTransformation());

            //display the player animations 
            DisplayPlayerAnimations();

            //Displaying the debug rectangles around the player 
            DebugRecs(intersections);

            //The spriteBatch still ends in the standard way
            spriteBatch.End();
        }

        //pre  :target is a valid rectangle
        //post :Returns a List<Rectangle> containing all the rectangles that intersect with the target rectangle horizontally
        //desc :Calculates and returns a list of rectangles intersecting with the target rectangle horizontally. The width and height of the target rectangle are divided into tiles, and intersecting rectangles are added to the list based on their grid positions
        private List<Rectangle> getIntersectingTilesHorizontal(Rectangle target)
        {

            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % TILEWIDTH)) / TILEWIDTH;
            int heightInTiles = (target.Height - (target.Height % TILEHEIGHT)) / TILEHEIGHT;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {

                    intersections.Add(new Rectangle(

                        (target.X + x * TILESIZE) / TILESIZE,
                        (target.Y + y * (TILESIZE - 1)) / TILESIZE,
                        TILESIZE,
                        TILESIZE

                    ));

                }
            }

            return intersections;
        }

        //pre  :target is a valid rectangle
        //post :Returns a List<Rectangle> containing all the rectangles that intersect with the target rectangle vertically
        //desc :Calculates and returns a list of rectangles intersecting with the target rectangle vertically. The width and height of the target rectangle are divided into tiles, and intersecting rectangles are added to the list based on their grid positions
        private List<Rectangle> getIntersectingTilesVertical(Rectangle target)
        {

            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightInTiles = (target.Height - (target.Height % TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {

                    intersections.Add(new Rectangle(

                        (target.X + x * (TILESIZE - 1)) / TILESIZE,
                        (target.Y + y * TILESIZE) / TILESIZE,
                        TILESIZE,
                        TILESIZE

                    ));

                }
            }

            return intersections;
        }


        //pre  :gameTime is a valid GameTime object, and collisionLayer is a dictionary mapping tile positions to tile indices
        //post :none
        //desc :Handles player movement based on user input (WASD keys) and updates various positions such as the mouse and player for other game logic. Updates player animations, state, and combat aspect. Re-centers the camera on the player and detects player and mouse coordinates relative to the world
        private void UpdatePlayerMovement(GameTime gameTime, Dictionary<Vector2, int> collisionLayer)
        {
            float timePassed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Reset speed for frame-based input
            playerSpeed.X = 0f;
            playerSpeed.Y = 0f;

            // Update player position based on rectangle position
            playerPos.X = playerRec.X;
            playerPos.Y = playerRec.Y;

            // Update animations
            playerAnims[playerState].Update(gameTime);

            // Update player state
            attackCooldown.Update(gameTime);
            PlayerStateUpdate();

            //updating combat aspect 
            CombatUpdate();

            // Movement logic
            if (kb.IsKeyDown(Keys.D))
            {
                dirX = FORWARD;
                playerSpeed.X = dirX * maxPLayerSpeed * timePassed;
                playerPos.X = MoveHorizontal(playerSpeed.X, playerRec, playerPos);
                if (playerState != SLASH && playerState != STAB && playerState != HEAVYATTACK)
                {
                    playerIdle = true;
                    playerState = MOVERIGHT;
                }
            }
            else if (kb.IsKeyDown(Keys.A))
            {
                dirX = BACKWARD;
                playerSpeed.X = dirX * maxPLayerSpeed * timePassed;
                playerPos.X = MoveHorizontal(playerSpeed.X, playerRec, playerPos);
                if (playerState != SLASH && playerState != STAB && playerState != HEAVYATTACK)
                {
                    playerIdle = true;
                    playerState = MOVELEFT;
                }
            }
            else if (playerSpeed.X == 0)
            {
                if (playerState == MOVERIGHT || playerState == MOVELEFT)
                {
                    playerIdle = true;
                    playerState = IDLE;
                }
            }

            // Handle attack animations
            if (!playerIdle && playerAnims[playerState].IsFinished())
            {
                playerAnims[playerState].Deactivate();
                playerState = IDLE;
                playerIdle = true;
            }

            if (kb.IsKeyDown(Keys.W))
            {
                // Move Up
                dirY = BACKWARD;
                playerSpeed.Y = dirY * maxPLayerSpeed * timePassed;
                playerPos.Y = MoveVertical(playerSpeed.Y, playerRec, playerPos);
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                // Move Down
                dirY = FORWARD;
                playerSpeed.Y = dirY * maxPLayerSpeed * timePassed;
                playerPos.Y = MoveVertical(playerSpeed.Y, playerRec, playerPos);
            }

            // Update player's position and rectangle after collision resolution
            playerRec.X = (int)playerPos.X;
            playerRec.Y = (int)playerPos.Y;

            HandleCollisions(collisionLayer);

            // Detect player coordinates relative to the world for display purposes
            playerScreenLoc = camera.WorldToScreen(new Vector2(playerRec.X, playerRec.Y));

            // Re-center the camera on the player
            camera.LookAt(playerRec);

            // Detect mouse coordinates relative to the world
            mouseWorldLoc = camera.ScreenToWorld(new Vector2(mouse.X, mouse.Y));
        }

        //pre  :collisionLayer is a dictionary mapping tile positions to tile indices
        //post :none
        //desc :Checks for intersections between the player's rectangle and each rectangle in the collision layer. If an intersection is detected, prevents the player from crossing the colliding rectangle by adjusting the player's position
        private void HandleCollisions(Dictionary<Vector2, int> collisionLayer)
        {
            // Get intersecting tiles for both horizontal and vertical movements
            var intersections = getIntersectingTilesHorizontal(playerRec);
            intersections = getIntersectingTilesVertical(playerRec);

            // Resolve horizontal collisions
            foreach (var rect in intersections)
            {
                if (collisionLayer.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {
                    Rectangle collision = new(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );
                    if (playerRec.Intersects(collision))
                    {
                        if (collision.Contains(playerRec.Center.X, playerRec.Top))
                        {
                            playerPos.Y = collision.Bottom;
                            break;
                        }
                        else if (collision.Contains(playerRec.Center.X, playerRec.Bottom))
                        {
                            playerPos.Y = collision.Top - playerRec.Height;
                            break;
                        }
                        else if (collision.Contains(playerRec.Right, playerRec.Center.Y))
                        {
                            playerPos.X = collision.Left - playerRec.Width;
                            break;
                        }
                        else if (collision.Contains(playerRec.X, playerRec.Center.Y))
                        {
                            playerPos.X = collision.Right;
                            break;
                        }
                    }

                }
            }

            // Update player's position and rectangle after collision resolution
            playerRec.X = (int)playerPos.X;
            playerRec.Y = (int)playerPos.Y;
        }

        //pre  :amount is the speed in float, targetRec is a valid rectangle, targetPos is a valid vector, and movement is restricted within worldBounds
        //post :Returns the changed X position of the rectangle after each frame
        //desc :Moves the player horizontally, clamping the player's position between the left and right edges of the world. Returns the updated X position of the rectangle
        private float MoveHorizontal(float amount, Rectangle targetRec, Vector2 targetPos)
        {
            //Move the player vertically depending on movement restrictions
            //Clamp the player between the left (worldBounds.X) and 
            //right (worldBounds.Right - player1Rec.Width) edges of the world
            targetPos.X += amount;
            targetRec.X = (int)targetPos.X;

            targetRec.X = MathHelper.Clamp(targetRec.X, worldBounds.X + (int)(1.5 * TILESIZE), worldBounds.Right - targetRec.Width - (int)(1.5 * TILESIZE));
            return targetRec.X;
        }

        //pre  :amount is the speed in float, targetRec is a valid rectangle, targetPos is a valid vector, and movement is restricted within worldBounds 
        //post :Returns the player's changed Y position after each frame 
        //desc :Moves the player vertically, clamping the player's position between the top and bottom edges of the world. Returns the updated Y position of the rectangle
        private float MoveVertical(float amount, Rectangle targetRec, Vector2 targetPos)
        {
            //Move the player vertically depending on movement restrictions

            //Clamp the player between the top (worldBounds.Y) and 
            //bottom (worldBounds.Bottom - player1Rec.Height) edges of the world
            targetPos.Y += amount;
            targetRec.Y = (int)targetPos.Y;

            targetRec.Y = MathHelper.Clamp(targetRec.Y, worldBounds.Y + (int)(1.5 * TILESIZE), worldBounds.Bottom - targetRec.Height - (int)(1.5 * TILESIZE));
            return targetRec.Y;
        }


        //pre  :intersections is a list of rectangles
        //post :none
        //desc :Iterates through the list of rectangles and draws them on the screen using a different sub-method (DrawRectHollow)
        private void DebugRecs(List<Rectangle> intersections)
        {
            foreach (var rect in intersections)
            {

                DrawRectHollow(
                    spriteBatch,
                    new Rectangle(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    ),
                    4
                );

            }
        }

        //pre  :spriteBatch is a valid SpriteBatch object, rect is a valid rectangle, and thickness is an integer representing the border thickness
        //post :none
        //desc :Draws a hollow rectangle with the specified thickness for the top, right, left, and bottom edges when in debug mode
        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );

        }

        //pre : houseList is an array of rectangles
        //post: none
        //desc: : Checks if the player intersects with any house rectangle. If the player presses "H" while intersecting, changes the game state to house
        public void HouseCheck(Rectangle[] houseList)
        {
            int i = 0;

            //ensuring that this code only runs after the first draw call as house list rectangles are defined in the first draw call 
            if (houseList[i] != null)
            {
                foreach (Rectangle rect in houseList)
                {
                    if (playerRec.Intersects(houseList[i]) && kb.IsKeyDown(Keys.H))
                    {
                        houseState = i;
                        gameState = HOUSE;
                    }
                    else if (playerRec.Intersects(houseList[i]))
                    {
                        //collision logic to keep the player from crossing over the house 
                    }
                    i++;
                }
            }
        }



        //Pre : target rectangle
        //post : none
        //desc :Checks for collisions between the player and objects like chests and crafting tables, preventing the player from passing through these objects by adjusting the player's position
        private void ObjectCollisionPlayer(Rectangle targetRec)
        {
            if (playerRec.Intersects(targetRec))
            {
                if (targetRec.Contains(playerRec.Center.X, playerRec.Top))
                {
                    playerPos.Y = targetRec.Bottom;
                }
                else if (targetRec.Contains(playerRec.Center.X, playerRec.Bottom))
                {
                    playerPos.Y = targetRec.Top - playerRec.Height;
                }
                else if (targetRec.Contains(playerRec.Right, playerRec.Center.Y))
                {
                    playerPos.X = targetRec.Left - playerRec.Width;
                }
                else if (targetRec.Contains(playerRec.X, playerRec.Center.Y))
                {
                    playerPos.X = targetRec.Right;
                }
                playerRec.X = (int)playerPos.X;
                playerRec.Y = (int)playerPos.Y;
            }
        }

        //pre :none
        //post : none
        //desc :Updates the crafting items based on specific combinations of items. Sets the resulting crafted item and resets the crafting slots
        private void UpdateCrafting()
        {
            for (int i = 0; i < craftingItems.Length - 1; i++)
            {
                if ((craftingItems[i] == 1 && craftingItems[i + 1] == 2) || (craftingItems[i] == 2 && craftingItems[i + 1] == 1))
                {
                    craftingItems[2] = 7;
                    craftingItems[1] = 0;
                    craftingItems[0] = 0;
                }
                else if ((craftingItems[i] == 3 && craftingItems[i + 1] == 4) || (craftingItems[i] == 4 && craftingItems[i + 1] == 3))
                {
                    craftingItems[2] = 8;
                    craftingItems[1] = 0;
                    craftingItems[0] = 0;
                }
            }
        }


        //pre  :none
        //post :none
        //Desc :Handles the logic for dragging and dropping items between the player's inventory, backpack, chest, and crafting table. Updates the position of the dragged item to follow the mouse cursor 
        private void UpdateDrag()
        {

            Vector2 mousePosition = new Vector2(mouse.Position.X, mouse.Position.Y);

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (!pressed)
                {
                    // Check inventory
                    if (inventoryBgRec.Contains(mousePosition))
                    {
                        for (int i = 0; i < inventoryDrawRecs.Length; i++)
                        {
                            if (inventoryDrawRecs[i].Contains(mousePosition) && inventoryQuantities[i] != 0)
                            {
                                handItem = inventoryItems[i];
                                inventoryQuantities[i]--;
                                location1D = i;
                                whereFrom = CHECKINVENTORY;
                                break;
                            }
                        }
                    }
                    // Check backpack
                    else if (backpackBgRec.Contains(mousePosition))
                    {
                        for (int i = 0; i < backpackDrawRecs.GetLength(0); i++)
                        {
                            for (int j = 0; j < backpackDrawRecs.GetLength(1); j++)
                            {
                                if (backpackDrawRecs[i, j].Contains(mousePosition) && backpackItems[i, j] != 0)
                                {
                                    handItem = backpackItems[i, j];
                                    location2DX = i;
                                    location2DY = j;
                                    whereFrom = CHECKBACKPACK;
                                    break;
                                }
                            }
                            if (handItem != 0) break;
                        }
                    }
                    // Check chest
                    else if (chestBgRec.Contains(mousePosition) && gameState == CHEST)
                    {
                        for (int j = 0; j < chestDrawRecs.GetLength(1); j++)
                        {
                            if (chestDrawRecs[houseState, j].Contains(mousePosition) && chestItems[houseState, j] != 0)
                            {
                                handItem = chestItems[houseState, j];
                                location2DX = houseState;
                                location2DY = j;
                                whereFrom = CHECKCHEST;
                                break;
                            }
                        }
                    }
                    // Check crafting
                    else if (craftingBgRec.Contains(mousePosition) && gameState == CRAFTING)
                    {
                        for (int i = 0; i < craftingItems.Length; i++)
                        {
                            if (craftingDrawRecs[i].Contains(mousePosition))
                            {
                                handItem = craftingItems[i];
                                location1D = i;
                                whereFrom = CHECKCRAFTINGTABLE;
                                break;
                            }
                        }
                    }

                    pressed = true;
                    handItemRec.X = mouse.Position.X;
                    handItemRec.Y = mouse.Position.Y;
                }
            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                if (inventoryBgRec.Contains(mousePosition))
                {
                    for (int i = 0; i < inventoryItems.Length; i++)
                    {
                        if (inventoryItems[i] == 0 && handItem != 7)
                        {
                            inventoryItems[i] = handItem;
                            inventoryQuantities[i]++;
                            dropped = true;
                            break;
                        }
                        else if (i == 3 && handItem == 7)
                        {
                            inventoryItems[i] = handItem;
                            inventoryQuantities[i]++;
                            dropped = true;
                            break;
                        }
                    }
                }
                else if (backpackBgRec.Contains(mousePosition))
                {
                    for (int i = 0; i < backpackItems.GetLength(0); i++)
                    {
                        for (int j = 0; j < backpackItems.GetLength(1); j++)
                        {
                            if (backpackItems[i, j] == 0)
                            {
                                backpackItems[i, j] = handItem;
                                dropped = true;
                                break;
                            }
                        }
                        if (dropped) break;
                    }
                }
                else if (chestBgRec.Contains(mousePosition) && gameState == CHEST)
                {
                    for (int j = 0; j < chestItems.GetLength(1); j++)
                    {
                        if (chestItems[houseState, j] == 0)
                        {
                            chestItems[houseState, j] = handItem;
                            dropped = true;
                            break;
                        }
                    }
                }
                else if (craftingBgRec.Contains(mousePosition) && gameState == CRAFTING)
                {
                    for (int i = 0; i < craftingItems.Length; i++)
                    {
                        if (craftingItems[i] == 0 && i != 2)
                        {
                            craftingItems[i] = handItem;
                            dropped = true;
                            break;
                        }
                    }
                }

                if (dropped)
                {
                    switch (whereFrom)
                    {
                        case CHECKINVENTORY:
                            if (location1D == 3)
                            {
                                if (inventoryQuantities[location1D] == 0)
                                {
                                    inventoryItems[location1D] = 0;
                                }
                            }
                            else
                            {
                                inventoryItems[location1D] = 0;
                            }
                            break;
                        case CHECKBACKPACK:
                            backpackItems[location2DX, location2DY] = 0;
                            break;
                        case CHECKCHEST:
                            chestItems[location2DX, location2DY] = 0;
                            break;
                        case CHECKCRAFTINGTABLE:
                            craftingItems[location1D] = 0;
                            break;
                    }

                    handItem = 0;
                    location1D = -1;
                    location2DX = -1;
                    location2DY = -1;
                    whereFrom = -1;
                    pressed = false;
                    dropped = false;
                }
            }

            // Update handItemRec position to follow the mouse cursor
            if (handItem != 0)
            {
                handItemRec.X = mouse.Position.X;
                handItemRec.Y = mouse.Position.Y;
            }
        }


        //Pre  : None 
        //Post : None 
        //Desc : Displays the quick access inventory of the player, showing only the non-empty slots
        private void DisplayInventory()
        {
            //displaying the background 
            spriteBatch.Draw(backpackBgImg, inventoryBgRec, Color.White);

            //loop through the items in the inventory 
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                //Only display the non 0 items (0 is an empty cell)
                if (inventoryItems[i] != 0)
                {
                    //display each item in the inventory rectangles(slots)
                    spriteBatch.Draw(itemTextures[inventoryItems[i]], inventoryDrawRecs[i], Color.White);
                }
            }
        }

        //pre  :None
        //post :None
        //Desc :Displays the item being dragged by the player, updating its position to follow the mouse cursor 
        private void DisplayDrag()
        {
            if (handItem != 0)
            {
                spriteBatch.Draw(itemTextures[handItem], handItemRec, Color.White);
            }
        }

        //pre  :None
        //post :None
        //Desc :Displays the items in the player's chest, showing only the non-empty slots
        private void DisplayChest()
        {
            //displaying the background 
            spriteBatch.Draw(backpackBgImg, chestBgRec, Color.White);

            //loop through the items in the inventory 
            for (int i = 0; i < chestItems.GetLength(1); i++)
            {
                //Only display the non 0 items (0 is an empty cell)
                if (chestItems[houseState, i] != 0)
                {
                    //display each item in the inventory rectangles(slots)
                    spriteBatch.Draw(itemTextures[chestItems[houseState, i]], chestDrawRecs[houseState, i], Color.White);
                }
            }

        }

        //pre  :None
        //post :None
        //Desc :Displays the player's backpack, showing only the non-empty slots 
        private void DisplayBackpack()
        {
            //displaying the background 
            spriteBatch.Draw(backpackBgImg, backpackBgRec, Color.White);

            //loop through the items in the inventory 
            for (int i = 0; i < backpackItems.GetLength(0); i++)
            {
                for (int j = 0; j < backpackItems.GetLength(1); j++)
                {
                    //Only display the non 0 items (0 is an empty cell)
                    if (backpackItems[i, j] != 0)
                    {
                        //display each item in the inventory rectangles(slots)
                        spriteBatch.Draw(itemTextures[backpackItems[i, j]], backpackDrawRecs[i, j], Color.White);
                    }
                }
            }
        }

        //pre  :None
        //post :None
        //Desc :Displays the crafting table, showing only the non-empty slots
        private void DisplayCrafting()
        {
            //displaying the background 
            spriteBatch.Draw(backpackBgImg, craftingBgRec, Color.White);

            //loop through the items in the inventory 
            for (int i = 0; i < craftingItems.Length; i++)
            {
                //Only display the non 0 items (0 is an empty cell)
                if (craftingItems[i] != 0)
                {
                    //display each item in the inventory rectangles(slots)
                    spriteBatch.Draw(itemTextures[craftingItems[i]], craftingDrawRecs[i], Color.White);
                }
            }
        }

        //pre :None
        //post:None
        //desc:Updates the player's state based on health, attack cooldowns, and input from the mouse and keyboard. Handles player death and attack animations
        private void PlayerStateUpdate()
        {

            if (!playerAlive) return;

            //Handle cooldowns
            if (attackCooldown.IsActive()) return;

            if (attackCooldown.IsFinished())
            {
                if (playerState != DEATH)
                {
                    attackCooldown.ResetTimer(true);
                    playerState = IDLE;
                    playerIdle = true;
                }
                else
                {
                    playerAlive = false;
                }
                attackCooldown.Deactivate();
            }

            // Handle mouse button inputs for attacks
            if (mouse.LeftButton == ButtonState.Pressed && playerState != SLASH)
            {
                playerState = SLASH;
                playerIdle = false;
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(500);
            }
            else if (mouse.RightButton == ButtonState.Pressed && playerState != DEFEND)
            {
                playerState = DEFEND;
                playerIdle = false;
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(500);
            }

            // Handle keyboard inputs for various attack types
            if (kb.IsKeyDown(Keys.Z) && playerState != HEAVYATTACK)
            {
                playerState = HEAVYATTACK;
                playerIdle = false;
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(1000);
            }
            else if (kb.IsKeyDown(Keys.X) && playerState != SLICE)
            {
                playerState = SLICE;
                playerIdle = false;
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(500);
            }
            else if (kb.IsKeyDown(Keys.C) && playerState != STAB)
            {
                playerState = STAB;
                playerIdle = false;
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(500);
            }
            else if (kb.IsKeyDown(Keys.G) && playerState != SHOOT)
            {
                playerState = SHOOT;
                playerIdle = false;
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(1000);
            }

            // Check if the player is dead
            if (playerHealth <= 0)
            {
                playerState = DEATH;
                ActivateAnimation(playerState);
                attackCooldown.Activate();
                attackCooldown.SetTargetTime(4000);
                return;
            }

        }


        //pre :None
        //post:None
        //desc:Updates combat interactions between the player and enemies. Handles player attacks on enemies and vice versa, updating health and states accordingly. Also updates projectiles
        private void CombatUpdate()
        {
            // Handle player attacks on enemies
            if (!playerIdle)
            {
                PerformPlayerAttack();
            }

            // Handle enemies attacking the player
            for (int i = 0; i < knightsRec.Length; i++)
            {
                if (knightsAlive[i] && playerAnims[playerState].GetDestRec().Intersects(knightsRec[i]) && !playerIdle && knightsState[i] != DEATH)
                {
                    if (knightsHealth[i] <= 0)
                    {
                        knightsState[i] = DEATH; // Change state to death when health is 0
                    }
                }
            }

            for (int i = 0; i < zombiesRec.Length; i++)
            {
                if (zombiesAlive[i] && zombiesState[i] != DEATH)
                {
                    if (zombiesHealth[i] <= 0)
                    {
                        zombiesState[i] = DEATH; // Change state to death
                    }
                }
            }

            for (int i = 0; i < skeletonsRec.Length; i++)
            {
                if (skeletonsAlive[i] && playerAnims[playerState].GetDestRec().Intersects(skeletonsRec[i]) && !playerIdle && skeletonsState[i] != DEATH)
                {
                    if (skeletonsHealth[i] <= 0)
                    {
                        skeletonsState[i] = DEATH; // Change state to death
                    }
                }
            }

            //updating the projectiles 
            UpdateProjectiles();

        }

        //pre :None
        //post:None
        //desc:Executes the player's attack based on the current state and applies damage to the nearest enemy. Handles various attack types and initiates animations
        private void PerformPlayerAttack()
        {
            if ((!playerAlive) || playerIdle) return;

            int damage = 0;

            switch (playerState)
            {
                case SLASH:
                    damage = 20;
                    break;

                case STAB:
                    damage = 25;
                    break;

                case HEAVYATTACK:
                    damage = 40;
                    break;

                case SLICE:
                    damage = 30;
                    break;

                case SHOOT:
                    damage = 15;
                    ShootProjectile(mouse.Position.ToVector2());
                    break;

                case DEATH:
                    break;

            }

            ActivateAnimation(playerState);

            if (playerState != DEFEND && playerState != DEATH)
            {
                ApplyDamageToNearestEnemy(damage);
            }

        }

        //pre :None
        //post:None
        //desc:Activates and manages the player's animations based on the current state. Ensures the correct animation plays and deactivates it when finished
        private void ActivateAnimation(int state)
        {
            if (!playerAnims[state].IsAnimating())
            {
                playerAnims[state].Activate(true); // Start the animation
            }

            if (playerAnims[state].IsFinished())
            {
                playerAnims[state].Deactivate();

                if (state != DEATH && state != DEFEND)
                {
                }
            }
        }

        //pre :None
        //post:None
        //desc:Calculates the nearest enemy to the player and applies damage to that enemy based on the player's attack state
        private void ApplyDamageToNearestEnemy(int damage)
        {
            float nearestDistance = 1000000000000;
            int nearestEnemyIndex = -1;
            string enemyType = "";

            // Check knights
            for (int i = 0; i < 5; i++)
            {
                if (knightsAlive[i])
                {
                    float distance = Vector2.Distance(knightsPos[i], knightsPos[i]);
                    if (distance < nearestDistance && distance < 20.0f)
                    {
                        nearestDistance = distance;
                        nearestEnemyIndex = i;
                        enemyType = "Knight";
                    }
                }
            }

            // Check zombies
            for (int i = 0; i < zombiesRec.Length; i++)
            {
                if (zombiesAlive[i])
                {
                    float distance = Vector2.Distance(playerPos, zombiesPos[i]);
                    if (distance < nearestDistance && distance < 20.0f)
                    {
                        nearestDistance = distance;
                        nearestEnemyIndex = i;
                        enemyType = "Zombie";
                    }
                }
            }

            // Check skeletons
            for (int i = 0; i < skeletonsRec.Length; i++)
            {
                if (skeletonsAlive[i])
                {
                    float distance = Vector2.Distance(playerPos, skeletonsPos[i]);
                    if (distance < nearestDistance && distance < 20.0f)
                    {
                        nearestDistance = distance;
                        nearestEnemyIndex = i;
                        enemyType = "Skeleton";
                    }
                }
            }

            // Check witch
            if (witchAlive)
            {
                float distance = Vector2.Distance(playerPos, witchPos);
                if (distance < nearestDistance && distance < 20.0f)
                {
                    nearestDistance = distance;
                    nearestEnemyIndex = -1; // Witch doesn't use array
                    enemyType = "Witch";
                }
            }

            // Apply damage to the nearest enemy
            if (nearestEnemyIndex != -1)
            {
                switch (enemyType)
                {
                    case "Knight":
                        knightsHealth[nearestEnemyIndex] -= damage;
                        break;
                    case "Zombie":
                        zombiesHealth[nearestEnemyIndex] -= damage;
                        break;
                    case "Skeleton":
                        skeletonsHealth[nearestEnemyIndex] -= damage;
                        break;
                }
            }
            else if (enemyType == "Witch")
            {
                witchHealth -= damage;
            }
        }

        //pre :targetPosition is a valid Vector2 representing the target position for the projectile
        //post:None
        //desc:Shoots a projectile towards the target position by setting up its position, velocity, and damage. Ensures there is space for a new projectile and increments the active projectile count
        private void ShootProjectile(Vector2 targetPosition)
        {
            // Ensure there's space for a new projectile
            if (activeProjectiles >= 10) return;

            // Calculate direction and normalize it
            Vector2 direction = targetPosition - playerPos;
            direction.Normalize();

            // Set up the projectile's position, velocity, and damage
            projectilePositions[activeProjectiles] = playerPos;
            projectileVelocities[activeProjectiles] = direction * 10f; // Speed of projectile
            projectileDamage[activeProjectiles] = 15; // Set the damage
            projectileIsAlive[activeProjectiles] = true;

            // Increment the active projectile count
            activeProjectiles++;
        }

        //pre :None
        //post:None
        //desc:Updates the positions of active projectiles and checks for collisions with enemies. Deactivates projectiles upon collision or if they go off-screen
        private void UpdateProjectiles()
        {
            for (int i = 0; i < activeProjectiles; i++)
            {
                if (!projectileIsAlive[i]) continue;

                // Move the projectile
                projectilePositions[i] += projectileVelocities[i];

                // Check collision with nearest enemies 
                if (CheckCollisionWithEnemies(i, knightsRec, knightsHealth))
                {
                    // If it collides, apply damage and deactivate projectile
                    projectileIsAlive[i] = false;
                }

                if (CheckCollisionWithEnemies(i, zombiesRec, zombiesHealth))
                {
                    // If it collides, apply damage and deactivate projectile
                    projectileIsAlive[i] = false;
                }

                if (CheckCollisionWithEnemies(i, skeletonsRec, skeletonsHealth))
                {
                    // If it collides, apply damage and deactivate projectile
                    projectileIsAlive[i] = false;
                }

                // Optional: Remove projectiles that go off-screen
                if (projectilePositions[i].X < 0 || projectilePositions[i].X > screenWidth ||
                    projectilePositions[i].Y < 0 || projectilePositions[i].Y > screenHeight)
                {
                    projectileIsAlive[i] = false;
                }
            }
        }

        //pre :projectileIndex is the index of the projectile being checked. enemyrectangle is an array of enemy rectangles, and enemyHealth is an array of enemy health values
        //post:Returns true if a collision with an enemy is detected, otherwise false
        //desc:Checks for collisions between the specified projectile and enemies. Applies damage to enemies upon collision
        private bool CheckCollisionWithEnemies(int projectileIndex, Rectangle[] enemyrectangle, int[] enemyHealth)
        {
            // Check collision with all enemies (replace this with your actual collision logic)
            for (int i = 0; i < enemyrectangle.Length; i++)
            {
                if (projectileIsAlive[i] && Vector2.Distance(projectilePositions[projectileIndex], enemyrectangle[i].Location.ToVector2()) < 10.0f)
                {
                    enemyHealth[i] -= projectileDamage[projectileIndex];

                    return true; // Projectile hits enemy
                }
            }

            return false; // No collision
        }

        //pre :spriteBatch is a valid SpriteBatch object
        //post:None
        //desc:Draws the active projectiles on the screen using the provided SpriteBatch
        private void DrawProjectiles(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < activeProjectiles; i++)
            {
                if (projectileIsAlive[i])
                {
                    spriteBatch.Draw(itemTextures[inventoryItems[3]], projectilePositions[i], Color.White);
                }
            }
        }

        //pre :None
        //post:None
        //desc:Displays the player's animations based on the player's state. Adjusts the animation position and orientation
        private void DisplayPlayerAnimations()
        {
            if (playerState == MOVELEFT)
            {
                playerAnims[playerState].TranslateTo(playerPos.X - 41, playerPos.Y - 40);
                playerAnims[playerState].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
            }
            else
            {
                playerAnims[playerState].TranslateTo(playerPos.X - 41, playerPos.Y - 40);
                playerAnims[playerState].Draw(spriteBatch, Color.White);
            }
        }

        //pre :followerState, followerWander, followerRecs, targetPoints, followPos, followEnemy, followCuePoint, followerSpeed, followerAnimations, gameTime, and followAlive are all valid and initialized
        //post:None
        //desc:Updates the state of followers (enemies) based on their health, distance to the player, and target points. Handles enemy attacks, movement, and animations
        private void followerStateUpdate(int[] followerState, bool[] followerWander, Rectangle[] followerRecs, Vector2[,,] targetPoints, Vector2[] followerPos, int followEnemy, int[] followCuePoint, Vector2[] followerSpeed, Animation[,] followerAnimations, GameTime gameTime, bool[] followAlive)
        {

            for (int i = 0; i < followerState.Length; i++)
            {

                Vector2 dist;
                Vector2 distToPoint;
                Rectangle followerRec;

                enemyCooldown[followEnemy, i].Update(gameTime);

                if (!followAlive[i]) return;

                //Handle cooldowns
                if (enemyCooldown[followEnemy, i].IsActive()) return;

                if (enemyCooldown[followEnemy, i].IsFinished())
                {
                    if (followerState[i] != DEATH)
                    {
                        enemyCooldown[followEnemy, i].ResetTimer(true);
                        followerState[i] = IDLE;
                        followerWander[i] = true;
                    }
                    else
                    {
                        followAlive[i] = false;
                    }
                    enemyCooldown[followEnemy, i].Deactivate();
                }

                dist = playerRec.Location.ToVector2() - followerRecs[i].Location.ToVector2();

                if (followAlive[i])
                {
                    if (followerState[i] != DEATH)
                    {
                        // Enemy attacks the player if they are close enough
                        if (playerRec.Intersects(followerRecs[i]) && enemyCooldown[followEnemy, i].IsInactive())
                        {
                            if (followEnemy == 0)
                            {
                                if (knightsHealth[i] < 30)
                                {
                                    followerState[i] = STAB;
                                    followerWander[i] = false;
                                    enemyCooldown[followEnemy, i].Activate();
                                    enemyCooldown[followEnemy, i].SetTargetTime(1000);
                                }
                                else if (knightsHealth[i] < 50)
                                {
                                    followerState[i] = SLICE;
                                    followerWander[i] = false;
                                    enemyCooldown[followEnemy, i].Activate();
                                    enemyCooldown[followEnemy, i].SetTargetTime(1000);
                                }
                                else
                                {
                                    followerState[i] = SLASH;
                                    followerWander[i] = false;
                                    enemyCooldown[followEnemy, i].Activate();
                                    enemyCooldown[followEnemy, i].SetTargetTime(1000);
                                }
                            }
                            else if (followEnemy == 1)
                            {

                                followerState[i] = SLASH;
                                followerWander[i] = false;
                                enemyCooldown[followEnemy, i].Activate();
                                enemyCooldown[followEnemy, i].SetTargetTime(1000);
                            }
                            else if (followEnemy == 2)
                            {
                                followerState[i] = SLASH;
                                followerWander[i] = false;
                                enemyCooldown[followEnemy, i].Activate();
                                enemyCooldown[followEnemy, i].SetTargetTime(1000);
                            }

                            // Play attack animation
                            followerAnimations[i, followerState[i]].Activate(true);

                            // Perform the attack
                            PerformEnemyAttack(followerRecs[i], followerState[i]);

                        }
                        else if (dist.Length() <= 300.0f)  // Follow player if within range
                        {
                            if (dist.Length() > 0.1f)
                            {
                                dist.Normalize();
                                followerPos[i] = FollowTheTarget(playerPos, followerPos[i]);


                                if (followerWander[i])
                                {
                                    if (dist.X > 0)
                                    {
                                        //setting the enemy state to MOVE RIGHT 
                                        followerState[i] = MOVERIGHT;
                                    }
                                    else if (dist.X < 0)
                                    {
                                        //setting the enemy staate to MOVELEFT 
                                        followerState[i] = MOVELEFT;
                                    }
                                    else
                                    {
                                        //setting the follower state to IDLE 
                                        followerState[i] = IDLE;
                                    }
                                }
                            }
                        }
                        else  // Follow target points if not near the player
                        {
                            //calculating the distance between 
                            distToPoint = targetPoints[followEnemy, i, followCuePoint[i]] - followerRecs[i].Center.ToVector2();

                            //checking if the enemy has almost reached the cue point 
                            if (distToPoint.Length() <= 10.0f)
                            {
                                //updating teh cue point 
                                followCuePoint[i] = (followCuePoint[i] + 1) % targetPoints.GetLength(2);
                            }

                            //normalizing the distance 
                            distToPoint.Normalize();

                            //updating the follower positon with the direction vector as speed 
                            followerPos[i].X += distToPoint.X;
                            followerPos[i].Y += distToPoint.Y;

                            //checking if the vector to cue point is Positive
                            if (distToPoint.X > 0)
                            {
                                followerState[i] = MOVERIGHT;
                            }
                            //checking if the vector to cue point is negative
                            else if (distToPoint.X < 0)
                            {
                                //setting the enemy state to MOVELEFT 
                                followerState[i] = MOVELEFT;
                            }
                            else
                            {
                                //setting the enemy state to IDLE 
                                followerState[i] = IDLE;
                            }
                        }
                    }

                    //updating the enemy rectangle to the follower position 
                    followerRecs[i].X = (int)followerPos[i].X;
                    followerRecs[i].Y = (int)followerPos[i].Y;
                }
            }
        }

        //pre :None
        //post:None
        //desc:Updates the witch's state based on distance to the player and predefined path points. Handles witch attacks, movement, and animations. Activates and manages the witch's cooldown for attacks
        private void witchStateUpdate(GameTime gameTime)
        {
            //creating vectors to temporarily use in the witch state 
            Vector2 dist;
            Vector2 distToPoint;
            Rectangle followerRec;

            //calculating the distnace between the player and the enemy 
            dist = playerRec.Location.ToVector2() - witchRec.Location.ToVector2();

            //updating the witch attack cooldown 
            witchCooldown.Update(gameTime);

            //checking if the witch is alive 
            if (witchAlive)
            {
                //checking if the witch is not in dead state for dead animation  
                if (witchState != DEATH)
                {
                    // Enemy attacks the player if they are close enough
                    if (playerRec.Intersects(witchRec) && witchCooldown.IsInactive())
                    {

                        //only stab if the witch health is less than 500 
                        if (witchHealth < 500)
                        {
                            //setting witch state to stab 
                            witchState = STAB;
                        }
                        else
                        {
                            //setting the witch state to slash 
                            witchState = SLASH;
                        }

                        // Play attack animation
                        witchAnims[witchState].Activate(true);

                        // Perform the attack
                        PerformEnemyAttack(witchRec, witchState);

                        //activating the attack cooldown for enemy 
                        witchCooldown.Activate();

                        //setting the witch wander state to false 
                        witchWander = false;

                    }
                    //checking if the player is in follow range with the enemy 
                    else if (dist.Length() <= 300.0f)
                    {
                        //checking if the distance is not 0 cause sometimes it stops the enemy completely and it gets stuck 
                        if (dist.Length() > 0.1f)
                        {
                            //normalizing the distance to make it into speed vector 
                            dist.Normalize();

                            //making witch follow the player 
                            witchPos = FollowTheTarget(playerPos, witchPos);

                            //checking if witch is wandering 
                            if (witchWander)
                            {
                                //checking if the distance is positive 
                                if (dist.X > 0)
                                {
                                    //setting the witch state to move right 
                                    witchState = MOVERIGHT;
                                }
                                //checking if the distance is negative 
                                else if (dist.X < 0)
                                {
                                    //setting teh witch state to move left 
                                    witchState = MOVELEFT;
                                }
                                else
                                {
                                    //setting the witch state to IDLE 
                                    witchState = IDLE;
                                }
                            }
                        }
                    }
                    else  // Follow target points if not near the player
                    {
                        //finding the distnace to the witch path points 
                        distToPoint = witchPathPoints[witchCuePoint] - witchRec.Center.ToVector2();

                        //checking if the enemy has reached cue point 
                        if (distToPoint.Length() <= 10.0f)
                        {
                            //changing the witch cue point 
                            witchCuePoint = (witchCuePoint + 1) % 5;
                        }

                        //normalizing the distance 
                        distToPoint.Normalize();

                        //adding distance to point as speed 
                        witchPos.X += distToPoint.X;
                        witchPos.Y += distToPoint.Y;

                        //checking if the distance is positve 
                        if (distToPoint.X > 0)
                        {
                            witchState = MOVERIGHT;
                        }
                        //checking if the distance is negative 
                        else if (distToPoint.X < 0)
                        {
                            //setting witch state to move left 
                            witchState = MOVELEFT;
                        }
                        else
                        {
                            //setting witch state to idle 
                            witchState = IDLE;
                        }
                    }

                    //checking if cooldown is finished 
                    if (witchCooldown.IsFinished())
                    {
                        //deactivating cooldown 
                        witchCooldown.Deactivate();
                    }
                }

                //updating the witch rectangle to witch position 
                witchRec.X = (int)witchPos.X;
                witchRec.Y = (int)witchPos.Y;
            }
        }

        //pre :targetAnimations, targetState, targetWander, targetEnemy, and gameTime are all valid and initialized
        //post:None
        //desc:Updates the animations of different types of enemies (knights, zombies, skeletons) based on their state and whether they are wandering or attacking. Resurrects dead enemies if certain conditions are met
        private void targetAnimsUpdate(Animation[,] targetAnimations, int[] targetState, bool[] targetWander, int targetEnemy, GameTime gameTime)
        {
            //looping through that specific enemy 
            for (int i = 0; i < targetAnimations.GetLength(0); i++)
            {
                //checking if the target enemy is knight 
                if (targetEnemy == KNIGHT)
                {
                    //checking if the player pressed relic 1 and the knight is not initialized 
                    if (!knightsAlive[i] && knightsHealth[i] == 100 && relic1)
                    {
                        //initializing the knight to alive 
                        knightsAlive[i] = true;
                    }

                    //updating the knight animation 
                    knightAnims[i, targetState[i]].Update(gameTime);

                    //checking if the knight is not wandering 
                    if (!targetWander[i])
                    {
                        //activating the knight animation for that state 
                        knightAnims[targetEnemy, targetState[i]].Activate(true);

                        //checking if the knight animation is finished 
                        if (knightAnims[targetEnemy, targetState[i]].IsFinished())
                        {
                            //checking if the knight is dead 
                            if (knightsState[i] == DEATH)
                            {
                                //making the knight dead 
                                knightsAlive[i] = false;
                            }
                            else
                            {
                                //setting knight wandering state to true 
                                targetWander[i] = true;
                            }
                            //deactivating the knight animation 
                            knightAnims[i, targetState[i]].Deactivate();
                        }
                    }
                    else // Enemy is wandering
                    {
                        //actiavating the knight wandering animation 
                        knightAnims[i, targetState[i]].Activate(true);
                    }
                }
                //checking if the enemy is knight 
                else if (targetEnemy == KNIGHT)
                {
                    //checking if the player has activated relic 2 and the zombie has not yet been initialized 
                    if (!zombiesAlive[i] && (zombiesHealth[i] == 200) && relic2)
                    {
                        //making the zombie alive 
                        zombiesAlive[i] = true;
                    }

                    //setting zombie animation to true 
                    zombieAnims[i, targetState[i]].Update(gameTime);

                    // checking if the target is not wandering 
                    if (!targetWander[i])
                    {
                        //activating the zombie animation 
                        zombieAnims[targetEnemy, targetState[i]].Activate(true);

                        //checking if the zombie animation is finished 
                        if (zombieAnims[targetEnemy, targetState[i]].IsFinished())
                        {
                            //checking if the zombie is dead 
                            if (zombiesState[i] == DEATH)
                            {
                                //setting dead flag for zombie to false 
                                zombiesAlive[i] = false;
                            }
                            else
                            {
                                //setting the wander to true if the zombie is 
                                targetWander[i] = true;
                            }

                            //deactivating the zombie animation 
                            zombieAnims[i, targetState[i]].Deactivate();
                        }
                    }
                    else // Enemy is wandering
                    {
                        //activating the animation for wandering 
                        zombieAnims[i, targetState[i]].Activate(true);
                    }
                }
                //checking if the target is skeleton 
                else if (targetEnemy == SKELETON)
                {
                    //checking if the player presses relic 3 and the enemey has not yet been initialized 
                    if (!skeletonsAlive[i] && skeletonsHealth[i] == 300 && relic3)
                    {
                        //initializing the skeleton by making it alive 
                        skeletonsAlive[i] = true;
                    }

                    //updating the skeleton animations 
                    skeletonAnims[i, targetState[i]].Update(gameTime);

                    //checking if the skeleton is not wandering 
                    if (!targetWander[i])
                    {
                        //Activating the skeleton animation if wander is false 
                        skeletonAnims[targetEnemy, targetState[i]].Activate(true);

                        //checking if the skeleton animation is fininshed 
                        if (skeletonAnims[targetEnemy, targetState[i]].IsFinished())
                        {
                            ///if the skeleton is dead 
                            if (skeletonsState[i] == DEATH)
                            {
                                //set player dead flag to false 
                                skeletonsAlive[i] = false;
                            }
                            else
                            {
                                //target is not wandering anymore 
                                targetWander[i] = true;
                            }

                            //deactivating skeleton animations 
                            skeletonAnims[i, targetState[i]].Deactivate();
                        }
                    }
                    else
                    {
                        //activating the skeleton animation 
                        skeletonAnims[i, targetState[i]].Activate(true);
                    }
                }

            }
        }

        //pre :targetAnimations and gameTime are valid and initialized
        //post:None
        //desc:Updates the witch's animations based on her state and whether she is wandering or attacking. Resurrects the witch if certain conditions are met
        private void witchAnimsUpdate(Animation[] targetAnimations, GameTime gameTime)
        {
            //looping through all the witch animations 
            for (int i = 0; i < targetAnimations.GetLength(0); i++)
            {
                //checking if the witch has not yet been initialized 
                if (!witchAlive && witchHealth == 1000 && relic4)
                {
                    //spawning witch if the player activated the relic 4 
                    witchAlive = true;
                }

                //updating the witch animation for that state 
                witchAnims[witchState].Update(gameTime);

                //checking if the witch is not wandering 
                if (!witchWander)
                {
                    //activating the witch animation for that state 
                    witchAnims[witchState].Activate(true);

                    //checking if the witch animation is finished 
                    if (witchAnims[witchState].IsFinished())
                    {
                        //checking if the witch is DEAD 
                        if (witchState == DEATH)
                        {
                            //witch is not alive now 
                            witchAlive = false;
                        }
                        else
                        {
                            //making witch to wander 
                            witchWander = true;
                        }
                        //deactivating the witch animation 
                        witchAnims[witchState].Deactivate();
                    }
                }
                else // Enemy is wandering
                {
                    //activating the witch animation 
                    witchAnims[witchState].Activate(true);
                }
            }
        }

        //pre :enemyRect is a valid rectangle representing the enemy's position, and attackType is an integer representing the type of attack
        //post:None
        //desc:Calculates and applies damage to the player if the enemy's attack overlaps with the player's position
        private void PerformEnemyAttack(Rectangle enemyRect, int attackType)
        {
            //calculating the damage done by that attack 
            int damage = CalculateDamage(attackType);

            // Check if the enemy's attack overlaps with the player
            if (playerRec.Intersects(enemyRect))
            {
                //updating the health by subtracting that health 
                playerHealth -= damage;
            }
        }

        //pre :AttackType is an integer representing the type of attack
        //post:Returns an integer value representing the damage based on the attack type
        //desc:Determines the damage value based on the given attack type
        private int CalculateDamage(int attackType)
        {

            //storing the attack damage in a temp. variable 
            int damage = 0;

            //check the attack type for each case 
            switch (attackType)
            {
                case SLASH:
                    //return the damage for slash 
                    damage = 10;
                    break;
                case STAB:
                    //return the 
                    damage = 15;
                    break;
                case HEAVYATTACK:
                    damage = 25;
                    break;
                case SLICE:
                    damage = 20;
                    break;
            }

            //returning the damage for that attack mode 
            return damage;
        }

        //pre :targetPosition and followPosition are valid Vector2 positions
        //post:Returns the updated followPosition after moving towards the targetPosition
        //desc:Calculates the direction and updates the follower's position to move towards the target position
        private Vector2 FollowTheTarget(Vector2 targetPosition, Vector2 followerPosition)
        {
            //calculating the distance between the target and the follower 
            Vector2 dist = targetPosition - followerPosition;

            //normalizing the distnce for a direction vector
            dist.Normalize();

            //adding the distance to the position as speed 
            followerPosition.X += dist.X;
            followerPosition.Y += dist.Y;

            //returning the follower position 
            return followerPosition;
        }

        //pre :None
        //post:None
        //desc:Displays the zombies' animations based on their state. Adjusts the animation position and orientation. Only displays zombies if the corresponding relic is active
        private void DisplayZombieAnimations()
        {
            //checking if the player has activated the relic 2 
            if (relic2)
            {
                //looping through each zombie 
                for (int i = 0; i < zombiesState.Length; i++)
                {
                    //checking if the zombie is alive and the player is not dead 
                    if (zombiesAlive[i] && playerState != DEATH)
                    {
                        //updating the zombie rectangle to rectangle position 
                        zombiesRec[i].X = (int)zombiesPos[i].X;
                        zombiesRec[i].Y = (int)zombiesPos[i].Y;

                        //checking if the zombie is not moving left 
                        if (zombiesState[i] != MOVELEFT)
                        {
                            //translating the animation to the zombie position 
                            zombieAnims[i, zombiesState[i]].TranslateTo((int)(zombiesPos[i].X - 8), (int)(zombiesPos[i].Y - 2));

                            //drawing the normal zombie animation 
                            zombieAnims[i, zombiesState[i]].Draw(spriteBatch, Color.White);
                        }
                        else
                        {
                            //translating the zombie animation to zombie position 
                            zombieAnims[i, zombiesState[i]].TranslateTo((int)(zombiesPos[i].X - 8), (int)(zombiesPos[i].Y - 2));

                            //drawing the zombie animation 
                            zombieAnims[i, zombiesState[i]].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                        }
                    }
                }
            }
        }

        //pre :None
        //post:None
        //desc:Displays the skeletons' animations based on their state. Adjusts the animation position and orientation. Only displays skeletons if the corresponding relic is active
        private void DisplaySkeletonAnimations()
        {
            //checking if the player has activated relic 3
            if (relic3)
            {
                //looping through the skeletons 
                for (int i = 0; i < skeletonsState.Length; i++)
                {
                    //checking if the skeleton is alive and player is not dead 
                    if (skeletonsAlive[i] && playerState != DEATH)
                    {
                        //updating the skeleton rectangle to skeleton's position 
                        skeletonsRec[i].X = (int)skeletonsPos[i].X;
                        skeletonsRec[i].Y = (int)skeletonsPos[i].Y;

                        //checking if the skeleton is not moving left 
                        if (skeletonsState[i] != MOVELEFT)
                        {
                            //translating the skeleton animation to the skeleton position 
                            skeletonAnims[i, skeletonsState[i]].TranslateTo((int)(skeletonsPos[i].X - 13), (int)(skeletonsPos[i].Y - 8));

                            //drawing the skeleton animation 
                            skeletonAnims[i, skeletonsState[i]].Draw(spriteBatch, Color.White);
                        }
                        else
                        {
                            //translating the skeleton animation to the skeleton position  
                            skeletonAnims[i, skeletonsState[i]].TranslateTo((int)(skeletonsPos[i].X - 13), (int)(skeletonsPos[i].Y - 8));

                            //drawing the horizontally flipped animation for moving left 
                            skeletonAnims[i, skeletonsState[i]].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                        }
                    }
                }
            }
        }

        //pre :None
        //post:None
        //desc:Displays the knights' animations based on their state. Adjusts the animation position and orientation. Only displays knights if the corresponding relic is active
        private void DisplayKnightAnimations()
        {
            //checking if the player has activated relic 2 
            if (relic1)
            {
                //looping through all the knights 
                for (int i = 0; i < knightsState.Length; i++)
                {
                    //checking if the knights are alive and player is not dead to give the death effect 
                    if (knightsAlive[i] && playerState != DEATH)
                    {
                        //Updating the knight's rectangle position to player Position 
                        knightsRec[i].X = (int)knightsPos[i].X;
                        knightsRec[i].Y = (int)knightsPos[i].Y;

                        //checking if the player is not moving left 
                        if (knightsState[i] != MOVELEFT)
                        {
                            //Translating the knight animation to knight's position 
                            knightAnims[i, knightsState[i]].TranslateTo((int)(knightsPos[i].X - 34), (int)(knightsPos[i].Y - 24));

                            //drawing the player animation 
                            knightAnims[i, knightsState[i]].Draw(spriteBatch, Color.White);
                        }
                        else
                        {
                            //Translating the knight Animation to knight's position  
                            knightAnims[i, knightsState[i]].TranslateTo((int)(knightsPos[i].X - 34), (int)(knightsPos[i].Y - 24));

                            //drawing the fipped knight
                            knightAnims[i, knightsState[i]].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                        }
                    }
                }
            }
        }

        //pre :None
        //post:None
        //desc:Displays the witch's animations based on her state. Adjusts the animation position and orientation. Only displays the witch if the corresponding relic is active
        private void DisplayWitch()
        {
            //checking if the player has activated the relic 
            if (relic4)
            {
                //checking if the witch is alive and player is not dead and ignoring the blank witch state 
                if (witchAlive && playerState != DEATH && witchState != 5)
                {
                    //updating the witch rectangle with witch position 
                    witchRec.X = (int)witchPos.X;
                    witchRec.Y = (int)witchPos.Y;

                    //checking if the witch is moving left 
                    if (witchState != MOVELEFT)
                    {
                        //translating the animation to that enemy's position 
                        witchAnims[witchState].TranslateTo((int)(witchPos.X), (int)(witchPos.Y));

                        //drawing the witch 
                        witchAnims[witchState].Draw(spriteBatch, Color.White);
                    }
                    else
                    {
                        //translating the animation to that enemy's position 
                        witchAnims[witchState].TranslateTo((int)(witchPos.X), (int)(witchPos.Y));

                        //drawing the anaimtion for the witch 
                        witchAnims[witchState].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                    }
                }

            }
        }

        //pre :None
        //post:None
        //desc:Updates the quest log based on the current game state and specific conditions. Sets the status of quests to true when certain conditions are met, such as the player's inventory, relics collected, or the health of the witch
        private void UpdateQuestLog()
        {
            switch (gameState)
            {
                case FIRST_LEVEL:
                    //only pass level 1 when you have filled the inventory Quantities
                    if (inventoryQuantities[3] != 0)
                    {
                        //setting quest flag to true
                        quest1Status = true;
                    }
                    break;
                case SECOND_LEVEL:
                    //looping through the parallel enemy arrays to go through the health of each enemy 
                    for (int i = 0; i < zombiesHealth.Length; i++)
                    {
                        //checking if the player has killed all enemies 
                        if (/*zombiesHealth[i] <= 0 && knightsHealth[i] <= 0 && skeletonsHealth[i] <= 0 &&*/ relic1 && relic2 && relic3)
                        {
                            //setting quest 2 flag to true
                            quest2Status = true;
                        }
                    }
                    break;
                case FINAL_LEVEL:
                    //checking if the witch is alive and relic 4 is pressed 
                    if (witchHealth <= 0 && relic4)
                    {
                        //setting level 2 quest flag to true
                        quest3Status = true;
                    }
                    break;
            }
        }

    }
}