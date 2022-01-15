namespace Maze
{
    public class MazeGeneratorCell
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool IsVisited;

        public bool IsWallLeftActive = true;
        public bool IsWallRightActive = true;
        public bool IsWallTopActive = true;
        public bool IsWallBottomActive = true;

        public MazeGeneratorCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}