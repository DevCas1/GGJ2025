using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _movementForce = 1;
    [Header("Damage related")]
    [SerializeField] private int _health = 1;
    [SerializeField] private float _damageKnockbackForce = 1;
    [Tooltip("Beware, this value means the square magnitude of velocity the player needs to surpass to be dangerous.")]
    [SerializeField] private float _dangerousVelocity = 1;
    [Header("Hamster visuals")]
    [SerializeField] private Transform _hamsterTransform;
    [SerializeField] private GameObject _hamsterBallTransform;
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
        if (
            _rigidbody.linearVelocity.sqrMagnitude > _dangerousVelocity && 
            col.transform.TryGetComponent<Obstacle>(out var obstacle)
        )
        {
            obstacle.ReceiveDamage();
        }

        if (col.transform.TryGetComponent<SharpObstacle>(out _))
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
            Escape();
            return;
        }

        _rigidbody.AddForce(-_hamsterTransform.forward * _damageKnockbackForce, ForceMode.VelocityChange);
    }

    private void Escape()
    {
        Vector3 randomDirection = new Vector3(Random.value, 0, Random.value).normalized * 100;

        Sequence escapeSequence = DOTween.Sequence();
        escapeSequence.Append(_hamsterTransform.DOJump(_hamsterTransform.position, 1, 1, 1));
        escapeSequence.Append(_hamsterTransform.DOLookAt(randomDirection, 0.2f));
        escapeSequence.Append(_hamsterTransform.DOMove(randomDirection, 10));

        Debug.Log("Hamster escaped!");
        _rigidbody.isKinematic = true;
        _hamsterBallTransform.SetActive(false);
        escapeSequence.Play();

        OnDeath.Invoke();
        this.enabled = false;
    }
}
