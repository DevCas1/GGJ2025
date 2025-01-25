using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField]
    private Transform _objectToFollow;
    [SerializeField]
    private bool _smoothFollow;
    [SerializeField]
    private float _smoothTime = 0.3f;
    // [SerializeField]
    // private float _followSpeed = 1;
    [SerializeField]
    private bool _constrainX;
    [SerializeField]
    private bool _constrainY;
    [SerializeField]
    private bool _constrainZ;

    private Vector3 _initialPosition;
    private Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 objectPosition = _objectToFollow.position;

        Vector3 targetPosition = new (
            _constrainX ? _initialPosition.x : objectPosition.x,
            _constrainY ? _initialPosition.y : objectPosition.y,
            _constrainZ ? _initialPosition.z : objectPosition.z
        );

        if (_smoothFollow)
        {
            // transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, Time.deltaTime * _followSpeed);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
        }
        else
            transform.position = targetPosition;
    }
}
