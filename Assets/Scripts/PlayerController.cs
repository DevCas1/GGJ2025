using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _movementForce = 1;
    [Header("Damage related")]
    [SerializeField] private int _health = 1;
    [SerializeField] private float _punchKnockbackForce = 1;
    [Tooltip("Knockback for when hamster takes damage")]
    [SerializeField] private float _damageKnockbackForce = 1;
    // [Tooltip("Beware, this value means the square magnitude of velocity the player needs to surpass to be able to punch obstacles.")]
    // [SerializeField] private float _dangerousVelocity = 1;
    [Header("Stamina")]
    [SerializeField] private RectTransform _staminaRectTransform;
    [Tooltip("Stamina in seconds")]
    [SerializeField] private float _maxStamina = 60;
    [SerializeField] private float _staminaBarLerpSpeed = 1;
    [Header("Dash")]
    [SerializeField] private float _dashForce = 1;
    [SerializeField] private float _dashEnergyConsumption;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dangerousDuration = 1;
    [Header("Hamster Visuals")]
    [SerializeField] private Transform _hamsterTransform;
    [SerializeField] private GameObject _hamsterBallTransform;
    [SerializeField] private float _hamsterRotationSpeed = 1;
    [Header("Dust related")]
    [SerializeField] private ParticleSystem _dustParticleSystem;
    [SerializeField] private LayerMask _groundLayer;
    // [SerializeField] private float _maxSpeed;
    // [Tooltip(
    //     "The amount of dust particle emissions over distance when hamster reaches max speed.\n"+
    //     "Result wil interpolate between 0 and max, given current speed."
    // )]
    // [SerializeField] private float _emissionAtMaxSpeed;
    [Header("Unity Events")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    public UnityEvent OnGameOver;

    private readonly float _staminaRectTopMax = 0;
    private readonly float _staminaRectTopMin = -600;

    private bool _isGrounded = false;
    private int _currentHealth;
    private float _currentStamina;
    private float _dashCooldownTimer;
    private float _dangerousTimer;
    private bool _isDangerous = false;
    private Vector2 _inputVector = Vector2.zero;
    private Quaternion _lookVector;

    private void Awake()
    {
        _currentHealth = _health;
        _currentStamina = _maxStamina;
        _lookVector = _hamsterTransform.rotation;
        _staminaRectTransform.anchoredPosition = new Vector2(0, _staminaRectTopMin);
    }

    private void Update()
    {
        UpdateHamsterRotation();

        // if (_rigidbody.linearVelocity.sqrMagnitude > _dangerousVelocity)
        // {
        //     _isDangerous = true;
        //     // Play fire effects
        // }

        UpdateTimers();
        UpdateStaminaVisuals();

        if (_dangerousTimer <= 0)
            DisableDangerous();

        if (_currentStamina <= 0)
            GameOver();
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

    private void UpdateTimers()
    {
        float deltaTime = Time.deltaTime;

        _currentStamina -= deltaTime;

        if (_dashCooldownTimer > 0)
            _dashCooldownTimer -= deltaTime;

        if (_dangerousTimer > 0)
            _dangerousTimer -= deltaTime;
    }

    void UpdateStaminaVisuals()
    {
        _staminaRectTransform.anchoredPosition = new Vector2(
                    0,
                    Mathf.Lerp(
                        _staminaRectTransform.anchoredPosition.y,
                        Mathf.Lerp(
                            _staminaRectTopMin,
                            _staminaRectTopMax,
                            _currentStamina / _maxStamina
                        ),
                        Time.deltaTime * _staminaBarLerpSpeed
                    )
                );
    }

    private void DisableDangerous()
    {
        _isDangerous = false;

        // Stop dangerous effects
        _dustParticleSystem.Stop();
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

    private void OnDash()
    {
        if (_dashCooldownTimer > 0)
            return;

        _dashCooldownTimer = _dashCooldown;
        _dangerousTimer = _dangerousDuration;
        _currentStamina -= _dashEnergyConsumption;
        _isDangerous = true;

        Vector3 dashVector;

        if (_rigidbody.linearVelocity.sqrMagnitude > 0.1f)
            dashVector = _rigidbody.linearVelocity.normalized;
        else
            dashVector = _hamsterTransform.forward.normalized;

        dashVector.y = 0;

        _rigidbody.AddForce(dashVector * _dashForce, ForceMode.VelocityChange);

        // Play dangerous effects
        _dustParticleSystem.Play();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<StaminaPickup>(out var pickup))
        {
            _currentStamina = (_currentStamina + pickup.StaminaRegainAmount) > _maxStamina ?
                _maxStamina :
                _currentStamina + pickup.StaminaRegainAmount;

            pickup.PickedUp();
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (CheckIfLayerInLayerMask(col.gameObject.layer, _groundLayer))
        {
            SetGrounded(true);
            return;
        }

        if (_isDangerous && col.transform.TryGetComponent<Obstacle>(out var obstacle))
        {
            DisableDangerous();
            obstacle.ReceiveDamage(_hamsterTransform.forward);
            _rigidbody.AddForce(-_hamsterTransform.forward * _punchKnockbackForce, ForceMode.VelocityChange);
        }
        else if (_isDangerous && col.transform.TryGetComponent<SharpObstacle>(out var sharpObstacle))
        {
            DisableDangerous();
            ReceiveDamage();
            sharpObstacle.ReceiveDamage(_hamsterTransform.forward);
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (CheckIfLayerInLayerMask(col.gameObject.layer, _groundLayer))
        {
            SetGrounded(false);
            return;
        }
    }

    // Some weird stuff with bits and layers, from https://discussions.unity.com/t/check-if-layer-is-in-layermask/16007/5
    private bool CheckIfLayerInLayerMask(int layer, LayerMask layerMask) => (layerMask & (1 << layer)) != 0;

    private void SetGrounded(bool grounded)
    {
        _isGrounded = grounded;

        // Potential effects for when becoming grounded
    }

    private void ReceiveDamage()
    {
        _currentHealth--;
        OnDamageTaken.Invoke();

        if (_currentHealth <= 0)
        {
            Escape();
            return;
        }

        _rigidbody.AddForce(-_hamsterTransform.forward * _damageKnockbackForce, ForceMode.VelocityChange);
    }

    private void Escape()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * 30;

        Sequence escapeSequence = DOTween.Sequence();
        escapeSequence.Append(_hamsterTransform.DOJump(
            _hamsterTransform.position - (_hamsterTransform.forward * 2),
            2.5f,
            1,
            0.66f
        ));
        // escapeSequence.Append(_hamsterTransform.DOJump(
        //     _hamsterTransform.position - (_hamsterTransform.forward * 3.5f),
        //     1.5f,
        //     1,
        //     0.33f
        // ));
        // escapeSequence.Append(_hamsterTransform.DOJump(
        //     _hamsterTransform.position - (_hamsterTransform.forward * 5),
        //     1,
        //     1,
        //     0.2f
        // ));
        escapeSequence.Append(_hamsterTransform.DOLookAt(randomDirection, 0.2f));
        escapeSequence.Append(_hamsterTransform.DOMove(randomDirection, 3));

        Debug.Log("Hamster escaped!");
        _rigidbody.isKinematic = true;
        _hamsterBallTransform.SetActive(false);
        escapeSequence.Play().OnComplete(() => OnDeath.Invoke());

        // OnDeath.Invoke();
        this.enabled = false;
    }

    private void GameOver()
    {
        Debug.Log("Hamster is worn out!");
        OnGameOver.Invoke();
        this.enabled = false;
    }
}
