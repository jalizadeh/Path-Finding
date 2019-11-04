using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour
{
    //for accessing data from `static void RequestPath`
    static PathRequestManager instance;
    PathFinding pathFinding;

    Queue<PathResult> results = new Queue<PathResult>();

    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }


    private void Update()
    {
        if(results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    //Every unit requests the path from this method, and this method will return
    // via Action, the data, after processing (NOT immediately)
    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathFinding.FindPath(request, instance.FinishedProcessingPath);
        };

        threadStart.Invoke();
    }


    public void FinishedProcessingPath(PathResult result) {
        lock (results) {
            results.Enqueue(result);
        }
    }
    
}


public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }

}


//The structure of the data for each path request
public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
