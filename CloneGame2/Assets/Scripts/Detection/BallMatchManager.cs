using System.Collections.Generic;
using UnityEngine;

public class BallMatchManager : MonoBehaviour
{
    public static BallMatchManager Instance;

    [Header("Match Settings")]
    [SerializeField] private int minimumMatch = 3;

    private void Awake()
    {
        Instance = this;
    }

    public void HandleProjectileHit(
        Ball hitBall,
        BallColour projectileColour
    )
    {
        if (hitBall == null)
            return;

        BallChainManager chain =
            BallChainManager.Instance;

        if (chain == null)
        {
            Debug.LogError(
                "BallChainManager does not exist."
            );

            return;
        }

        int hitIndex =
            chain.GetBallIndex(hitBall);

        if (hitIndex == -1)
        {
            Debug.LogError(
                "Hit ball isn't registered in the chain."
            );

            return;
        }

        Debug.Log(
            "Projectile hit "
            + hitBall.Colour
            + " at index "
            + hitIndex
        );

        

        Ball insertedBall =
            chain.InsertBall(
                hitIndex,
                projectileColour
            );

        if (insertedBall == null)
            return;

        // -----------------------------------------------------
        // CHECK MATCH
        // -----------------------------------------------------

        CheckForMatch(
            insertedBall
        );
    }

    private void CheckForMatch(
        Ball insertedBall
    )
    {
        BallChainManager chain =
            BallChainManager.Instance;

        int insertedIndex =
            chain.GetBallIndex(
                insertedBall
            );

        if (insertedIndex == -1)
            return;

        BallColour colour =
            insertedBall.Colour;

        List<Ball> matchingBalls =
            new List<Ball>();

        // Always include the inserted ball.
        matchingBalls.Add(
            insertedBall
        );

        // -----------------------------------------------------
        // SEARCH BACKWARDS
        // -----------------------------------------------------

        int index =
            insertedIndex - 1;

        while (index >= 0)
        {
            Ball ball =
                chain.GetBallAtIndex(index);

            if (ball == null)
                break;

            if (ball.Colour != colour)
                break;

            matchingBalls.Add(ball);

            index--;
        }

        // -----------------------------------------------------
        // SEARCH FORWARDS
        // -----------------------------------------------------

        index =
            insertedIndex + 1;

        while (
            index <
            chain.GetBallCount()
        )
        {
            Ball ball =
                chain.GetBallAtIndex(index);

            if (ball == null)
                break;

            if (ball.Colour != colour)
                break;

            matchingBalls.Add(ball);

            index++;
        }

        Debug.Log(
            "MATCH CHECK: "
            + matchingBalls.Count
            + " "
            + colour
            + " balls."
        );

        // -----------------------------------------------------
        // POP
        // -----------------------------------------------------

        if (
            matchingBalls.Count >=
            minimumMatch
        )
        {
            PopMatch(
                matchingBalls
            );
        }
        else
        {
            Debug.Log(
                "Not enough matching balls."
            );
        }
    }

    private void PopMatch(
        List<Ball> matchingBalls
    )
    {
        Debug.Log(
            "POP! "
            + matchingBalls.Count
            + " "
            + matchingBalls[0].Colour
            + " balls."
        );

        BallChainManager.Instance.RemoveBalls(
            matchingBalls
        );
    }
}