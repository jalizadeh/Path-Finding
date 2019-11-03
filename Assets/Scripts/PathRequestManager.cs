using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    PathFinding pathFinding;

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    //for accessing data from `static void RequestPath`
    static PathRequestManager instance;

    bool isProcessing;

    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }


    //Every unit requests the path from this method, and this method will return
    // via Action, the data, after processing (NOT immediately)
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newPathRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newPathRequest);
        instance.TryProcessingNext();
    }


    //Process each requested path, untill there is no more
    private void TryProcessingNext()
    {
        if(!isProcessing && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessing = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }


    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentPathRequest.callback(path, success);
        isProcessing = false;
        TryProcessingNext();
    }



    //The structure of the data for each path request
    struct PathRequest
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
}
