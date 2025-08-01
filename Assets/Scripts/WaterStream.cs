using NUnit.Framework;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class WaterStream : MonoBehaviour
{
    public static List<WaterStream> waterStreams = new List<WaterStream>();

    LineRenderer lineRenderer;
    float[] segmentLengthProgress;
    float lineLength;

    float startPoint;
    float strengthAtStartPoint;
    float endPoint;
    float strengthAtEndPoint;

    bool isStreaming = false;

    float speed = 1f; // Speed of the water streams

    public float minDistance = 0.1f;
    public float maxDistance = 5f;

    public float strengthDamping = 0.1f;
    public float speedDamping = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waterStreams.Add(this);

        lineRenderer = GetComponent<LineRenderer>();
        Vector2 prevPoint = lineRenderer.GetPosition(0);
        float currentLength = 0;
        segmentLengthProgress = new float[lineRenderer.positionCount];
        segmentLengthProgress[0] = 0;
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            currentLength += Vector2.Distance(lineRenderer.GetPosition(i), prevPoint);
            prevPoint = lineRenderer.GetPosition(i);
        }
        lineLength = currentLength;
        prevPoint = lineRenderer.GetPosition(0);
        currentLength = 0;
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            currentLength += Vector2.Distance(lineRenderer.GetPosition(i), prevPoint);
            segmentLengthProgress[i] = currentLength/lineLength;
            prevPoint = lineRenderer.GetPosition(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStreaming)
        {
            startPoint += Time.deltaTime;
            strengthAtStartPoint -= strengthDamping * Time.deltaTime * speed;
            endPoint += Time.deltaTime;
            strengthAtEndPoint -= strengthDamping * Time.deltaTime * speed;

            speed -= speedDamping * Time.deltaTime;
            if (endPoint >= 1)
            {
                isStreaming = false;
            }
        }
    }


    public void StartStream(float length, float strength, float speed)
    {
        this.speed = speed;
        isStreaming = true;
        startPoint = 0;
        strengthAtStartPoint = strength;
        endPoint = -length;
        strengthAtEndPoint = 0;
    }

    float GetVelocityAtIndex(int point)
    {
        return Mathf.Lerp(strengthAtEndPoint, strengthAtStartPoint, Mathf.InverseLerp(endPoint, startPoint, segmentLengthProgress[point]));
    }
    public Vector2 GetMoveVector(Vector3 worldPos)
    {
        Vector3 velocity = Vector3.zero;
        int point = GetClosestPointIndexOnLine(worldPos);
        float vel = GetVelocityAtIndex(point);
        if(point == lineRenderer.positionCount -1) point--;
        Vector2 direction = (lineRenderer.GetPosition(point+1) - lineRenderer.GetPosition(point)).normalized;
        float distance = 1 - Mathf.InverseLerp(minDistance, maxDistance, Vector2.Distance(worldPos, lineRenderer.GetPosition(point)));
        Vector2 force = direction * vel * Mathf.Pow(distance, 2);
        Debug.Log($"Point: {point}, Distance: {Vector2.Distance(worldPos, lineRenderer.GetPosition(point))}, Force: {force}, Velocity: {vel}");
        return force;
    }

    int GetClosestPointIndexOnLine(Vector3 worldPos)
    {
        float minDist = float.MaxValue;
        int closestPoint = 0;    
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            float dist = Vector2.Distance(worldPos, lineRenderer.GetPosition(i));  
            if(dist < minDist)
            {
                minDist = dist;
                closestPoint = i;
            }
        }
        return closestPoint;
    }
}
