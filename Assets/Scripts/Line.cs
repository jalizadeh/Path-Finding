﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    const float verticalLineGradient = 1e5f;

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    float gradientPerpendicular;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        //if dx equals 0, we should use a constant value
        // experiment more in https://www.geogebra.org/m/JkysyNJS
        if (dx == 0)
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }

        //it comes from the rule: `gradient * gradientPerpendicular = -1`, but if
        if (gradientPerpendicular == 0)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }


        //Where the line intercepts Y axis
        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }


    bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3
            .up;

        Gizmos.DrawLine(lineCenter - lineDir * length / 2f, lineCenter + lineDir * length / 2f);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    /*
     * Y1 = m1X + c1
     * Y2 = m2X + c2
     * 
     * Y1 = Y2 =>   m1X + c1 = m2X + c2
     *              X(m1 - m2) = c2 - c1
     *              X = (c2 -c1) / (m1 - m2)
     */ 
    public float DistanceFromPoint(Vector2 p)
    {
        float yIntersectPerpendicular = p.y - gradientPerpendicular * p.x;
        float intersectX = (yIntersectPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
        float intersectY = gradient * intersectX + y_intercept;
        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }
}
