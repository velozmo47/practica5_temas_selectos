using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
	[SerializeField] GameObject mazeWall;
	[SerializeField] GameObject mazeFloor;
	[SerializeField] GameObject mazePassage;
	[SerializeField] int m_nMazeWidth = 5;
	[SerializeField] int m_nMazeHeight = 5;

	[SerializeField] float waitInterval = 3f;

	CELL_PATH[] m_maze;

	// Some bit fields for convenience
	enum CELL_PATH
	{
		CELL_PATH_N = 0x01,
		CELL_PATH_E = 0x02,
		CELL_PATH_S = 0x04,
		CELL_PATH_W = 0x08,
		CELL_VISITED = 0x10,
	};

	// Algorithm variables
	int m_nVisitedCells;
	Stack<Vector2Int> m_stack;  // (x, y) coordinate pairs
	[SerializeField] int m_nPathWidth = 1;

	// Called by olcConsoleGameEngine
	void Start()
	{
		m_maze = new CELL_PATH[m_nMazeWidth * m_nMazeHeight];
		m_stack = new Stack<Vector2Int>();

		// Choose a starting cell
		int x = Random.Range(0, m_nMazeWidth - 1);
		int y = Random.Range(0, m_nMazeHeight - 1);
		m_stack.Push(new Vector2Int(x, y));
		m_maze[y * m_nMazeWidth + x] = CELL_PATH.CELL_VISITED;
		m_nVisitedCells = 1;

		CreateMaze();
	}

	int Offset(int x, int y)
	{
		return (m_stack.Peek().y + y) * m_nMazeWidth + (m_stack.Peek().x + x);
	}

	// Called by olcConsoleGameEngine
	void CreateMaze()
	{
        // Do Maze Algorithm
        while (m_nVisitedCells < m_nMazeWidth * m_nMazeHeight)
		{
			// Create a set of unvisted neighbours
			List<int> neighbours = new List<int>();

			// North neighbour
			if (m_stack.Peek().y > 0 && (m_maze[Offset(0, -1)] & CELL_PATH.CELL_VISITED) == 0)
				neighbours.Add(0);
			// East neighbour
			if (m_stack.Peek().x < m_nMazeWidth - 1 && (m_maze[Offset(1, 0)] & CELL_PATH.CELL_VISITED) == 0)
				neighbours.Add(1);
			// South neighbour
			if (m_stack.Peek().y < m_nMazeHeight - 1 && (m_maze[Offset(0, 1)] & CELL_PATH.CELL_VISITED) == 0)
				neighbours.Add(2);
			// West neighbour
			if (m_stack.Peek().x > 0 && (m_maze[Offset(-1, 0)] & CELL_PATH.CELL_VISITED) == 0)
				neighbours.Add(3);

			// Are there any neighbours available?
			if (neighbours.Count > 0)
			{
				// Choose one available neighbour at random
				int next_cell_dir = neighbours[Random.Range(0, 1000) % neighbours.Count];

				// Create a path between the neighbour and the current cell
				switch (next_cell_dir)
				{
					case 0: // North
						m_maze[Offset(0, -1)] |= CELL_PATH.CELL_VISITED | CELL_PATH.CELL_PATH_S;
						m_maze[Offset(0, 0)] |= CELL_PATH.CELL_PATH_N;
						m_stack.Push(new Vector2Int((m_stack.Peek().x + 0), (m_stack.Peek().y - 1)));
						break;

					case 1: // East
						m_maze[Offset(+1, 0)] |= CELL_PATH.CELL_VISITED | CELL_PATH.CELL_PATH_W;
						m_maze[Offset(0, 0)] |= CELL_PATH.CELL_PATH_E;
						m_stack.Push(new Vector2Int((m_stack.Peek().x + 1), (m_stack.Peek().y + 0)));
						break;

					case 2: // South
						m_maze[Offset(0, +1)] |= CELL_PATH.CELL_VISITED | CELL_PATH.CELL_PATH_N;
						m_maze[Offset(0, 0)] |= CELL_PATH.CELL_PATH_S;
						m_stack.Push(new Vector2Int((m_stack.Peek().x + 0), (m_stack.Peek().y + 1)));
						break;

					case 3: // West
						m_maze[Offset(-1, 0)] |= CELL_PATH.CELL_VISITED | CELL_PATH.CELL_PATH_E;
						m_maze[Offset(0, 0)] |= CELL_PATH.CELL_PATH_W;
						m_stack.Push(new Vector2Int((m_stack.Peek().x - 1), (m_stack.Peek().y + 0)));
						break;

				}

				m_nVisitedCells++;
			}
			else
			{
				// No available neighbours so backtrack!
				m_stack.Pop();
			}
		}

		//StartCoroutine(DrawMaze());
		DrawMaze();
	}

    // === DRAWING STUFF ===
    void DrawMaze()
    {
		// Clear Screen by drawing 'spaces' everywhere

		// Draw Maze
		for (int x = 0; x < m_nMazeWidth; x++)
		{
			for (int y = 0; y < m_nMazeHeight; y++)
			{
				// Each cell is inflated by m_nPathWidth, so fill it in
				for (int py = 0; py < m_nPathWidth; py++)
				{
					for (int px = 0; px < m_nPathWidth; px++)
					{
						if ((m_maze[y * m_nMazeWidth + x] & CELL_PATH.CELL_VISITED) == CELL_PATH.CELL_VISITED)
						{
							Vector3 pos = new Vector3(x * (m_nPathWidth + 1) + px, 0, y * (m_nPathWidth + 1) + py);

							CreateCell(mazeFloor, pos);
						}
					}
				}

				// Draw passageways between cells
				for (int p = -1; p < m_nPathWidth; p++)
				{
					GameObject model;
					if ((m_maze[y * m_nMazeWidth + x] & CELL_PATH.CELL_PATH_S) == CELL_PATH.CELL_PATH_S)
					{
						model = p == -1 ? mazeWall : mazePassage;
					}
					else
					{
						model = mazeWall;
					}

					Vector3 pos = new Vector3(x * (m_nPathWidth + 1) + p, 0, y * (m_nPathWidth + 1) + m_nPathWidth);

					CreateCell(model, pos);


					if ((m_maze[y * m_nMazeWidth + x] & CELL_PATH.CELL_PATH_E) == CELL_PATH.CELL_PATH_E)
					{
						model = p == -1 ? mazeWall : mazePassage;
					}
					else
					{
						model = mazeWall;
					}

					pos = new Vector3(x * (m_nPathWidth + 1) + m_nPathWidth, 0, y * (m_nPathWidth + 1) + p);

					CreateCell(model, pos);

					if (y == 0)
					{
						if ((m_maze[y * m_nMazeWidth + x] & CELL_PATH.CELL_PATH_N) == CELL_PATH.CELL_PATH_N)
						{
							model = mazePassage;
						}
						else
						{
							model = mazeWall;
						}

						pos = new Vector3(x * (m_nPathWidth + 1) + p, 0, y * (m_nPathWidth + 1) - 1);

						CreateCell(model, pos);
					}

					if (x == 0)
                    {
						if ((m_maze[y * m_nMazeWidth + x] & CELL_PATH.CELL_PATH_W) == CELL_PATH.CELL_PATH_W)
						{
							model = mazePassage;
						}
						else
						{
							model = mazeWall;
						}

						pos = new Vector3(x * (m_nPathWidth + 1) - 1, 0, y * (m_nPathWidth + 1) + p);

						CreateCell(model, pos);
					}

					if (x == m_nMazeWidth - 1 && y == m_nMazeHeight - 1)
					{
						pos = new Vector3(x * (m_nPathWidth + 1) + m_nPathWidth, 0, y * (m_nPathWidth + 1) + m_nPathWidth);

						CreateCell(mazeWall, pos);
					}
                }
            }
		}

        StaticBatchingUtility.Combine(gameObject);
    }

    void CreateCell(GameObject model, Vector3 pos)
    {
		GameObject newGameObject = Instantiate(model, pos, Quaternion.identity, transform);
		//newGameObject.transform.localScale = Vector3.one * 2;
    }
}