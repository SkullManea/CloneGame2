using System.Collections.Generic;
using UnityEngine;

public class BallChainManager : MonoBehaviour
{
    public static BallChainManager Instance;

    // =========================================================
    // PATH
    // =========================================================

    [Header("Path")]
    [Tooltip("The 7 red waypoint transforms in order.")]
    [SerializeField] private Transform[] pathPoints;

    // =========================================================
    // SPAWN POINT
    // =========================================================

    [Header("Spawn Point")]
    [Tooltip("The fixed point where new balls enter the chain.")]
    [SerializeField] private Transform spawnPoint;

    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Chain Movement")]
    [SerializeField] private float movementSpeed = 1f;

    // =========================================================
    // BALL SPACING
    // =========================================================

    [Header("Ball Spacing")]
    [SerializeField] private float ballSpacing = 0.75f;

    // =========================================================
    // BALL PREFABS
    // =========================================================

    [Header("Ball Prefabs")]
    [SerializeField] private GameObject[] ballPrefabs;

    // =========================================================
    // INTERNAL CHAIN
    // =========================================================

    private readonly List<Ball> balls =
        new List<Ball>();

    // =========================================================
    // INTERNAL PATH
    // =========================================================

    /*
     * This contains:
     *
     * [0] = SpawnPoint
     * [1] = Path Point 0
     * [2] = Path Point 1
     * [3] = Path Point 2
     * ...
     *
     * The SpawnPoint is therefore part of the
     * path calculation, but its Transform is
     * NEVER moved.
     */

    private Transform[] completePath;

    private float[] pathDistances;

    private float totalPathLength;

    /*
     * Distance travelled by the FRONT of the chain.
     */
    private float chainDistance;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        Instance = this;

        BuildCompletePath();
    }

    private void Update()
    {
        MoveChain();
    }

    // =========================================================
    // BUILD PATH
    // =========================================================

    private void BuildCompletePath()
    {
        if (spawnPoint == null)
        {
            Debug.LogError(
                "BallChainManager: Spawn Point has not been assigned."
            );

            return;
        }

        if (
            pathPoints == null ||
            pathPoints.Length < 1
        )
        {
            Debug.LogError(
                "BallChainManager: You need to assign your path points."
            );

            return;
        }

        /*
         * Create a new array that is one element
         * larger than pathPoints.
         *
         * The first element is the fixed SpawnPoint.
         */

        completePath =
            new Transform[pathPoints.Length + 1];

        completePath[0] =
            spawnPoint;

        for (
            int i = 0;
            i < pathPoints.Length;
            i++
        )
        {
            completePath[i + 1] =
                pathPoints[i];
        }

        CalculatePathDistances();
    }

    // =========================================================
    // CALCULATE PATH DISTANCES
    // =========================================================

    private void CalculatePathDistances()
    {
        if (
            completePath == null ||
            completePath.Length < 2
        )
        {
            return;
        }

        pathDistances =
            new float[completePath.Length];

        pathDistances[0] = 0f;

        totalPathLength = 0f;

        for (
            int i = 1;
            i < completePath.Length;
            i++
        )
        {
            float segmentDistance =
                Vector2.Distance(
                    completePath[i - 1].position,
                    completePath[i].position
                );

            totalPathLength +=
                segmentDistance;

            pathDistances[i] =
                totalPathLength;
        }
    }

   

    private void MoveChain()
    {
        if (balls.Count == 0)
            return;

        

        chainDistance +=
            movementSpeed *
            Time.deltaTime;

       

        RemoveBallsAtEnd();

       

        UpdateBallPositions();
    }

    

    private void UpdateBallPositions()
    {
        if (balls.Count == 0)
            return;

        for (
            int i = 0;
            i < balls.Count;
            i++
        )
        {
            Ball ball =
                balls[i];

            if (ball == null)
                continue;


            float distance =
                chainDistance -
                (i * ballSpacing);

          

            distance =
                Mathf.Max(
                    distance,
                    0f
                );

            ball.transform.position =
                GetPositionAtDistance(
                    distance
                );
        }
    }


    public Ball AddBallAtEnd(
        BallColour colour
    )
    {
        GameObject prefab =
            GetBallPrefab(
                colour
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No ball prefab found for colour: "
                + colour
            );

            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogError(
                "Spawn Point is not assigned."
            );

            return null;
        }

       

        GameObject newObject =
            Instantiate(
                prefab,
                spawnPoint.position,
                Quaternion.identity
            );

        Ball newBall =
            newObject.GetComponent<Ball>();

        if (newBall == null)
        {
            Debug.LogError(
                "The ball prefab does not have Ball.cs."
            );

            Destroy(
                newObject
            );

            return null;
        }

     

        balls.Add(
            newBall
        );



        UpdateBallPositions();

        return newBall;
    }

    

    public Ball InsertBall(
        int index,
        BallColour colour
    )
    {
        GameObject prefab =
            GetBallPrefab(
                colour
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No ball prefab found for colour: "
                + colour
            );

            return null;
        }

       

        GameObject newObject =
            Instantiate(
                prefab,
                spawnPoint.position,
                Quaternion.identity
            );

        Ball newBall =
            newObject.GetComponent<Ball>();

        if (newBall == null)
        {
            Debug.LogError(
                "The ball prefab does not have Ball.cs."
            );

            Destroy(
                newObject
            );

            return null;
        }

       

        index =
            Mathf.Clamp(
                index,
                0,
                balls.Count
            );

      

        balls.Insert(
            index,
            newBall
        );

       

        UpdateBallPositions();

        Debug.Log(
            "Inserted "
            + colour
            + " ball at index "
            + index
        );

        return newBall;
    }

    

    public void RemoveBall(
        Ball ball
    )
    {
        if (ball == null)
            return;

        if (!balls.Contains(ball))
            return;

        
        

        balls.Remove(
            ball
        );

       

        ball.Pop();

        

        UpdateBallPositions();
    }

    

    public void RemoveBalls(
        List<Ball> ballsToRemove
    )
    {
        if (
            ballsToRemove == null ||
            ballsToRemove.Count == 0
        )
        {
            return;
        }

       

        foreach (
            Ball ball
            in ballsToRemove
        )
        {
            if (ball == null)
                continue;

            if (!balls.Contains(ball))
                continue;

            balls.Remove(
                ball
            );
        }

       

        UpdateBallPositions();


        foreach (
            Ball ball
            in ballsToRemove
        )
        {
            if (ball == null)
                continue;

            ball.Pop();
        }
    }

    
 

    public int GetBallIndex(
        Ball ball
    )
    {
        return balls.IndexOf(
            ball
        );
    }

   

    public Ball GetBallAtIndex(
        int index
    )
    {
        if (
            index < 0 ||
            index >= balls.Count
        )
        {
            return null;
        }

        return balls[index];
    }

   

    public int GetBallCount()
    {
        return balls.Count;
    }

    

    public List<Ball> GetBalls()
    {
        return balls;
    }

    

    private void RemoveBallsAtEnd()
    {
        while (
            balls.Count > 0 &&
            chainDistance >
            totalPathLength +
            ballSpacing
        )
        {
            Ball ball =
                balls[0];

            
           

            balls.RemoveAt(
                0
            );

            if (ball != null)
            {
                Destroy(
                    ball.gameObject
                );
            }
        }
    }

   

    private GameObject GetBallPrefab(
        BallColour colour
    )
    {
        foreach (
            GameObject prefab
            in ballPrefabs
        )
        {
            if (prefab == null)
                continue;

            Ball ball =
                prefab.GetComponent<Ball>();

            if (ball == null)
                continue;

            if (
                ball.Colour ==
                colour
            )
            {
                return prefab;
            }
        }

        return null;
    }

  

    private Vector3 GetPositionAtDistance(
        float distance
    )
    {
        if (
            completePath == null ||
            completePath.Length < 2
        )
        {
            return spawnPoint.position;
        }

        

        if (distance <= 0f)
        {
            return spawnPoint.position;
        }

        

        if (
            distance >= totalPathLength
        )
        {
            return completePath[
                completePath.Length - 1
            ].position;
        }

       
        for (
            int i = 1;
            i < completePath.Length;
            i++
        )
        {
            if (
                distance <=
                pathDistances[i]
            )
            {
                float segmentStart =
                    pathDistances[i - 1];

                float segmentLength =
                    pathDistances[i] -
                    segmentStart;

                if (segmentLength <= 0f)
                {
                    return completePath[i].position;
                }

                float t =
                    (
                        distance -
                        segmentStart
                    ) /
                    segmentLength;

                return Vector3.Lerp(
                    completePath[i - 1].position,
                    completePath[i].position,
                    t
                );
            }
        }

        return completePath[
            completePath.Length - 1
        ].position;
    }
}