using shared.math;

namespace shared.algorithm.spatial
{
    public class DDA
    {
        private double _cellSizeX;
        private double _cellSizeY;

        public DDA(double cellSizeX, double cellSizeY)
        {
            _cellSizeX = cellSizeX;
            _cellSizeY = cellSizeY;
        }

        public void Run(double x1, double y1, double x2, double y2, IDDACallback callback)
        {
            // Simplified implementation for now
            int cellX = (int)(x1 / _cellSizeX);
            int cellY = (int)(y1 / _cellSizeY);
            int endCellX = (int)(x2 / _cellSizeX);
            int endCellY = (int)(y2 / _cellSizeY);

            while (cellX != endCellX || cellY != endCellY)
            {
                if (!callback.OnTraverse(cellX, cellY))
                    return;

                // Move towards end cell (simple)
                if (cellX < endCellX) cellX++;
                else if (cellX > endCellX) cellX--;
                if (cellY < endCellY) cellY++;
                else if (cellY > endCellY) cellY--;
            }
            callback.OnTraverse(endCellX, endCellY);
        }
    }
}