using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles moving the player via PlayerInput.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera _boundaryCamera;    // Camera used to calculate the playable area. Can be null.
    [SerializeField] private float _movementSpeed = 6f; // Multiplier for movement speed

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput = Vector2.zero;      // Current movement input from player control
    private Vector2 _boundaryExtents;                   // The half-extents of the origin-centered playable boundary.


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // Use an orthographic camera to determine boundaries, or default if null.
        if (_boundaryCamera != null)
            _boundaryExtents = CalculateCameraBoundary(_boundaryCamera);
        else
            _boundaryExtents = new Vector2(10f, 10f);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// Calcuate and set the new player position when moved via input.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 positionDelta = _movementInput * _movementSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = positionDelta + _rigidbody.position;

        _rigidbody.MovePosition(BoundaryClamp(newPosition));
    }

    /// <summary>
    /// Using an orthographic camera, determine the world-boundary of the screen.
    /// </summary>
    /// <remarks>
    /// This does not consider the position of the camera, extents will be centered at
    /// the world origin.
    /// </remarks>
    /// <param name="cam">An orthographic camera used to determine the boundary</param>
    /// <returns>2D World-space boundaries for the player</returns>
    private Vector2 CalculateCameraBoundary(Camera cam)
    {
        // Camera's orthographic size is distance from the center to top boundary.
        // Multiply that by the aspect ratio to get the horizontal distance.

        Vector2 extents = new Vector2();
        extents.x = cam.orthographicSize * cam.aspect;
        extents.y = cam.orthographicSize;

        return extents;
    }

    /// <summary>
    /// Clamps position using the boundaryExtents as a symmetric 2D bound.
    /// </summary>
    /// <param name="position">Position to clamp</param>
    /// <returns>Clamped Position</returns>
    private Vector2 BoundaryClamp(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, -_boundaryExtents.x, _boundaryExtents.x),
            Mathf.Clamp(position.y, -_boundaryExtents.y, _boundaryExtents.y)
        );
    }

    // Input handlers
    private void OnMove(InputValue value) => _movementInput = value.Get<Vector2>();
}
