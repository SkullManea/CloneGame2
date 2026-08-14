using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{
    [Header("Shooter References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform muzzlePoint;

    [Header("Projectile Prefabs")]
    [Tooltip("Assign your Red, Blue, Green and Yellow projectile prefabs here.")]
    [SerializeField] private GameObject[] projectilePrefabs;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float shootCooldown = 0.25f;

    private float nextShootTime;

    private BallColour currentColour;
    private BallColour nextColour;

    private Camera mainCamera;

    // The projectile currently sitting at the muzzle
    private GameObject previewProjectile;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Choose the first projectile colour
        currentColour = GetRandomColour();

        // Choose the projectile that will come after it
        nextColour = GetRandomColour();

        // Display the current projectile at the muzzle
        CreatePreviewProjectile();
    }

    private void Update()
    {
        AimAtPointer();

        if (Pointer.current != null &&
            Pointer.current.press.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void AimAtPointer()
    {
        if (Pointer.current == null || mainCamera == null)
            return;

        Vector2 pointerPosition =
            Pointer.current.position.ReadValue();

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(pointerPosition);

        // Keep the shooter on the 2D plane
        worldPosition.z = pivot.position.z;

        Vector2 direction =
            worldPosition - pivot.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        pivot.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    private void Shoot()
    {
        // Prevent shooting during cooldown
        if (Time.time < nextShootTime)
            return;

        nextShootTime =
            Time.time + shootCooldown;

        // Safety check
        if (previewProjectile == null)
        {
            CreatePreviewProjectile();
        }

        if (previewProjectile == null)
            return;

        Projectile projectile =
            previewProjectile.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "The projectile prefab does not contain a Projectile component."
            );

            Destroy(previewProjectile);
            previewProjectile = null;

            return;
        }

        // IMPORTANT:
        // Store the direction BEFORE unparenting/rotating anything.
        Vector2 shootDirection =
            muzzlePoint.right;

        // IMPORTANT:
        // Remove the projectile from the muzzle hierarchy.
        // This prevents it from following the muzzle after being fired.
        previewProjectile.transform.SetParent(null);

        // Initialise the projectile
        projectile.Initialise(
            shootDirection,
            currentColour,
            projectileSpeed
        );

        // Tell the projectile that it is no longer a preview
        projectile.SetPreview(false);

        // The projectile is now flying independently
        previewProjectile = null;

        // Move the next projectile into the current position
        currentColour = nextColour;

        // Generate a new next projectile colour
        nextColour = GetRandomColour();

        // Display the new projectile at the muzzle
        CreatePreviewProjectile();

        Debug.Log(
            "Shot: " + currentColour +
            " | Next: " + nextColour
        );
    }

    private void CreatePreviewProjectile()
    {
        // Remove an existing preview if there is one
        if (previewProjectile != null)
        {
            Destroy(previewProjectile);
            previewProjectile = null;
        }

        // Find the correct prefab based on the current colour
        GameObject prefab =
            GetProjectilePrefab(currentColour);

        if (prefab == null)
        {
            Debug.LogError(
                "No projectile prefab was found for colour: "
                + currentColour
            );

            return;
        }

        // Create the projectile at the muzzle
        previewProjectile =
            Instantiate(
                prefab,
                muzzlePoint.position,
                muzzlePoint.rotation
            );

        // Get the Projectile component
        Projectile projectile =
            previewProjectile.GetComponent<Projectile>();

        if (projectile != null)
        {
            // Tell it that it is currently only a preview
            projectile.SetPreview(true);
        }
        else
        {
            Debug.LogError(
                "Projectile prefab '" +
                prefab.name +
                "' does not have a Projectile component."
            );
        }

        // Parent the preview to the muzzle
        // so it follows the shooter while aiming
        previewProjectile.transform.SetParent(muzzlePoint);

        // Keep it directly on the muzzle
        previewProjectile.transform.localPosition =
            Vector3.zero;

        previewProjectile.transform.localRotation =
            Quaternion.identity;
    }

    private GameObject GetProjectilePrefab(BallColour colour)
    {
        
        string requiredTag =
            colour.ToString().ToLower();

        foreach (GameObject prefab in projectilePrefabs)
        {
            if (prefab == null)
                continue;

            if (prefab.CompareTag(requiredTag))
            {
                return prefab;
            }
        }

        Debug.LogError(
            "Could not find a projectile prefab with the tag: "
            + requiredTag
        );

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
}