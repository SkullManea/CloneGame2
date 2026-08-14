using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Vector2 direction;

    private bool isPreview;
    private bool hasHit;

    public BallColour Colour { get; private set; }

    public void Initialise(
        Vector2 shootDirection,
        BallColour colour,
        float projectileSpeed
    )
    {
        direction =
            shootDirection.normalized;

        Colour =
            colour;

        speed =
            projectileSpeed;

        hasHit = false;
    }

    public void SetPreview(
        bool preview
    )
    {
        isPreview =
            preview;

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (
            Collider2D collider
            in colliders
        )
        {
            collider.enabled =
                !preview;
        }
    }

    private void Update()
    {
        if (isPreview)
            return;

        transform.Translate(
            direction *
            speed *
            Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (isPreview)
            return;

        if (hasHit)
            return;

        if (!other.CompareTag("Ball"))
            return;

        Ball hitBall =
            other.GetComponent<Ball>();

        if (hitBall == null)
        {
            Debug.LogError(
                "Ball tagged object does not have Ball.cs."
            );

            return;
        }

        hasHit = true;

        Debug.Log(
            "PROJECTILE HIT: "
            + Colour
            + " > "
            + hitBall.Colour
        );

        
        BallMatchManager.Instance
            .HandleProjectileHit(
                hitBall,
                Colour
            );

        
        Destroy(gameObject);
    }
}