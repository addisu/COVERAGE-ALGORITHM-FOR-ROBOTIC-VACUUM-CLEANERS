using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CoverageAlgorithm
{
    public class GridBlock
    {
        Rectangle square;
        private bool cleaned;
        private bool isObstacle;
        private bool visited;
        private bool isStart;
        private bool isFinish;

        public int RowIndex { get; private set; }


        public bool Visited
        {
            get { return visited; }
            set
            {
                visited = value;
                if (visited)
                    square.Fill = new SolidColorBrush(Colors.Yellow);
                else
                    square.Fill = new SolidColorBrush(Colors.White);
            }
        }

        public bool IsStart
        {
            get { return isStart; }
            set
            {
                isStart = value;
                if (isStart)
                    square.Fill = new SolidColorBrush(Colors.Green);
                else
                    square.Fill = new SolidColorBrush(Colors.White);
            }
        }


        public bool IsFinish
        {
            get { return isFinish; }
            set           
            {
                isFinish = value;
                if (isFinish)
                    square.Fill = new SolidColorBrush(Colors.Red);
                else
                    square.Fill = new SolidColorBrush(Colors.White);
            }
        }



        public bool IsObstacle
        {
            get { return isObstacle; }
            set 
            { 
                isObstacle = value;
                if (isObstacle)
                    square.Fill = new SolidColorBrush(Colors.Black);
                else
                    square.Fill = new SolidColorBrush(Colors.White);
            }
        }


        public bool Cleaned
        {
            get { return cleaned; }
            set
            {
                cleaned = value;
                if (cleaned)
                    square.Fill = new SolidColorBrush(Colors.LightBlue);
                else
                    square.Fill = new SolidColorBrush(Colors.White);
            }
        }

        public double G { get; set; }
        public double F { get; set; }

        public GridBlock Parent { get; set; }

        public GridBlock(CoverageSpaceBase matrixBase, Rectangle gridBlock, int rowIndex)
        {
            this.G = 1;
            this.F = 1;
            square = gridBlock;
            RowIndex = rowIndex;
            Cleaned = false;

            square.MouseLeftButtonDown += (s, e) =>
            {
                if(!IsStart && !IsFinish)
                {
                    IsObstacle = !IsObstacle;
                }
            };

            square.MouseMove += Square_MouseMove;

            square.MouseEnter += Square_MouseEnter;

        }

        private void Square_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsObstacle = !IsObstacle;
            }
        }

        private void Square_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //IsObstacle = !IsObstacle;
            }
        }
    }
}
