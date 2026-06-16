using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DfsGizmoVisualizer : MonoBehaviour
{
    [Header("Grid")]
    [Tooltip("가로 칸 수입니다.")]
    [SerializeField] private int width = 3;

    [Tooltip("세로 칸 수입니다.")]
    [SerializeField] private int height = 3;

    [Tooltip("Scene 뷰에 그릴 칸 크기입니다.")]
    [SerializeField] private float cellSize = 1f;

    private readonly Stack<Vector2Int> frontier = new Stack<Vector2Int>();
    private readonly HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
    private Vector2Int currentNode;

    private void Start()
    {
        ResetSearch();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StepSearch();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetSearch();
        }
    }

    private void ResetSearch()
    {
        frontier.Clear();
        visited.Clear();

        currentNode = new Vector2Int(0, 0);
        frontier.Push(currentNode);
        visited.Add(currentNode);
    }

    private void StepSearch()
    {
        if (frontier.Count == 0)
        {
            return;
        }

        currentNode = frontier.Pop();

        foreach (Vector2Int neighbor in GetNeighbors(currentNode))
        {
            if (visited.Contains(neighbor))
            {
                continue;
            }

            visited.Add(neighbor);

            frontier.Push(neighbor);
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int next = node + direction;

            if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
            {
                continue;
            }

            neighbors.Add(next);
        }

        return neighbors;
    }

    private void OnDrawGizmos()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int node = new Vector2Int(x, y);
                Vector3 position = transform.position + new Vector3(x * cellSize, 0f, y * cellSize);

                Gizmos.color = GetNodeColor(node);
                Gizmos.DrawCube(position, Vector3.one * (cellSize * 0.8f));

                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(position, Vector3.one * (cellSize * 0.8f));
            }
        }
    }

    private Color GetNodeColor(Vector2Int node)
    {
        if (Application.isPlaying && node == currentNode)
        {
            return Color.yellow;
        }

        if (Application.isPlaying && visited.Contains(node))
        {
            return Color.magenta;
        }

        return Color.gray;
    }
}