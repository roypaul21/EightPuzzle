using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;

public class BFSScript : MonoBehaviour
{

    public void bfs()
    {
        AStarSearch("071583246");
    }

    public static List<string> AStarSearch(string initialState)
    {
        if (IsWinCondition(initialState))
        {
            return new List<string> { initialState };
        }

        Dictionary<string, string> visitedMap = new Dictionary<string, string>();
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(initialState);


        Dictionary<string, int> list_val = new Dictionary<string, int>();
        List<int> f_list = new List<int>();

        List<string> path_list = new List<string>();

        int nodeDepth = 1;
        while (queue.Count > 0)
        {
            string node = queue.Dequeue(); //remove from queue
            //Debug.Log("Depth == " + nodeDepth);
            //Debug.Log("Node == " + node);
            foreach (string neighbour in GetNeighbourStates(node))
            {
                if (!visitedMap.ContainsKey(neighbour))
                {
                    //Debug.Log("child = " + neighbour);
                    int md = GetHValue(neighbour, nodeDepth);
                    //Debug.Log(neighbour + " == " + md);
                    f_list.Add(md);
                    list_val.Add(neighbour, md);
                }
               

            }

            foreach (KeyValuePair<string, int> path in list_val)
            {
                if (path.Value == f_list.AsQueryable().Min())
                {
                    path_list.Add(path.Key);
                }
            }

            foreach (string paths in path_list)
            {
                visitedMap.Add(paths, node);
                queue.Enqueue(paths); // add queue 

                if (IsWinCondition(paths))
                {
                    Debug.Log("Solution to Path Found");
                    return GeneratePath(visitedMap, paths);
                }
            }

            path_list.Clear();
            list_val.Clear();
            f_list.Clear();
            nodeDepth++;

            if (!visitedMap.ContainsKey(node))
            {
                visitedMap.Add(node, "");
            }
        }
        Debug.Log("Cannot be Solved!");
        return null;
    }

    public static List<string> GeneratePath(Dictionary<string, string> parentMap, string endState)
    {
        Debug.Log("GeneratePath");
        List<string> path = new List<string>();
        string parent = endState;
        while (parentMap.ContainsKey(parent))
        {
            path.Add(parent);
            parent = parentMap[parent];
        }
        foreach(var x in path)
        {
            Debug.Log(x);
        }
        
        return path;
    }

    //Goal State Tile Position
    public static bool IsWinCondition(string state)
    {
        return state.Equals("012345678");
    }

    //get the manhattan distance
    public static int GetHValue(string State, int depth)
    {
        string GoalState = "012345678";
        int hval = 0;
        for (int x = 0; x < 9; x++)
        {
            if (GoalState[x] != State[x])
                hval++;
        }

        int manhattan = hval + depth;

        return manhattan;
    }

    public static List<string> GetNeighbourStates(string state)
    {
        int emptyIndex = state.IndexOf("0");

        bool canMoveLeft = emptyIndex % 3 > 0;
        bool canMoveRight = emptyIndex % 3 < 2;
        bool canMoveUp = emptyIndex / 3 > 0;
        bool canMoveDown = emptyIndex / 3 < 2;

        List<string> neighbours = new List<string>();

        if (canMoveLeft)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex - 1];
            sb[emptyIndex] = newChar;
            sb[emptyIndex - 1] = '0';
            neighbours.Add(sb.ToString());

        }
        if (canMoveRight)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex + 1];
            sb[emptyIndex] = newChar;
            sb[emptyIndex + 1] = '0';
            neighbours.Add(sb.ToString());

        }
        if (canMoveUp)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex - 3];
            sb[emptyIndex] = newChar;
            sb[emptyIndex - 3] = '0';
            neighbours.Add(sb.ToString());

        }
        if (canMoveDown)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex + 3];
            sb[emptyIndex] = newChar;
            sb[emptyIndex + 3] = '0';
            neighbours.Add(sb.ToString());

        }

        return neighbours;
    }

}
