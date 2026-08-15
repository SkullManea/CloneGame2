using UnityEngine;

public class Spawning : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 1.3f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            Spawn();
        }
    }

    private void Spawn()
    {
        BallColour colour =
            GetRandomColour();

        BallChainManager.Instance.AddBallAtEnd(
            colour
        );
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
}