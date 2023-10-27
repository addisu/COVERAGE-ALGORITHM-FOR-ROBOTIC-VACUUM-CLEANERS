using System;
using System.Windows;
using System.Windows.Controls;

namespace CoverageAlgorithm
{
    public class CoverageSpace : CoverageSpaceBase
    {
        public CoverageSpace(Tuple<int, int> matrixSize, Grid grid, Button codeButton) :
            base (matrixSize, grid, codeButton) { }


        /// <summary>
        /// Inverts all of the LEDs
        /// </summary>
        public void InvertAll()
        {
            foreach (CoverageSpaceRow row in rows)
            {
                foreach (GridBlock led in row.GridBlocks)
                {
                    led.IsObstacle = !led.IsObstacle;
                }
            }

        }

        /// <summary>
        /// Sets all of the LEDs to a state
        /// </summary>
        public void SetAll(bool state)
        {
            foreach (CoverageSpaceRow row in rows)
            {
                foreach (GridBlock gridBlock in row.GridBlocks)
                {
                    gridBlock.IsObstacle = false;
                    gridBlock.Cleaned = false;

                }
            }

        }


    }
}
