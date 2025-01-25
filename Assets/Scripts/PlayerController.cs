using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _movementForce = 1;
    [Header("Damage related")]
    [SerializeField] private int _health = 1;
    [SerializeField] private float _damageKnockbackForce = 1;
    [Header("Hamster visuals")]
    [SerializeField] private Transform _hamsterTransform;
    [SerializeField] private float _hamsterRotationSpeed = 1;
    [Header("Unity Events")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;

    private int _currentHealth;
    private Vector2 _inputVector = Vector2.zero;
    private Quaternion _lookVector;

    private void Awake()
    {
        _currentHealth = _health;
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
        if (_inputVector.sqrMagnitude > 0)
        {
            _rigidbody.AddForce(
                new Vector3(_inputVector.x, 0, _inputVector.y) * _movementForce, 
                ForceMode.Acceleration
            );
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 input = movementValue.Get<Vector2>();

        if (input.sqrMagnitude > 1)
            input = input.normalized;

        _inputVector = input;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.TryGetComponent<SharpObstacle>(out SharpObstacle sharpObstacle))
        {
            ReceiveDamage();
        }
    }

    private void ReceiveDamage()
    {
        _currentHealth--;
        Debug.Log("Receiving Damage! Current health: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            Death();
            return;
        }

        _rigidbody.AddForce(-_hamsterTransform.forward * _damageKnockbackForce, ForceMode.VelocityChange);
    }

    private void Death()
    {
        Debug.Log("Death");
        OnDeath.Invoke();
        this.enabled = false;
    }
}
