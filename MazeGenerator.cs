using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenSolve
{
    public class MazeGenerator
    {
        private Random rd = new Random();
        private T randomItem<T>(List<T> input)
        {
            return input[rd.Next(0, input.Count - 1)];
        }
        public Maze Primes(int width, int height)
        {
            Maze maze = new Maze(width, height);

            CellPosition ident = maze.AddRandomCellToMaze();
            maze.DefaultMarkAllNeighbours(ident);

            while (maze.DefaultMarkedList.Count != 0)
            {
                // We retrieve a random "frontier" that will be added to the maze.
                CellPosition frontier = randomItem(maze.DefaultMarkedList);
                maze.AddToMaze(frontier);
                maze.DefaultUnmark(frontier);

                // Now we have to get all the neighbours of the frontier that are already part of the maze and carve a passage to one of them. 
                List<CellPosition> neighboursFrontier = maze.YieldNeighbours(frontier).Where(cell => maze.StateFromIdent(cell).BelongsToMaze).ToList();
                CellPosition theChosenOne = randomItem(neighboursFrontier);
                maze.Connect(frontier, theChosenOne);

                // Now add the frontier to the maze and mark suitable neighbours of the chosen frontier.
                maze.DefaultMarkRange(maze.YieldNeighbours(frontier).Where(cell => (maze.StateFromIdent(cell).Mark != Maze.DEFAULT_MARK && !maze.StateFromIdent(cell).BelongsToMaze)));
            }

            //maze.Connect(new CellPosition(0, 0), new CellPosition(0, 1));


            return maze;
        }
        public Maze Binary(int width, int height)
        {
            Maze maze = new Maze(width, height);
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int eastOrSouth = rd.Next(0, 2);
                    if (eastOrSouth == 0) // East
                    {
                        maze.Connect(new CellPosition(x, y), new CellPosition(x, y + 1));
                    }

                    else
                    {
                        maze.Connect(new CellPosition(x, y), new CellPosition(x + 1, y));
                    }
                }

                maze.Connect(new CellPosition(width-2, y), new CellPosition(width-1, y));

            }
            for(int x = 0; x<width-1; x++)
            {
                maze.Connect(new CellPosition(x, width - 1), new CellPosition(x + 1, width - 1));
            }

            return maze;
        }
    }
}
