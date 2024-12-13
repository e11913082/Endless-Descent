using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Vector2[] scanDirections = new Vector2[16];
    private float[] interest = new float[16];
    private float[] danger = new float[16];
    private ContactFilter2D obstacleContactFilter;
    private ContactFilter2D targetContactFilter;
    private ContactFilter2D pointContactFilter;
    private Vector2 lastKnownPosition;
    private float acceptableRange = 0.1f;
    private Vector2 returnPoint;
    private List<Vector2> mileStones = new List<Vector2> ();
    // Start is called before the first frame update
    void Start()
    {
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 22.5f);
        scanDirections[0] = Vector2.up;

        for (int i = 1; i < 16; i++)
        {
            scanDirections[i] = rotation * scanDirections[i - 1];
        }
        obstacleContactFilter = new ContactFilter2D() {
            useTriggers = false,
            layerMask = ~ LayerMask.GetMask("Player", "Enemy"),
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
        Vector2 nextMoveDirection = CalculateMoveDirection(originPosition, targetDirection);

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
        return (false, returnMoveDirection);
    }

    private Vector2 CalculateMoveDirection(Vector2 originPosition, Vector2 targetDirection)
    {
        CalculateInterest(originPosition, targetDirection);
        CalculateDanger(originPosition, targetDirection);

        Vector2 nextMoveDirection = Vector2.zero;

        for (int i = 0; i < 16; i++)
        {
            nextMoveDirection += (interest[i] - danger[i]) * scanDirections[i];
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
        for (int i = 0; i < 16; i++)
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
            Debug.DrawLine(originPosition, originPosition + interest[i] * scanDirections[i]);
        }
    }

    private void CalculateDanger(Vector2 originPosition, Vector2 targetDirection)
    {
        for (int i = 0; i < 16; i++)
        {
            RaycastHit2D[] hits = new RaycastHit2D[2];
            int numHits = Physics2D.Raycast(originPosition, scanDirections[i], obstacleContactFilter, hits, targetDirection.magnitude);
            
            if (numHits == 1) // ray hits own collider
            {
                danger[i] = 0.0f;
            }
            else if (numHits > 1)
            {
                danger[i] = (1.0f - (hits[1].distance - hits[0].distance) / targetDirection.magnitude) * 1.5f;
            }

            Debug.DrawLine(originPosition, originPosition + danger[i] * scanDirections[i]);
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
