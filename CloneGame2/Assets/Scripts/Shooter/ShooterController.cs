using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{
    [Header("Shooter References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform muzzlePoint;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        AimAtPointer();
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
}