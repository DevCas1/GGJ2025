using UnityEngine;
using UnityEngine.InputSystem;

public class HamsterAnimator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1;

    private Vector2 _inputVector;

    // Update is called once per frame
    void Update()
    {
        if (_inputVector.sqrMagnitude == 0)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_inputVector.x, 0, _inputVector.y));

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * _rotationSpeed
        );
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 input = movementValue.Get<Vector2>();

        if (input.sqrMagnitude > 0)
            _inputVector = input;
    }
}
