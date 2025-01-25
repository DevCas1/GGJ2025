using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _movementForce = 1;
    [SerializeField] private Transform _hamsterTransform;
    [SerializeField] private float _hamsterRotationSpeed = 1;

    private Vector2 _inputVector = Vector2.zero;
    private Quaternion _lookVector;

    private void Awake()
    {
        _lookVector = _hamsterTransform.rotation;
    }

    private void Update()
    {
        UpdateHamsterRotation();
    }

    private void UpdateHamsterRotation()
    {
        if (_inputVector.sqrMagnitude > 0)
        {
            _lookVector = Quaternion.LookRotation(
                new Vector3(_inputVector.x, 0, _inputVector.y),
                Vector3.up
            );
        }

        Vector3 newRotationEuler = Quaternion.Slerp(
            Quaternion.Euler(0, _hamsterTransform.rotation.eulerAngles.y, 0),
            _lookVector,
            Time.deltaTime * _hamsterRotationSpeed
        ).eulerAngles;

        _hamsterTransform.rotation = Quaternion.Euler(0, newRotationEuler.y, 0);
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(new Vector3(_inputVector.x, 0, _inputVector.y) * _movementForce);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 input = movementValue.Get<Vector2>();

        if (input.sqrMagnitude > 1)
            input = input.normalized;

        _inputVector = input;
    }

    // private void OnCollisionEnter(Collision col)
    // {

    // }
}
