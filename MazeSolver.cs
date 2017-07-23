using System;
using System.Collections.Generic;

namespace MazeGenSolve
{
    public class SearchActivity
    {
        CellPosition cellVisited;
        bool partOfCorrectWay;

        public CellPosition CellVisited { get { return cellVisited; } }
        public bool PartOfCorrectWay { get { return partOfCorrectWay; } }

        public SearchActivity(CellPosition cellVisited, bool partOfCorrectWay)
        {
            this.cellVisited = cellVisited;
            this.partOfCorrectWay = partOfCorrectWay;
        }
    }
    public class MazeSolver
    {
        const int DFS_VISITED = 1;
        const int BFS_VISITED = 2;

        Random rd = new Random();
        CellPosition from = new CellPosition(0, 0);
        CellPosition goal;
        private bool depthFirstSearch(Maze maze, CellPosition position, List<SearchActivity> paths)
        {
            CellState state = maze.StateFromIdent(position);
            if (position != from) // Adding from again would mess with the red and green dots.
            {
                if (position == goal)
                {
                    paths.Add(new SearchActivity(position, true));
                    return true;
                }

                else paths.Add(new SearchActivity(position, false));
            }

            foreach (CellPosition adjPos in state.AdjCells)
            {
                CellState adjState = maze.StateFromIdent(adjPos);
                if (adjState.Visited == DFS_VISITED) continue;
                adjState.Visited = DFS_VISITED;
                if (depthFirstSearch(maze, adjPos, paths))
                {
                    paths.Add(new SearchActivity(adjPos, true));
                    return true;
                }

            }

            return false;

        }
        public IEnumerable<SearchActivity> DepthFirstSearch(Maze maze)
        {
            from = randomPoint(maze);
            goal = randomPoint(maze);
            List <SearchActivity> paths = new List<SearchActivity>();
            paths.Add(new SearchActivity(from,true));
            paths.Add(new SearchActivity(goal,true));
            depthFirstSearch(maze, from, paths);
            return paths;
        }

        private CellPosition randomPoint(Maze maze)
        {
            int randX = rd.Next(0, maze.Width - 1);
            int randY = rd.Next(0, maze.Height - 1);
            return new CellPosition(randX, randY);
        }

        public IEnumerable<SearchActivity> BreadthFirstSearch(Maze maze)
        {
            from = randomPoint(maze);
            goal = randomPoint(maze);
            yield return new SearchActivity(from, true);
            yield return new SearchActivity(goal,true);
            Queue<CellPosition> queue = new Queue<CellPosition>();
            queue.Enqueue(from);
            bool goalFound = false;
            while (queue.Count > 0)
            {
                CellPosition cell = queue.Dequeue();
                if (cell == goal)
                {
                    goalFound = true; break;
                }
                CellState state = maze.StateFromIdent(cell);
                if(cell != from) yield return new SearchActivity(cell, false);
                foreach (CellPosition adjCell in state.AdjCells)
                {
                    CellState adjState = maze.StateFromIdent(adjCell);
                    if (adjState.Visited == BFS_VISITED) continue;
                    adjState.Visited = BFS_VISITED;
                    adjState.Parent = cell;
                    queue.Enqueue(adjCell);
                }
            }

            if (!goalFound) yield break; // There is no way! 

            CellState parentState = maze.StateFromIdent(goal);
            CellPosition parent = goal;
            while (parent != from)
            {
                yield return new SearchActivity(parent, true);
                parent = parentState.Parent;
                parentState = maze.StateFromIdent(parent);
            }
        }

    }
}