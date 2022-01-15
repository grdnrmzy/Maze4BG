using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maze
{
    public class MazeGenerator : MonoBehaviour
    {
        public static MazeGenerator Instance;
        private Vector3[] Path { get; set; }

        public event Action OnMazeGenerated;
        public event Action<Vector3[]> OnPathChanged;

        [SerializeField] private int mazeSize = 12;
        [SerializeField] private int deathZonesCount = 6;

        [SerializeField] private GameObject mazeCell;
        [SerializeField] private GameObject Finish;
        [SerializeField] private GameObject deathZone;
        

        private float _cellSize = 3;
        private float _borderSize = 3;

        private MazeGeneratorCell[,] _maze;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var cell = mazeCell.GetComponent<MazeCell>();
            if (!cell) return;

            _cellSize = cell.cellSize;
            _borderSize = cell.borderSize;

            GenerateMaze();
        }

        public void GenerateMaze()
        {
            ClearMaze();

            InitializeMaze();
            BuildMaze();
            InstantiateMaze();
            GenerateDeathZones();

            InstantiateObject(ref Finish, mazeSize - 1, mazeSize - 1);

            OnMazeGenerated?.Invoke();
        }

        private void ClearMaze()
        {
            if (transform.childCount == 0) return;

            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }

        private void InitializeMaze()
        {
            _maze = new MazeGeneratorCell[mazeSize, mazeSize];

            for (int x = 0; x < mazeSize; x++)
                for (int y = 0; y < mazeSize; y++)
                    _maze[x, y] = new MazeGeneratorCell(x, y);
        }

        private void BuildMaze()
        {
            var currentCell = _maze[0, 0];
            currentCell.IsVisited = true;
            bool isPeek = false;

            Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();
            do
            {
                GetUnvisitedCellsAround(currentCell.X, currentCell.Y, out var unvisitedCells);

                if (unvisitedCells.Count > 0)
                {
                    var nextCell = unvisitedCells[Random.Range(0, unvisitedCells.Count)];
                    nextCell.IsVisited = true;
                    stack.Push(nextCell);
                    ChangeBordersVisibility(ref currentCell, ref nextCell);
                    currentCell = nextCell;

                    if (nextCell.X == mazeSize - 1 && nextCell.Y == mazeSize - 1) GeneratePath(ref stack);

                    isPeek = false;
                }
                else
                {
                    if (!isPeek)
                    {
                        currentCell = stack.Peek();
                        isPeek = true;
                    }
                    else
                    {
                        currentCell = stack.Pop();
                        isPeek = false;
                    }
                }
            } while (stack.Count > 0);
        }

        private void GetUnvisitedCellsAround(int x, int y, out List<MazeGeneratorCell> unvisitedCells)
        {
            unvisitedCells = new List<MazeGeneratorCell>();

            if (x > 0 && !_maze[x - 1, y].IsVisited) unvisitedCells.Add(_maze[x - 1, y]);
            if (y > 0 && !_maze[x, y - 1].IsVisited) unvisitedCells.Add(_maze[x, y - 1]);
            if (x < mazeSize - 1 && !_maze[x + 1, y].IsVisited) unvisitedCells.Add(_maze[x + 1, y]);
            if (y < mazeSize - 1 && !_maze[x, y + 1].IsVisited) unvisitedCells.Add(_maze[x, y + 1]);
        }

        private void ChangeBordersVisibility(ref MazeGeneratorCell firstCell, ref MazeGeneratorCell secondCell)
        {
            if (firstCell.X == secondCell.X)
            {
                if (firstCell.Y > secondCell.Y)
                {
                    firstCell.IsWallBottomActive = false;
                    secondCell.IsWallTopActive = false;
                }
                else
                {
                    secondCell.IsWallBottomActive = false;
                    firstCell.IsWallTopActive = false;
                }
            }
            else
            {
                if (firstCell.X > secondCell.X)
                {
                    firstCell.IsWallLeftActive = false;
                    secondCell.IsWallRightActive = false;
                }
                else
                {
                    secondCell.IsWallLeftActive = false;
                    firstCell.IsWallRightActive = false;
                }
            }
        }

        private void GeneratePath(ref Stack<MazeGeneratorCell> stack)
        {
            /*if (pathPrefab == null) return;

            var position = new Vector3(0, -0.49f, 0);
            var line = Instantiate(pathPrefab, position, Quaternion.identity, transform).GetComponent<LineRenderer>();
            int index = 0;
            line.positionCount = stack.Count + 1;
            Path = new Vector3[stack.Count];

            foreach (var cell in stack)
            {
                line.SetPosition(index, ConvertToPositionXY(cell.X, cell.Y));
                Path[index] = ConvertToPositionXZ(cell.X, cell.Y);
                index++;
            }

            line.SetPosition(index, ConvertToPositionXY(0, 0));
            line.transform.rotation = Quaternion.Euler(90f, 0, 0);

            OnPathChanged?.Invoke(Path);*/
        }

        private void InstantiateMaze()
        {
            for (int x = 0; x < mazeSize; x++)
            {
                for (int y = 0; y < mazeSize; y++)
                {
                    var cell = InstantiateObject(ref mazeCell, x, y).GetComponent<MazeCell>();

                    cell.wallLeft.SetActive(_maze[x, y].IsWallLeftActive);
                    cell.wallTop.SetActive(_maze[x, y].IsWallTopActive);
                    cell.wallRight.SetActive(_maze[x, y].IsWallRightActive);
                    cell.wallBottom.SetActive(_maze[x, y].IsWallBottomActive);
                }
            }
        }

        private GameObject InstantiateObject(ref GameObject spawningObj, int x, int y)
        {
            return Instantiate(spawningObj, ConvertToPositionXZ(x, y), Quaternion.identity, transform);
        }

        private void GenerateDeathZones()
        {
            for (int i = 0; i < deathZonesCount; i++)
            {
                int x = Random.Range(0, mazeSize);
                int y = Random.Range(x == 0 ? 1 : 0, x == mazeSize - 1 ? mazeSize - 1 : mazeSize);

                Instantiate(deathZone, ConvertToPositionXZ(x, y), Quaternion.identity, transform);
            }
        }

        private Vector3 ConvertToPositionXZ(int x, int y)
        {
            float cellSize = _cellSize - _borderSize;
            return new Vector3(x * cellSize, 0f, y * cellSize);
        }

        private Vector3 ConvertToPositionXY(int x, int y)
        {
            float cellSize = _cellSize - _borderSize;
            return new Vector3(x * cellSize, y * cellSize, 0);
        }
    }
}