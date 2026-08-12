using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{
    [Header("Shooter References")]

    [SerializeField]
    private Transform pivot;
    [SerializeField] 
    private Transform muzzlePoint;

    [Header("Projectile")]

    [SerializeField] 
    private GameObject projectilePrefab;
    [SerializeField] 
    private float projectileSpeed = 10f;
    [SerializeField] 
    private float shootCooldown = 0.25f;
    private float nextShootTime;

    private BallColour currentColour;
    private BallColour nextColour;

    private Camera mainCamera;

    private void Start()
    {
        currentColour = GetRandomColour();
        nextColour = GetRandomColour();
    }
    private void Awake()
    {
        mainCamera = Camera.main;
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
        if (Pointer.current == null)
            return;

        Vector2 pointerPosition =
            Pointer.current.position.ReadValue();

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(pointerPosition);

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

        if (Time.time < nextShootTime)
        {
            return;
        }

        nextShootTime =
            Time.time + shootCooldown;

        GameObject projectileObject =
            Instantiate(
                projectilePrefab,
                muzzlePoint.position,
                muzzlePoint.rotation
            );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.Initialise(
            muzzlePoint.right,
            currentColour
        );

        currentColour = nextColour;
        nextColour = GetRandomColour();

        Debug.Log(
            "Current: " + currentColour +
            " | Next: " + nextColour
);
    }

    private BallColour GetRandomColour()
    {
        int numberOfColours =
            System.Enum.GetValues(typeof(BallColour)).Length;

        return (BallColour)Random.Range(
            0,
            numberOfColours
        );
    }
}