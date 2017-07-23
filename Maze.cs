using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenSolve
{
    
    public class CellState
    {
        public bool BelongsToMaze { get; set; } = false;
#region Searching
        public int Visited { get; set; } = NOT_VISITED;

        const int NOT_VISITED = -1;
        public CellPosition Parent { get; set; } = new CellPosition(-1, -1);

       public List<CellPosition> AdjCells { get; set; } // For fast graph traversals

        public int Mark { get; set; } = -1; // Mark in different colors.
#endregion Searching
        public bool ExistsNorthWall { get; set; } = true;
        public bool ExistsEastWall { get; set; } = true;
        public bool ExistsSouthWall { get; set; } = true;
        public bool ExistsWestWall { get; set; } = true;

        public CellState()
        {
            AdjCells = new List<CellPosition>(4);
        }


    }

    public struct CellPosition
    {
        public int Y { get; private set; }
        public int X { get; private set; }

        public CellPosition(int x, int y)
        {
            Y = y; X = x;
        }

        public static bool operator ==(CellPosition c1, CellPosition c2)
        {
            return c1.X == c2.X && c1.Y == c2.Y;
        }

        public static bool operator !=(CellPosition c1, CellPosition c2)
        {
            return !(c1 == c2);
        }
    }

    public class Maze
    {
        public const int DEFAULT_MARK = 0;
        public const int NO_MARK = -1;
        Random rd = new Random();
        CellState[,] cells;
        int width, height;
        public int Width { get { return width; }}
        public int Height { get { return height; } }

        public void DefaultMarkRange(IEnumerable<CellPosition> cells)
        {
            foreach(CellPosition cell in cells)
            {
                StateFromIdent(cell).Mark = DEFAULT_MARK;
                DefaultMarkedList.Add(cell);
            }
        }

        public List<CellPosition> DefaultMarkedList { get; private set; } = new List<CellPosition>();

        public void DefaultUnmark(CellPosition frontier)
        {
            DefaultMarkedList.Remove(frontier);
            StateFromIdent(frontier).Mark = NO_MARK;
        }

        public Maze(int width, int height)
        {
            cells = new CellState[height, width];
            
            for(int y =0; y<height; y++)
            {
                for(int x = 0; x<width; x++)
                {
                    cells[y,x] = new CellState();
                }
            }

            this.height = height;
            this.width = width;
        }

        public CellState StateFromIdent(CellPosition cell)
        {
            return cells[cell.Y, cell.X]; 
        }

        public IEnumerable<CellPosition> YieldNeighbours(CellPosition c)
        {
            if (c.Y > 0) yield return new CellPosition(c.X, c.Y - 1);
            if (c.Y < height - 1) yield return new CellPosition(c.X, c.Y + 1);
            if (c.X > 0) yield return new CellPosition(c.X - 1, c.Y);
            if (c.X < width - 1) yield return new CellPosition(c.X + 1, c.Y);
        }

        public CellState StateFromIdent(int x, int y)
        {
            return cells[y, x];
        }

        public CellPosition AddRandomCellToMaze()
        {
            int y = rd.Next(0, height - 1);
            int x = rd.Next(0, width - 1);
            CellPosition randomCell = new CellPosition(x, y);
            AddToMaze(randomCell);
            return randomCell;
        }

        // c1 and c2 have to be neighbours
        public void Connect(CellPosition c1, CellPosition c2)
        {
            CellState state1 = StateFromIdent(c1);
            CellState state2 = StateFromIdent(c2);

            if (c1.Y < c2.Y) { state1.ExistsSouthWall = false; state2.ExistsNorthWall = false; state1.AdjCells.Add(c2); state2.AdjCells.Add(c1); }
            else if (c1.Y > c2.Y) { state1.ExistsNorthWall = false; state2.ExistsSouthWall = false; state1.AdjCells.Add(c2); state2.AdjCells.Add(c1); }
            else if (c1.X < c2.X) { state1.ExistsEastWall = false; state2.ExistsWestWall = false; state1.AdjCells.Add(c2); state2.AdjCells.Add(c1); }
            else if (c1.X > c2.X) { state1.ExistsWestWall = false; state2.ExistsEastWall = false; state1.AdjCells.Add(c2); state2.AdjCells.Add(c1); }
            else throw new ArgumentException("Wrong parameter c1, c2");
        }

        public void AddToMaze(CellPosition cell)
        {
            StateFromIdent(cell).BelongsToMaze = true;
        }

        public void DefaultMarkAllNeighbours(CellPosition ident)
        {
            foreach (CellPosition c in YieldNeighbours(ident))
            {
                CellState state = StateFromIdent(c);
                if (state.Mark != DEFAULT_MARK)
                {
                    state.Mark = DEFAULT_MARK;
                    DefaultMarkedList.Add(c);
                }
            }
        }
    }
}
