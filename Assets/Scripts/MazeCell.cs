using UnityEngine;

namespace Maze
{
    public class MazeCell : MonoBehaviour
    {
        public float cellSize = 3f;
        public float borderSize = 0.5f;

        public GameObject wallLeft;
        public GameObject wallRight;
        public GameObject wallTop;
        public GameObject wallBottom;
    }
}
