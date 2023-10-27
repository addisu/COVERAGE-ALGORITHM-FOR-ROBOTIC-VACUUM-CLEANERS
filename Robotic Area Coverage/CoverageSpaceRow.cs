using System;

namespace CoverageAlgorithm
{
    public abstract partial class CoverageSpaceBase
    {
        protected class CoverageSpaceRow
        {
            public GridBlock[] GridBlocks { get; private set; }


            public CoverageSpaceRow(int size)
            {
                GridBlocks = new GridBlock[size];
            }

        }
    }
}
