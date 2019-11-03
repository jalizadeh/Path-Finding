using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    float speed = 20f;
    Vector3[] path;
    int targetIndex;

    // Start is called before the first frame update
    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

    }

    //On each found path, this is called to be run
    private void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }

    }


    IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }


    private void OnDrawGizmos()
    {
        if(path != null)
        {
            Gizmos.color = Color.black;

            //draw cubes on the direction change points
            for (int i = 0; i < path.Length; i++)
            {
                Gizmos.DrawCube(path[i], Vector3.one);
            }

            //draw line point-to-point and unit-to-point
            for (int i = targetIndex; i< path.Length; i++)
            {
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                } else
                {
                    Gizmos.DrawLine(path[i -1] , path[i]);
                }
            }
        }
    }
}
