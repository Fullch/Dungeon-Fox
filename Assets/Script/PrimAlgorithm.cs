using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrimAlgorithm : MonoBehaviour
{
    public List<(Habitacion, Habitacion)> Prim(List<Habitacion> rooms)
    {
        // Create a dictionary to store the neighbors of each room
        Dictionary<Habitacion, List<(Habitacion, int)>> graph = new Dictionary<Habitacion, List<(Habitacion, int)>>();
        foreach (Habitacion room in rooms)
        {
            graph[room] = new List<(Habitacion, int)>();
        }

        // Add all edges to the graph with their weights
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                Habitacion room1 = rooms[i];
                Habitacion room2 = rooms[j];
                int distance = (int)Vector2.Distance(room1.getGameObject().transform.position, room2.getGameObject().transform.position); // Assuming Room objects have a DistanceTo method
                graph[room1].Add((room2, distance));
                graph[room2].Add((room1, distance));
            }
        }

        // Initialize the algorithm
        HashSet<Habitacion> visited = new HashSet<Habitacion>();
        List<(Habitacion, Habitacion)> edges = new List<(Habitacion, Habitacion)>();
        PriorityQueue<(Habitacion, Habitacion, int)> pq = new PriorityQueue<(Habitacion, Habitacion, int)>((x, y) => x.Item3.CompareTo(y.Item3));
        foreach ((Habitacion, int) neighbor in graph[rooms[0]])
        {
            pq.Enqueue((rooms[0], neighbor.Item1, neighbor.Item2));
        }
        visited.Add(rooms[0]);

        // Run the algorithm
        while (pq.Count > 0)
        {
            (Habitacion, Habitacion, int) edge = pq.Dequeue();
            Habitacion room1 = edge.Item1;
            Habitacion room2 = edge.Item2;
            int distance = edge.Item3;

            if (visited.Contains(room2))
            {
                continue;
            }

            visited.Add(room2);
            edges.Add((room1, room2));

            foreach ((Habitacion, int) neighbor in graph[room2])
            {
                if (!visited.Contains(neighbor.Item1))
                {
                    pq.Enqueue((room2, neighbor.Item1, neighbor.Item2));
                }
            }
        }

        return edges;
    }
}

public class PriorityQueue<T>
{
    private List<T> data;
    private Comparison<T> comparison;

    public PriorityQueue(Comparison<T> comparison)
    {
        this.data = new List<T>();
        this.comparison = comparison;
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int childIndex = data.Count - 1;
        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if (comparison(data[childIndex], data[parentIndex]) < 0)
            {
                T tmp = data[childIndex];
                data[childIndex] = data[parentIndex];
                data[parentIndex] = tmp;
                childIndex = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    public T Dequeue()
    {
        int lastIndex = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);

        lastIndex--;

        int parentIndex = 0;
        while (true)
        {
            int childIndex = parentIndex * 2 + 1;
            if (childIndex > lastIndex) break;

            int rightChild = childIndex + 1;
            if (rightChild <= lastIndex && comparison(data[rightChild], data[childIndex]) < 0)
            {
                childIndex = rightChild;
            }
            if (comparison(data[childIndex], data[parentIndex]) < 0)
            {
                T tmp = data[parentIndex];
                data[parentIndex] = data[childIndex];
                data[childIndex] = tmp;
                parentIndex = childIndex;
            }
            else
            {
                break;
            }
        }
        return frontItem;
    }

    public int Count
    {
        get { return data.Count; }
    }
}

