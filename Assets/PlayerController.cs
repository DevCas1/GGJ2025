using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _movementForce = 1;
    [SerializeField]
    private float _maxMoveVelocity;

    private Vector2 _movementVector;

    private void Start()
    {
        Debug.Log(
            $"One directional input vs full input: {new Vector2(1, 0)} vs {new Vector2(1, 1)}\n" +
            $"sqrMagnitude of both: {new Vector2(1, 0).sqrMagnitude} vs {new Vector2(1, 1).sqrMagnitude}\n" +
            $"magnitude of both: {new Vector2(1, 0).magnitude} vs {new Vector2(1, 1).magnitude}"
        );
    }

    private void FixedUpdate()
    {
        // Vector3 currentLinearVelocity = _rigidbody.linearVelocity;
        // Vector3 horizontalVelocity = new (currentLinearVelocity.x, 0, currentLinearVelocity.z);
        // Vector3 normalizedHorizontalVelocity = horizontalVelocity.normalized;
        // float currentSqrMagnitude = horizontalVelocity.sqrMagnitude;

        // if (currentSqrMagnitude > _maxMoveVelocity)
        // {
        //     _rigidbody.linearVelocity = new Vector3(
        //         normalizedHorizontalVelocity.x * _maxMoveVelocity,
        //         currentLinearVelocity.y,
        //         normalizedHorizontalVelocity.z * _maxMoveVelocity
        //     );
        // }

        _rigidbody.AddForce(new Vector3(_movementVector.x, 0, _movementVector.y) * _movementForce);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 input = movementValue.Get<Vector2>();

        if (input.sqrMagnitude > 1)
            input = input.normalized;

        _movementVector = input;
    }
}
