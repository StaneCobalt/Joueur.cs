// This is where you build your AI for the Newtonian game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add additional using(s) here
using System.Runtime.CompilerServices;
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Newtonian
{
    /// <summary>
    /// This is where you build your AI for Newtonian.
    /// </summary>
    public class AI : BaseAI
    {
        #region Properties
        #pragma warning disable 0169 // the never assigned warnings between here are incorrect. We set it for you via reflection. So these will remove it from the Error List.
        #pragma warning disable 0649
        /// <summary>
        /// This is the Game object itself. It contains all the information about the current game.
        /// </summary>
        public readonly Game Game;
        /// <summary>
        /// This is your AI's player. It contains all the information about your player's state.
        /// </summary>
        public readonly Player Player;
#pragma warning restore 0169
#pragma warning restore 0649

        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional properties here for your AI to use'
        bool needsRedium { get; set; }
        bool redTeam { get; set; }
        bool teamSet { get; set; }
        // <<-- /Creer-Merge: properties -->>
        #endregion


        #region Methods
        /// <summary>
        /// This returns your AI's name to the game server. Just replace the string.
        /// </summary>
        /// <returns>Your AI's name</returns>
        public override string GetName()
        {
            // <<-- Creer-Merge: get-name -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            return "Tis but a Segfault"; // REPLACE THIS WITH YOUR TEAM NAME!
            // <<-- /Creer-Merge: get-name -->>
        }

        /// <summary>
        /// This is automatically called when the game first starts, once the Game and all GameObjects have been initialized, but before any players do anything.
        /// </summary>
        /// <remarks>
        /// This is a good place to initialize any variables you add to your AI or start tracking game objects.
        /// </remarks>
        public override void Start()
        {
            // <<-- Creer-Merge: start -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Start();

            needsRedium = false;
            redTeam = false;
            teamSet = false;

            Console.Clear();
            // <<-- /Creer-Merge: start -->>
        }

        /// <summary>
        /// This is automatically called every time the game (or anything in it) updates.
        /// </summary>
        /// <remarks>
        /// If a function you call triggers an update, this will be called before that function returns.
        /// </remarks>
        public override void GameUpdated()
        {
            // <<-- Creer-Merge: game-updated -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.GameUpdated();
            /*this.DisplayMap(); // be careful using this as it will probably cause your client to time out in this function.
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;*/
            // <<-- /Creer-Merge: game-updated -->>
        }

        /// <summary>
        /// This is automatically called when the game ends.
        /// </summary>
        /// <remarks>
        /// You can do any cleanup of you AI here, or do custom logging. After this function returns, the application will close.
        /// </remarks>
        /// <param name="won">True if your player won, false otherwise</param>
        /// <param name="reason">A string explaining why you won or lost</param>
        public override void Ended(bool won, string reason)
        {
            // <<-- Creer-Merge: ended -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Ended(won, reason);
            // <<-- /Creer-Merge: ended -->>
        }


        /// <summary>
        /// This is called every time it is this AI.player's turn.
        /// </summary>
        /// <returns>Represents if you want to end your turn. True means end your turn, False means to keep your turn going and re-call this function.</returns>
        public bool RunTurn()
        {
            // <<-- Creer-Merge: runTurn -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            // Put your game logic here for runTurn
            /*DisplayMap();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;*/
            if(!teamSet)
            {
                redTeam = (this.Player.Units[0].Tile.TileWest.IsWall) ? true : false;
                teamSet = true;
            }

            /*
            Please note: This code is intentionally bad. You should try to optimize everything here. THe code here is only to show you how to use the game's
                        mechanics with the MegaMinerAI server framework.
            */
            int internNum = 0;
            // Goes through all the units that you own.
            foreach (Unit unit in this.Player.Units) {
                Tile target = null;

                needsRedium = PriorityOre();

                // Only tries to do something if the unit actually exists.
                if (unit != null && unit.Tile != null)
                {
                    if (unit.Job.Title == "physicist")
                    {
                        unit.Log("Nerd");
                        Whack(unit, "manager");
                        WorkItHarderMakeItBetter(unit, target);
                    }
                    else if (unit.Job.Title == "intern")
                    {
                        internNum += 1;

                        unit.Log("Expendable");
                        Whack(unit, "physicist");
                        if (unit.RediumOre == 0 && unit.BlueiumOre == 0)
                            FetchOre(unit, target, internNum%2);
                        else
                            DropOre(unit, target, internNum%2);
                    }
                    ///////////////////////////////////////////CODE FOR MANAGER///////////////////
                    else if (unit.Job.Title == "manager")
                    {
                        unit.Log("Bouncer");


                        // Finds enemy interns, stuns, and attacks them if there is no blueium to take to the generator.
                        Tile refined = null;
                        Tile generator = null;
                        bool physicistFound = false;
                        bool refinedExists = false;

                        if (this.Game.CurrentTurn < 2)
                        {
                            foreach (Tile tile in this.Game.Tiles)
                            {
                                if (tile.Owner == this.Player.Opponent)
                                {
                                    target = tile;
                                    break;
                                }
                            }
                            if (this.FindPath(unit.Tile, target).Count > 0)
                            {
                                while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0)
                                {
                                    if (!unit.Move(this.FindPath(unit.Tile, target)[0]))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        
                        //Prioritize collecting refined material:
                        foreach (Tile tile in this.Game.Tiles)
                        {
                            if (tile.Redium > 0 || tile.Blueium > 0)
                            {
                                refinedExists = true;
                                refined = tile;
                            }
                        }

                        if (refinedExists && (unit.Blueium + unit.Redium) < 3)
                        {
                            FetchRefined(unit, refined, 3);
                        }
                        else if (unit.Blueium + unit.Redium == 3)
                        {
                            DropRefined(unit, generator, 3);
                        }

                        Guard(unit, target, physicistFound);

                    }//end if manager
                }
            }

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        /// <summary>
        /// A very basic path finding algorithm (Breadth First Search) that when given a starting Tile, will return a valid path to the goal Tile.
        /// </summary>
        /// <remarks>
        /// This is NOT an optimal pathfinding algorithm. It is intended as a stepping stone if you want to improve it.
        /// </remarks>
        /// <param name="start">the starting Tile</param>
        /// <param name="goal">the goal Tile</param>
        /// <returns>A list of Tiles representing the path where the first element is a valid adjacent Tile to the start, and the last element is the goal. Or an empty list if no path found.</returns>
        List<Tile> FindPath(Tile start, Tile goal)
        {
            // no need to make a path to here...
            if (start == goal)
            {
                return new List<Tile>();
            }

            // the tiles that will have their neighbors searched for 'goal'
            Queue<Tile> fringe = new Queue<Tile>();

            // How we got to each tile that went into the fringe.
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

            // Enqueue start as the first tile to have its neighbors searched.
            fringe.Enqueue(start);

            // keep exploring neighbors of neighbors... until there are no more.
            while (fringe.Any())
            {
                // the tile we are currently exploring.
                Tile inspect = fringe.Dequeue();

                // cycle through the tile's neighbors.
                foreach (Tile neighbor in inspect.GetNeighbors())
                {
                    if (neighbor == goal)
                    {
                        // Follow the path backward starting at the goal and return it.
                        List<Tile> path = new List<Tile>();
                        path.Add(goal);

                        // Starting at the tile we are currently at, insert them retracing our steps till we get to the starting tile
                        for (Tile step = inspect; step != start; step = cameFrom[step])
                        {
                            path.Insert(0, step);
                        }

                        return path;
                    }

                    // if the tile exists, has not been explored or added to the fringe yet, and it is pathable
                    if (neighbor != null && !cameFrom.ContainsKey(neighbor) && neighbor.IsPathable())
                    {
                        // add it to the tiles to be explored and add where it came from.
                        fringe.Enqueue(neighbor);
                        cameFrom.Add(neighbor, inspect);
                    }

                } // foreach(neighbor)

            } // while(fringe not empty)

            // if you're here, that means that there was not a path to get to where you want to go.
            //   in that case, we'll just return an empty path.
            return new List<Tile>();
        }

        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional methods here for your AI to call

        //if adjacent enemy is stunnable, stuns, otherwise attacks
        private void Whack(Unit unit, String enemyToStun)
        {
            foreach (Tile tile in unit.Tile.GetNeighbors())
            {
                if (tile.Unit != null)
                {
                    if (tile.Unit.Owner == this.Player.Opponent)
                    {
                        if (tile.Unit.Job.Title == enemyToStun)
                        {
                            if (tile.Unit.StunImmune == 0)
                            {
                                unit.Act(tile);
                                break;
                            }
                        }
                        unit.Attack(tile);
                        break;
                    }
                }
            }
        }

        // picks up adjacent ore or finds ore to move to
        private void FetchOre(Unit unit, Tile target, int pattern)
        {
            bool justDoIt = false;
            if((!redTeam && pattern == 1)||(redTeam && pattern == 0))
            {
                justDoIt = true;
            }

            // looks for adjacent ore, if found, picks it up
            foreach(Tile tile in unit.Tile.GetNeighbors())
            {
                if (tile.Machine == null)
                {
                    
                    if (tile.RediumOre > 0 && justDoIt)
                        unit.Pickup(tile, unit.Job.CarryLimit, "redium ore");
                    else if (tile.BlueiumOre > 0 && !justDoIt)
                        unit.Pickup(tile, unit.Job.CarryLimit, "blueium ore");
                }
            }

            // looks for tile that has ore
            foreach (Tile tile in this.Game.Tiles)
            {
                if (tile.RediumOre > 0 && justDoIt)
                {
                    target = tile;
                    break;
                }
                else if (tile.BlueiumOre > 0 && !justDoIt)
                {
                    target = tile;
                    break;
                }
            }
            
            // moves to ore
            if (this.FindPath(unit.Tile, target).Count > 0)
            {
                while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0)
                {
                    if (!unit.Move(this.FindPath(unit.Tile, target)[0]))
                    {
                        break;
                    }
                }
            }
        }
        
        private void FetchRefined(Unit unit, Tile target, int ammount)
        {
            foreach(Tile tile in unit.Tile.GetNeighbors())
            {
                if (tile.Redium > 0)
                    unit.Pickup(tile, ammount, "redium");
                else if (tile.Blueium > 0)
                    unit.Pickup(tile, ammount, "blueium");
            }
            foreach(Tile tile in this.Game.Tiles)
            {
                /*right now this is equevalent to 
                if(tile.Redium > 0 || tile.Blueium > 0){ ... }
                But we need to add extra descision making later so I'm keeping it like this */
                if(tile.Redium > 0)
                {
                    target = tile;
                    break;
                }
                else if (tile.Blueium > 0)
                {
                    target = tile;
                    break;
                }
            }
            if(this.FindPath(unit.Tile, target).Count > 0)
            {
                while(unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0)
                {
                    if(!unit.Move(this.FindPath(unit.Tile, target)[0]))
                    {
                        break;
                    }
                }
            }
        }

        // drops ore on adjacent machine or finds machine to move to
        private void DropOre(Unit unit, Tile target, int pattern)
        {
            bool justDoIt = false;
            if ((!redTeam && pattern == 1) || (redTeam && pattern == 0))
            {
                justDoIt = true;
            }

            // looks for adjacent machine of certain oretype, and drops ore
            foreach (Tile tile in unit.Tile.GetNeighbors())
            {
                if (tile.Machine != null)
                {
                    if (tile.Machine.OreType == "redium" && justDoIt) {
                        unit.Drop(tile, unit.Job.CarryLimit, "redium ore");
                    } else if (tile.Machine.OreType == "blueium" && !justDoIt) {
                        unit.Drop(tile, unit.Job.CarryLimit, "blueium ore");
                    }
                }
            }

            // looks for machine of certain oretype
            foreach (Tile tile in this.Game.Tiles)
            {
                if (tile.Machine != null)
                {
                    if (tile.Machine.OreType == "redium" && justDoIt)
                    {
                        target = tile;
                    }
                    else if (tile.Machine.OreType == "blueium" && !justDoIt)
                    {
                        target = tile;
                    }
                }
            }
        }
            
        //run this if a manager has refined material
        //drops off refined at friendly generator, or runs to friendly generator.
        private void DropRefined(Unit unit, Tile target, int ammount)
        {
            bool friendlyFound = false;
            //drop refined in friendly generator:
            foreach(Tile tile in unit.Tile.GetNeighbors())
            {
                if(tile.Type == "generator" && tile.Owner == this.Player)
                {
                    friendlyFound = true;
                    if (unit.Blueium > 0)
                        unit.Drop(tile, unit.Blueium, "blueium");
                    else if (unit.Redium > 0)
                        unit.Drop(tile, unit.Redium, "redium");
                    else Console.WriteLine(unit.ToString() + ": Error, nothing to drop off");
                }
            }
            //find friendly generator:
            if (!friendlyFound)
            {
                foreach (Tile tile in this.Game.Tiles)
                {
                    if (tile.Type == "generator" && tile.Owner == this.Player)
                        target = tile;
                }
            }

            // moves to machine
            if (this.FindPath(unit.Tile, target).Count > 0)
            {
                while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0)
                {
                    if (!unit.Move(this.FindPath(unit.Tile, target)[0]))
                    {
                        break;
                    }
                }
            }
        }

        private bool PriorityOre()
        {
            // true prioritizes redium, false prioritizes blueium
            if (this.Game.CurrentTurn < 10)
            {
                if (redTeam)
                    return false;
                return true;
            }
            return (this.Player.Heat < this.Player.Pressure + 6);
        }

        private void WorkItHarderMakeItBetter(Unit unit, Tile target)
        {
            // looks for adjacent machine, if found, then works adjacent machine
            foreach(Tile tile in unit.Tile.GetNeighbors())
            {
                if(tile.Machine != null)
                {
                    if (tile.Machine.OreType == "redium" && needsRedium && tile.RediumOre >= tile.Machine.RefineInput)
                        unit.Act(tile);
                    else if (tile.Machine.OreType == "blueium" && !needsRedium && tile.BlueiumOre >= tile.Machine.RefineInput)
                        unit.Act(tile);
                }
            }

            // finds machine
            foreach (Machine machine in this.Game.Machines)
            {
                if (machine.Tile.RediumOre >= machine.RefineInput && needsRedium)
                {
                    target = machine.Tile;
                }
                else if (machine.Tile.BlueiumOre >= machine.RefineInput && !needsRedium)
                {
                    target = machine.Tile;
                }
            }

            if(target == null)
            {
                //target = this.Player.SpawnTiles[0];
                foreach(Tile tile in this.Game.Tiles)
                {
                    if (tile.Unit != null)
                    {
                        if(tile.Unit.Job.Title == "manager" && tile.Unit.Owner == this.Player.Opponent)
                        {
                            target = tile;
                            break;
                        }
                    }
                    
                }
            }

            // moves to machine
            if (this.FindPath(unit.Tile, target).Count > 0)
            {
                while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0)
                {
                    if (!unit.Move(this.FindPath(unit.Tile, target)[0]))
                    {
                        break;
                    }
                }
            }
        }

        //first checks for interns nearby the physicist to attack
        //second finds physicist to follow
        //third moves toward physicist
        private void Guard(Unit unit, Tile target, bool physicistFound)
        {
            //Finds closest physicist:
            int currentPathLength = 0;
            physicistFound = false;
            foreach (Unit gameUnit in this.Game.Units)
            {
                if (gameUnit.Job.Title == "physicist" && gameUnit.Owner == this.Player)
                {
                    if (physicistFound)
                    {
                        if (currentPathLength > FindPath(unit.Tile, gameUnit.Tile).Count)
                        {
                            target = gameUnit.Tile;
                            currentPathLength = FindPath(unit.Tile, gameUnit.Tile).Count;
                        }
                    }
                    else
                    {
                        physicistFound = true;
                        target = gameUnit.Tile;
                        currentPathLength = FindPath(unit.Tile, gameUnit.Tile).Count;
                    }
                }
            }
            //move to target
            if (this.FindPath(unit.Tile, target).Count > 0)
            {
                while (unit.Moves > 0 && this.FindPath(unit.Tile, target).Count > 0)
                {
                    if (!unit.Move(this.FindPath(unit.Tile, target)[0]))
                    {
                        break;
                    }
                }
            }
        }

        private void DisplayMap() {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(new string(' ', this.Game.MapWidth + 2));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
            for (int y = 0; y < this.Game.MapHeight; y++) {
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(' ');
                for (int x = 0; x < this.Game.MapWidth; x++) {
                    Tile t = this.Game.Tiles[y * this.Game.MapWidth + x];

                    // Background color
                    if (t.Machine != null) {
                        Console.BackgroundColor = ((t.Machine.OreType == "redium") ? ConsoleColor.DarkRed : ConsoleColor.DarkBlue);
                    } else if (t.IsWall == true) {
                        if (t.Decoration == 1 || t.Decoration == 2) {
                            Console.BackgroundColor = ConsoleColor.DarkGray;  // Black;
                        } else {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                        }
                    } else {
                        if (t.Decoration == 1 || t.Decoration == 2) {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                        } else {
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                    }

                    // Character to display
                    char foreground = t.Machine == null ? '·' : 'M';
                    Console.ForegroundColor = ConsoleColor.White;

                    // Tile specific stuff
                    if (t.Unit != null) {
                        Console.ForegroundColor = t.Unit.Owner == this.Player ? ConsoleColor.Green : ConsoleColor.Red;
                        foreground = t.Unit.Job.Title[0] == 'i' ? 'I' : t.Unit.Job.Title[0] == 'm' ? 'M' : 'P'; //t.Unit.ShipHealth > 0 ? 'S' : 'C';
                    }
                    if(t.Blueium > 0 || t.Redium > 0) {
                        Console.BackgroundColor = t.Blueium >= t.Redium ? ConsoleColor.DarkBlue : ConsoleColor.DarkRed;
                        if(foreground == '·') {
                            foreground = 'R';
                        }
                    }
                    else if(t.BlueiumOre > 0 || t.RediumOre > 0) {
                        Console.BackgroundColor = t.BlueiumOre >= t.RediumOre ? ConsoleColor.DarkBlue : ConsoleColor.DarkRed;
                        if(foreground == '·') {
                            foreground = 'O';
                        }
                    }
                    else if(t.Owner != null) {
                        if(t.Type == "spawn") {
                            Console.BackgroundColor = t.Owner == this.Player ? ConsoleColor.Cyan : ConsoleColor.Magenta;
                        } else if(t.Type == "generator") {
                            Console.BackgroundColor = t.Owner == this.Player ? ConsoleColor.DarkCyan : ConsoleColor.DarkMagenta;
                        }
                        /*if (false && this.Game.Units.Any(u => u.Path.Contains(t))) {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            foreground = '*';
                        } else if (t.Decoration) {
                            Console.ForegroundColor = ConsoleColor.White;
                            foreground = '.';*/
                    } else if(t.Type == "conveyor") {
                        if(t.Direction == "north") {
                            foreground = '^';
                        } else if(t.Direction == "east") {
                            foreground = '>';
                        } else if(t.Direction == "west") {
                            foreground = '<';
                        } else if(t.Direction == "blank") {
                            foreground = '_';
                        } else {
                            foreground = 'V';
                        }
                    }

                    Console.Write(foreground);
                }

                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(' ');
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(y);
                Console.WriteLine();
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(new string(' ', this.Game.MapWidth + 2));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            // Clear everything past here
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.Write(new string(' ', Math.Max(Console.WindowHeight, Console.WindowWidth * (Console.WindowHeight - top) - 1)));
            Console.SetCursorPosition(left, top);
        }
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
