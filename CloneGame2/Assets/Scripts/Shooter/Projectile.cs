using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Vector2 direction;

    public BallColour Colour { get; private set; }
    public void Initialise(Vector2 shootDirection, BallColour colour)
    {
        direction = shootDirection.normalized;
        Colour = colour;
    }

    private void Update()
    {
        transform.Translate(
            direction * speed * Time.deltaTime,
            Space.World
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log(
                "Projectile hit a ball. Colour: "
                + Colour
            );

            Destroy(gameObject);
        }
    }
}