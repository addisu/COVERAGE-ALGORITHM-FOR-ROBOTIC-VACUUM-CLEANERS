using System;
using System.Collections.Generic;

namespace CoverageAlgorithm
{
    /// <summary>
    /// Contains needed information for saving a graphic
    /// </summary>
    public struct CoverageSpaceData
    {
        public Tuple<int, int> Size { get; private set; }

        public CoverageSpaceData(Tuple<int, int> size)
        {
            Size = size;
        }
    }
}
