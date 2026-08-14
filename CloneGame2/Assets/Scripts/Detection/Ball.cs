using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Colour")]
    [SerializeField] private BallColour colour;

    [Header("Pop Animation")]
    [SerializeField] private float popDuration = 0.18f;
    [SerializeField] private float popScale = 1.35f;

    public BallColour Colour => colour;

    private bool isPopping;

    public void Pop()
    {
        if (isPopping)
            return;

        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        isPopping = true;

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        SpriteRenderer[] renderers =
            GetComponentsInChildren<SpriteRenderer>();

        Vector3 originalScale =
            transform.localScale;

        Vector3 enlargedScale =
            originalScale * popScale;

        float timer = 0f;

        // -------------------------------------------------
        // PHASE 1: QUICKLY EXPAND
        // -------------------------------------------------

        float expandDuration =
            popDuration * 0.35f;

        while (timer < expandDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / expandDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale =
                Vector3.Lerp(
                    originalScale,
                    enlargedScale,
                    t
                );

            yield return null;
        }

        // -------------------------------------------------
        // PHASE 2: SHRINK + FADE
        // -------------------------------------------------

        timer = 0f;

        float shrinkDuration =
            popDuration * 0.65f;

        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / shrinkDuration;

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale =
                Vector3.Lerp(
                    enlargedScale,
                    Vector3.zero,
                    t
                );

            foreach (
                SpriteRenderer renderer
                in renderers
            )
            {
                Color color =
                    renderer.color;

                color.a =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t
                    );

                renderer.color =
                    color;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}