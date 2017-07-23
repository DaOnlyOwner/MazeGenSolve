using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MazeGenSolve
{
    public partial class Form1 : Form
    {
        bool terminated = false;
        int mazeWidth;
        int mazeHeight;
        Bitmap canvas;
        Graphics drawingTool;
        Pen wallPen;
        Pen cellPen;


        Thread workerThread;
        public Form1()
        {
            InitializeComponent();
            BackColor = Color.Black;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            canvasShower.Width = Width > Height ? Height : Width;
            canvasShower.Height = canvasShower.Width;
            canvasShower.Location = new Point((int)(Width / 2.0f - canvasShower.Width / 2.0f), (int)(Height / 2.0f - canvasShower.Height / 2.0f));

            FormClosed += Form1_FormClosed;
            wallPen = new Pen(Brushes.Black, 1.5f);
            cellPen = new Pen(Brushes.LightGray, 1);
            cellPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            canvas = new Bitmap(canvasShower.Width, canvasShower.Height);
            canvasShower.BackColor = Color.White;
            canvasShower.Image = canvas;

            mazeWidth =  600;
            mazeHeight = 600;

            drawingTool = Graphics.FromImage(canvas);
            workerThread = new Thread(() => genAndSolveMazes());
            workerThread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            terminated = true;
            workerThread.Interrupt();
        }

        void genAndSolveMazes()
        {
            MazeGenerator mazeGen = new MazeGenerator();
            MazeSolver mazeSolver = new MazeSolver();
            Maze maze = mazeGen.Primes(mazeWidth, mazeHeight);
            IEnumerable<SearchActivity> path = mazeSolver.BreadthFirstSearch(maze);
            while (!terminated)
            {
                Task mazeDraw = Task.Run(() => drawMaze(maze, path));
                maze = mazeGen.Primes(mazeWidth, mazeHeight);
                path = mazeSolver.BreadthFirstSearch(maze);
                try
                {
                    mazeDraw.Wait();
                }
                catch (ThreadInterruptedException) { };
            }
        }

        void drawMaze(Maze maze, IEnumerable<SearchActivity> path)
        {
            float cellWidth = (float)canvasShower.Width / (maze.Width + 0.1f);
            float cellHeight = (float)canvasShower.Height / (maze.Height + 0.1f);

            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    CellState state = maze.StateFromIdent(x, y);
                    //drawingTool.DrawRectangle(cellPen, x * cellWidth, y * cellHeight,cellWidth, cellHeight);
                    if (state.ExistsNorthWall) drawingTool.DrawLine(wallPen, x * cellWidth, y * cellHeight, (x + 1) * cellWidth, y * cellHeight);
                    if (state.ExistsEastWall) drawingTool.DrawLine(wallPen, (x + 1) * cellWidth, y * cellHeight, (x + 1) * cellWidth, (y + 1) * cellHeight);
                    if (state.ExistsSouthWall) drawingTool.DrawLine(wallPen, x * cellWidth, (y + 1) * cellHeight, (x + 1) * cellWidth, (y + 1) * cellHeight);
                    if (state.ExistsWestWall) drawingTool.DrawLine(wallPen, x * cellWidth, y * cellHeight, x * cellWidth, (y + 1) * cellHeight);
                }
            }
            canvasShower.Invalidate();

            foreach (SearchActivity sacv in path)
            {
                Brush brush;
                if (sacv.PartOfCorrectWay) brush = Brushes.Green;
                else brush = Brushes.Red;
                drawingTool.FillEllipse(brush, sacv.CellVisited.X * cellWidth + cellWidth * (0.3f / 2f), sacv.CellVisited.Y * cellHeight + cellHeight * (0.3f / 2f), cellWidth * 0.7f, cellHeight * 0.7f);
                //Thread.Sleep(1);
                canvasShower.Invalidate();
            }
            Thread.Sleep(1000*3);
            canvas.Save("test.png");
            drawingTool.Clear(Color.White);


        }

    }
}
