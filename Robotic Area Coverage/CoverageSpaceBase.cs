using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CoverageAlgorithm
{
    public abstract partial class CoverageSpaceBase
    {
        const int elementSize = 45;

        Button StartCleaningButton;
        Grid grid;

        protected CoverageSpaceRow[] rows;
        Tuple<int, int> size;
        HashSet<Tuple<int, int>> backtrackingList;
        private Tuple<int, int> criticalPoint;

        public CoverageSpaceBase(Tuple<int, int> matrixSize, Grid matrixGrid, Button startCleaningButton)
        {
            StartCleaningButton = startCleaningButton;
            grid = matrixGrid;
            backtrackingList = new HashSet<Tuple<int, int>>();
            StartCleaningButton.Click += async (s, e) =>
            {
                await NavigateCleaningPath();
            };

            CreateMatrix(matrixSize);
        }

        /// <summary>
        /// Main Method - using BA* Search Algorithm 
        /// </summary>
        /// <returns></returns>
        private async Task NavigateCleaningPath()
        {
            Tuple<int, int> initialStart = new Tuple<int, int>(0, 0);
            Tuple<int, int> nextStartPoint = initialStart;
            Tuple<int, int> criticalPoint = null;

            do
            {
                criticalPoint = await BoustrophedonMotionAlgorithm(nextStartPoint);

                nextStartPoint = NextStartingPoint(criticalPoint);

                if (nextStartPoint == criticalPoint)
                {
                    backtrackingList.Remove(nextStartPoint);
                    continue;
                }

                if (criticalPoint != null && nextStartPoint != null) 
                { 
                    await AStarSearchAsync(criticalPoint, nextStartPoint); 
                }

            } while (backtrackingList.Count > 0);

            if (criticalPoint != null && initialStart != null)
            {
                await AStarSearchAsync(criticalPoint, initialStart);
            }
        }


        /// <summary>
        /// A* Search Algorithm - find the shortest path to the critical point found by using backtracking 
        /// </summary>
        /// <param name="criticalPoint"></param>
        /// <param name="newStart"></param>
        /// <returns></returns>
        private async Task AStarSearchAsync(Tuple<int, int> criticalPoint, Tuple<int, int> newStart)
        {
            ResetAStarSearch();
            var explored = new HashSet<Tuple<int, int>>();
            SimplePriorityQueue<Tuple<int, int>> pQueue = new SimplePriorityQueue<Tuple<int, int>>();

            pQueue.Enqueue(criticalPoint, (float) 0);

            while(pQueue.Count > 0)
            {
                Tuple<int, int> current = pQueue.Dequeue();
                var currentGridBlock = rows[current.Item2].GridBlocks[current.Item1];
                explored.Add(current);
                if(current == newStart) break;


                List<Tuple<int, int>> neighbours = GetNeighbours(current);

                foreach(var neighbour in neighbours)
                {
                    var neighbourGridBlock = rows[neighbour.Item2].GridBlocks[neighbour.Item1];

                    if (neighbourGridBlock.IsObstacle) continue;

                    double cost = 1;
                    double tempG = currentGridBlock.G + cost;
                    double tempF = tempG + Heuristic(newStart, neighbour);  // Euclidean distance for Heuristic

                    if (explored.Contains(neighbour) && tempF >= neighbourGridBlock.F) { continue;  }
                    else if (!pQueue.Contains(neighbour) || tempF < neighbourGridBlock.F)
                    {
                        neighbourGridBlock.Parent = currentGridBlock;
                        neighbourGridBlock.F = tempF;
                        neighbourGridBlock.G = tempG;

                        if (pQueue.Contains(neighbour)) { pQueue.Remove(neighbour); }

                        pQueue.Enqueue(neighbour, (float) neighbourGridBlock.F);
                    }


                }
            }

            var destination = rows[newStart.Item2].GridBlocks[newStart.Item1];

            List<GridBlock> path = new List<GridBlock>();
            for (var node = destination; node != null; node = node.Parent) { path.Add(node); }
            path.Reverse();
            foreach(var node in path)
            {
                node.Visited = true;
                await Task.Delay(200);
            }

        }

        /// <summary>
        /// Find all Neighbour cells that are not obstact 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private List<Tuple<int, int>> GetNeighbours(Tuple<int, int> current)
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();
            var left = GetLeft(current);
            var right = GetRight(current);
            var top = GetTop(current);
            var bottom = GetBottom(current);

            if (left != null && !IsObstactle(left)) result.Add(left);
            if (right != null && !IsObstactle(right)) result.Add(right);
            if (top != null && !IsObstactle(top)) result.Add(top);   
            if (bottom != null && !IsObstactle(bottom)) result.Add(bottom);

            return result;
        }

        /// <summary>
        /// CHeck if cell is Obstacle 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsObstactle(Tuple<int, int> node)
        {
            return rows[node.Item2].GridBlocks[node.Item1].IsObstacle;
        }

        /// <summary>
        /// Find the next Starting point 
        /// go through the backtracking list and find the closest one using Euclidean distance 
        /// </summary>
        /// <param name="criticalPoint"></param>
        /// <returns></returns>
        private Tuple<int, int> NextStartingPoint(Tuple<int, int> criticalPoint)
        {
            double minDistance = double.MaxValue;
            Tuple<int, int> nextStartingPoint = null;

            foreach (var pair in backtrackingList)
            {   
                var temp = Heuristic(pair, criticalPoint); //Using Euclidean distance 
                if (temp < minDistance)
                {
                    nextStartingPoint = pair;
                    minDistance = temp;
                }
            }

            return nextStartingPoint;
        }

        /// <summary>
        /// Heuristic function to find Euclidean distance 
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private double Heuristic(Tuple<int, int> node1, Tuple<int, int> node2)
        {
            double xSqr = (node1.Item2 - node2.Item2) * (node1.Item2 - node2.Item2);
            double ySqr = (node1.Item1 - node2.Item1) * (node1.Item1 - node2.Item1);

            return Math.Sqrt(xSqr + ySqr);

        }

        /// <summary>
        /// Boustrophedon Motion algorithm 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private async Task<Tuple<int, int>> BoustrophedonMotionAlgorithm(Tuple<int, int>  start)
        {
            Tuple<int, int> current = start;
            while (!CriticalPoint(current))
            {
                if(!ObstacleNorth(current))
                {
                    current = MoveNorth(current);
                }
                else
                {
                    if (!ObstacleSouth(current))
                    {
                        current = MoveSouth(current);
                    }
                    else
                    {
                        if (!ObstacleEast(current))
                        {
                            current = MoveEast(current);
                        }
                        else
                        {
                            current = MoveWest(current);
                        }
                    }
                }
                rows[current.Item2].GridBlocks[current.Item1].Cleaned = true;
                UpdateBacktrackingPoints(current);
                await Task.Delay(100);
            }
            criticalPoint = current;
            return criticalPoint;
        }

        /// <summary>
        /// Update Backtracking Points - Cell that has uncleaned Neighbour 
        /// </summary>
        /// <param name="current"></param>
        private void UpdateBacktrackingPoints(Tuple<int, int> current)
        {
            if(!CriticalPoint(current))
            {
                if (!backtrackingList.Contains(current)) 
                    backtrackingList.Add(current);
            }

            Tuple<int, int> left = GetLeft(current);
            Tuple<int, int> right = GetRight(current);
            Tuple<int, int> top = GetTop(current);
            Tuple<int, int> bottom = GetBottom(current);

            if (top != null && backtrackingList.Contains(top) && CriticalPoint(top)) backtrackingList.Remove(top);
            if (bottom != null && backtrackingList.Contains(bottom) && CriticalPoint(bottom)) backtrackingList.Remove(bottom);
            if (left != null && backtrackingList.Contains(left) && CriticalPoint(left)) backtrackingList.Remove(left);
            if (right != null && backtrackingList.Contains(right) && CriticalPoint(right)) backtrackingList.Remove(right);
        }

        private Tuple<int, int> GetLeft(Tuple<int, int> current)
        {
            if (current.Item1 <= 0) return null;

            return new Tuple<int, int>(current.Item1 - 1, current.Item2);

        }

        private Tuple<int, int> GetRight(Tuple<int, int> current)
        {
            if (current.Item1 >= rows[0].GridBlocks.Length - 1) return null;

            return new Tuple<int, int>(current.Item1 + 1, current.Item2);
        }

        private Tuple<int, int> GetBottom(Tuple<int, int> current)
        {
            if (current.Item2 >= rows.Length - 1) return null;

            return new Tuple<int, int>(current.Item1, current.Item2 + 1);
        }

        private Tuple<int, int> GetTop(Tuple<int, int> current)
        {
            if (current.Item2 <= 0) return null;

            return new Tuple<int, int>(current.Item1, current.Item2 - 1);
        }

        private Tuple<int, int> MoveWest(Tuple<int, int> current)
        {
            return new Tuple<int, int>(current.Item1 - 1, current.Item2);
        }

        private Tuple<int, int> MoveEast(Tuple<int, int> current)
        {
            return new Tuple<int, int>(current.Item1 + 1, current.Item2);
        }

        private Tuple<int, int> MoveSouth(Tuple<int, int> current)
        {

            return new Tuple<int, int>(current.Item1, current.Item2 + 1);
        }

        private Tuple<int, int> MoveNorth(Tuple<int, int> current)
        {
            
            return new Tuple<int, int>(current.Item1, current.Item2 - 1);
        }

        /// <summary>
        /// Critical Point is a cell that is  
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool CriticalPoint(Tuple<int, int> current)
        {
            if(ObstacleNorth(current) && 
                ObstacleSouth(current) &&
                ObstacleEast(current) &&
                ObstacleWest(current))
            {
                return true;
            }    
            return false;
        }

        private bool ObstacleWest(Tuple<int, int> current)
        {
            if (current.Item1 <= 0) return true;
            if (rows[current.Item2].GridBlocks[current.Item1 - 1].IsObstacle) return true;
            if (rows[current.Item2].GridBlocks[current.Item1 - 1].IsStart) return true;
            if (rows[current.Item2].GridBlocks[current.Item1 - 1].Cleaned) return true;

            return false;
        }

        private bool ObstacleEast(Tuple<int, int> current)
        {
            if (current.Item1 >= rows[0].GridBlocks.Length - 1) return true;
            if (rows[current.Item2].GridBlocks[current.Item1 + 1].IsObstacle) return true;
            if (rows[current.Item2].GridBlocks[current.Item1 + 1].IsStart) return true;
            if (rows[current.Item2].GridBlocks[current.Item1 + 1].Cleaned) return true;

            return false;
        }

        private bool ObstacleSouth(Tuple<int, int> current)
        {
            if (current.Item2 >= rows.Length - 1) return true;
            if (rows[current.Item2 + 1].GridBlocks[current.Item1].IsObstacle) return true;
            if (rows[current.Item2 + 1].GridBlocks[current.Item1].IsStart) return true;
            if (rows[current.Item2 + 1].GridBlocks[current.Item1].Cleaned) return true;

            return false;
        }

        private bool ObstacleNorth(Tuple<int, int> current)
        {
            if (current.Item2 <= 0) return true;
            if (rows[current.Item2 - 1].GridBlocks[current.Item1].IsObstacle) return true;
            if (rows[current.Item2 - 1].GridBlocks[current.Item1].IsStart) return true;
            if (rows[current.Item2 - 1].GridBlocks[current.Item1].Cleaned) return true;

            return false;

        }

        public void ResetAStarSearch()
        {
            foreach (CoverageSpaceRow row in rows)
            {
                foreach (GridBlock gridBlock in row.GridBlocks)
                {
                    gridBlock.F = 1;
                    gridBlock.G = 1;
                    gridBlock.Parent = null;
                }
            }

            //UpdateCode(currentType, true);
        }

        /// <summary>
        /// Sets the matrix according to MatrixData (from collection)
        /// </summary>
        public void SetMatrix(CoverageSpaceData data)
        {
            //nameText.Text = data.Name;
            CreateMatrix(data.Size);

        }


        /// <summary>
        /// Creates a matrix including the visual representation
        /// </summary>
        public void CreateMatrix(Tuple<int, int> matrixSize)
        {
            size = matrixSize;
            rows = new CoverageSpaceRow[size.Item2];

            // Clear the grid to make sure nothing is left over
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            // Resize
            grid.Width = elementSize * size.Item1;
            grid.Height = elementSize * size.Item2;

            // Create collumns and rows
            for (int i = 0; i < size.Item1; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < size.Item2; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(elementSize);
                grid.RowDefinitions.Add(row);

                rows[i] = new CoverageSpaceRow(size.Item1);
            }

            // Iterate through the matrix
            for (int row = 0; row < rows.Length; row++)
            {
                for (int column = 0; column < rows[row].GridBlocks.Length; column++)
                {
                    // Create visual representation of the LED
                    Rectangle rectangle = new Rectangle();
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);

                    // Create the "back-end" of the LED
                    GridBlock gridBlock = new GridBlock(this, rectangle, row);
                    rows[row].GridBlocks[column] = gridBlock;

                    // Assign the ellipse to the grid
                    Grid.SetColumn(rectangle, column);
                    Grid.SetRow(rectangle, row);
                    grid.Children.Add(rectangle);
                }
            }
            var start = rows[0].GridBlocks[0];
            var end = rows[rows.Length - 1].GridBlocks[rows[0].GridBlocks.Length - 1];
            start.IsStart = true;
            end.IsFinish = true;
        }

    }
}
