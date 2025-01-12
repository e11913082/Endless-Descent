using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Vector2[] scanDirections;
    private float[] interest;
    private float[] danger;
    private ContactFilter2D obstacleContactFilter;
    private ContactFilter2D targetContactFilter;
    private ContactFilter2D pointContactFilter;
    private Vector2 lastKnownPosition;
    private float acceptableRange = 0.1f;
    private Vector2 returnPoint;
    private List<Vector2> mileStones = new List<Vector2> ();
    private float obstacleScanRadius = 1.51f;
    private int directionNumber = 32;
    // Start is called before the first frame update

    void Awake()
    {
        scanDirections = new Vector2[directionNumber];
        interest = new float[directionNumber];
        danger = new float[directionNumber];
    }
    void Start()
    {
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 360f / directionNumber);
        scanDirections[0] = Vector2.up;

        for (int i = 1; i < directionNumber; i++)
        {
            scanDirections[i] = rotation * scanDirections[i - 1];
        }
        obstacleContactFilter = new ContactFilter2D() {
            useTriggers = false,
            layerMask = ~ LayerMask.GetMask("Player", "Enemy", "Weapon"),
            useLayerMask = true
            };
        targetContactFilter = new ContactFilter2D() {
            useTriggers = false,
            layerMask = ~ LayerMask.GetMask("Player", "Enemy"),
            useLayerMask = true
            };
        pointContactFilter = new ContactFilter2D() {
            useTriggers = false,
            layerMask = ~ LayerMask.GetMask("Player", "Enemy"),
            useLayerMask = true
            };
    }

    public (bool, Vector2) GetNextMoveDirection(Vector2 originPosition, Vector2 targetPosition)
    {
        if (!PointVisible(originPosition, mileStones.Last()))
        {
            mileStones.Add(originPosition);
        }

        Vector2 targetDirection = targetPosition - originPosition;
        bool targetVisible = TargetVisible(originPosition, targetDirection);
        bool targetContact = true;

        if (targetVisible is true)
        {
            lastKnownPosition = targetPosition;
            targetContact = true;
        }
        else if ((lastKnownPosition - originPosition).magnitude < acceptableRange)
        {
            targetContact = false;
            return (targetContact, Vector2.zero);
        }
        else
        {
            targetContact = true;
        }

        targetDirection = lastKnownPosition - originPosition;
        Vector2 nextMoveDirection = CalculateMoveDirection(originPosition, lastKnownPosition - originPosition);
        Debug.DrawLine(originPosition, originPosition + nextMoveDirection, Color.green);

        return (targetContact, nextMoveDirection);
    }

    public (bool, Vector2) GetReturnMoveDirection(Vector2 originPosition)
    {
        if (PointVisible(originPosition, returnPoint))
        {
            mileStones.Clear();
            mileStones.Add(returnPoint);
        }

        float distanceNextMileStone = (mileStones.Last() - originPosition).magnitude;
        if (distanceNextMileStone < acceptableRange)
        {
            mileStones.Remove(mileStones.Last());
        }
        
        if (mileStones.Count == 0)
        {
            mileStones.Add(returnPoint);
            return (true, Vector2.zero);
        }
    
        Vector2 returnMoveDirection = CalculateMoveDirection(originPosition, mileStones.Last() - originPosition);
        Debug.DrawLine(originPosition, originPosition + returnMoveDirection);
        return (false, returnMoveDirection);
    }

    private Vector2 CalculateMoveDirection(Vector2 originPosition, Vector2 interestDirection)
    {
        //CalculateInterest(originPosition, targetDirection);
        CalculateInterest(originPosition, interestDirection);
        CalculateDanger(originPosition);

        Vector2 nextMoveDirection = Vector2.zero;

        for (int i = 0; i < directionNumber; i++)
        {
            if (interest[i] < 0.01f) {danger[i] = 0f;}
            float directionWeight = interest[i] - danger[i];
            //if (directionWeight < -1.2f) {directionWeight = -1.2f;}
            //else if (directionWeight > 1.2f) {directionWeight = 1.2f;}
            nextMoveDirection +=  directionWeight * scanDirections[i];
        }
        nextMoveDirection.Normalize();
        Debug.DrawLine(originPosition, (Vector2) originPosition + nextMoveDirection);

        return nextMoveDirection;
    }

    public bool TargetVisible(Vector2 originPosition, Vector2 targetDirection)
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int noHits = Physics2D.Raycast(originPosition, targetDirection.normalized, targetContactFilter, hits, targetDirection.magnitude);
        return noHits == 0;
    }

    private bool PointVisible(Vector2 originPosition, Vector2 targetPoint)
    {
        Vector2 direction = targetPoint - originPosition;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int noHits = Physics2D.Raycast(originPosition, direction.normalized, targetContactFilter, hits, direction.magnitude);
        return noHits == 0;
    }
    private void CalculateInterest(Vector2 originPosition, Vector2 targetDirection)
    {
        for (int i = 0; i < directionNumber; i++)
        {
            float targetAngle = Mathf.Abs(Vector2.Angle(targetDirection, scanDirections[i]));
            if (targetAngle > 90.0f)
            {
                interest[i] = 0.0f;
            }
            else
            {
                interest[i] = Mathf.Cos(targetAngle * Mathf.Deg2Rad);
            }
            Debug.DrawLine(originPosition, originPosition + interest[i] * scanDirections[i], Color.blue);
        }
    }

    private void CalculateDanger(Vector2 originPosition)
    {
        for (int i = 0; i < directionNumber; i++)
        {
            RaycastHit2D[] hits = new RaycastHit2D[2];
            int numHits = Physics2D.Raycast(originPosition, scanDirections[i], obstacleContactFilter, hits, obstacleScanRadius);
            
            if (numHits == 0) // ray hits nothing
            {
                danger[i] = 0.0f;
            }
            else
            {
                // danger[i] = (1.0f - Math.Abs(hits[1].distance - hits[0].distance) / targetDirection.magnitude) * 2f;
                //float obstacleDistance = hits[1].distance - hits[0].distance;
                float obstacleDistance = hits[0].distance;
                if (obstacleDistance > obstacleScanRadius) {danger[i] = 0f;}
                else {danger[i] = (1.0f - obstacleDistance / obstacleScanRadius) * 4f;}
                if (danger[i] < 0f) {danger[i] = 0f;}
            }

            Debug.DrawLine(originPosition, originPosition - danger[i] * scanDirections[i], Color.black);
        }
    }

    public void SetReturnPoint(Vector2 retPoint)
    {
        returnPoint = retPoint;
        if (mileStones.Count == 0)
        {
            mileStones.Add(returnPoint);
        }
    }
}
