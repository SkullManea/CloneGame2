using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallChainManager : MonoBehaviour
{
    public static BallChainManager Instance;

    [Header("Path")]
    [SerializeField] private Transform[] pathPoints;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int totalBallsToSpawn = 30;
    [SerializeField] private float spawnInterval = 0.5f;

    [Header("Chain Movement")]
    [SerializeField] private float movementSpeed = 1f;

    [Header("Speed Progression")]
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private int firstSpeedWaypoint = 3;
    [SerializeField] private int secondSpeedWaypoint = 6;

    [Header("Ball Spacing")]
    [SerializeField] private float ballSpacing = 0.75f;

    [Header("Snap Back")]
    [SerializeField] private float snapBackSpeed = 4f;
    [SerializeField] private bool enableSnapBack = true;

    [Header("Ball Prefabs")]
    [SerializeField] private GameObject[] ballPrefabs;

    private readonly List<Ball> balls = new List<Ball>();

    private Transform[] completePath;
    private float[] pathDistances;
    private float totalPathLength;

    private float chainDistance;
    private float snapBackOffset;

    private int ballsSpawned;

    private Coroutine snapBackCoroutine;

    private bool gameEnded;
    private bool firstSpeedIncreaseTriggered;
    private bool secondSpeedIncreaseTriggered;

    private void Awake()
    {
        Instance = this;

        BuildCompletePath();
    }

    private void Start()
    {
        StartCoroutine(SpawnBallSequence());
    }

    private void Update()
    {
        if (gameEnded)
        {
            return;
        }

        MoveChain();
    }

    private void BuildCompletePath()
    {
        if (spawnPoint == null)
        {
            Debug.LogError(
                "BallChainManager: SpawnPoint has not been assigned."
            );

            return;
        }

        if (
            pathPoints == null ||
            pathPoints.Length == 0
        )
        {
            Debug.LogError(
                "BallChainManager: No path points assigned."
            );

            return;
        }

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
        {
            return;
        }

        chainDistance +=
            movementSpeed *
            Time.deltaTime;

        CheckSpeedProgression();

        RemoveBallsAtEnd();

        UpdateBallPositions();
    }

    private void CheckSpeedProgression()
    {
        float frontDistance =
            chainDistance -
            snapBackOffset;

        int currentWaypoint =
            GetWaypointAtDistance(
                frontDistance
            );

        if (
            !firstSpeedIncreaseTriggered &&
            currentWaypoint >= firstSpeedWaypoint
        )
        {
            movementSpeed +=
                speedIncreaseAmount;

            firstSpeedIncreaseTriggered =
                true;

            Debug.Log(
                "Reached waypoint " +
                firstSpeedWaypoint +
                ". Speed increased to " +
                movementSpeed
            );
        }

        if (
            !secondSpeedIncreaseTriggered &&
            currentWaypoint >= secondSpeedWaypoint
        )
        {
            movementSpeed +=
                speedIncreaseAmount;

            secondSpeedIncreaseTriggered =
                true;

            Debug.Log(
                "Reached waypoint " +
                secondSpeedWaypoint +
                ". Speed increased to " +
                movementSpeed
            );
        }
    }

    private int GetWaypointAtDistance(
        float distance
    )
    {
        if (
            pathDistances == null ||
            pathDistances.Length == 0
        )
        {
            return 0;
        }

        for (
            int i = 1;
            i < pathDistances.Length;
            i++
        )
        {
            if (
                distance <
                pathDistances[i]
            )
            {
                return i - 1;
            }
        }

        return pathDistances.Length - 1;
    }

    private void UpdateBallPositions()
    {
        if (balls.Count == 0)
        {
            return;
        }

        for (
            int i = 0;
            i < balls.Count;
            i++
        )
        {
            Ball ball =
                balls[i];

            if (ball == null)
            {
                continue;
            }

            float distance =
                chainDistance -
                (i * ballSpacing) -
                snapBackOffset;

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

    private IEnumerator SpawnBallSequence()
    {
        ballsSpawned = 0;

        while (
            ballsSpawned <
            totalBallsToSpawn
        )
        {
            if (gameEnded)
            {
                yield break;
            }

            SpawnBall();

            ballsSpawned++;

            yield return new WaitForSeconds(
                spawnInterval
            );
        }

        Debug.Log(
            "Finished spawning " +
            ballsSpawned +
            " balls."
        );
    }

    private void SpawnBall()
    {
        BallColour colour =
            GetRandomColour();

        GameObject prefab =
            GetBallPrefab(
                colour
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No prefab found for " +
                colour
            );

            return;
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
                "Ball prefab requires Ball.cs."
            );

            Destroy(
                newObject
            );

            return;
        }

        balls.Add(
            newBall
        );

        UpdateBallPositions();
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
                "No prefab found for " +
                colour
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

        return newBall;
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

        int removedCount = 0;

        foreach (
            Ball ball
            in ballsToRemove
        )
        {
            if (ball == null)
            {
                continue;
            }

            if (!balls.Contains(ball))
            {
                continue;
            }

            balls.Remove(
                ball
            );

            removedCount++;
        }

        foreach (
            Ball ball
            in ballsToRemove
        )
        {
            if (ball == null)
            {
                continue;
            }

            ball.Pop();
        }

        if (
            ScoreManager.Instance != null &&
            removedCount > 0
        )
        {
            ScoreManager.Instance.AddScore(
                removedCount
            );
        }

        UpdateBallPositions();

        if (
            enableSnapBack &&
            removedCount > 0
        )
        {
            float distanceToMoveBack =
                removedCount *
                ballSpacing;

            StartSnapBack(
                distanceToMoveBack
            );
        }

        CheckForGameOver();
    }

    private void StartSnapBack(
        float distance
    )
    {
        if (snapBackCoroutine != null)
        {
            StopCoroutine(
                snapBackCoroutine
            );
        }

        snapBackCoroutine =
            StartCoroutine(
                SnapBack(
                    distance
                )
            );
    }

    private IEnumerator SnapBack(
        float distance
    )
    {
        float targetOffset =
            snapBackOffset +
            distance;

        while (
            snapBackOffset <
            targetOffset
        )
        {
            if (gameEnded)
            {
                yield break;
            }

            snapBackOffset =
                Mathf.MoveTowards(
                    snapBackOffset,
                    targetOffset,
                    snapBackSpeed *
                    Time.deltaTime
                );

            UpdateBallPositions();

            yield return null;
        }

        snapBackOffset =
            targetOffset;

        UpdateBallPositions();

        snapBackCoroutine =
            null;
    }

    private void RemoveBallsAtEnd()
    {
        while (
            balls.Count > 0
        )
        {
            float frontDistance =
                chainDistance -
                snapBackOffset;

            if (
                frontDistance <
                totalPathLength +
                ballSpacing
            )
            {
                break;
            }

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

        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (gameEnded)
        {
            return;
        }

        if (
            ballsSpawned >=
            totalBallsToSpawn &&
            balls.Count == 0
        )
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        if (snapBackCoroutine != null)
        {
            StopCoroutine(
                snapBackCoroutine
            );

            snapBackCoroutine = null;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ShowScorePanel();
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
            {
                continue;
            }

            Ball ball =
                prefab.GetComponent<Ball>();

            if (ball == null)
            {
                continue;
            }

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

    private BallColour GetRandomColour()
    {
        int numberOfColours =
            System.Enum.GetValues(
                typeof(BallColour)
            ).Length;

        return (BallColour)Random.Range(
            0,
            numberOfColours
        );
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
            distance >=
            totalPathLength
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