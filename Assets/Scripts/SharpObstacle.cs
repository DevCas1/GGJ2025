using DG.Tweening;
using UnityEngine;

public class SharpObstacle : MonoBehaviour 
{
    [SerializeField] private int _health = 1;

    private int _currentHealth;

    private void Start()
    {
        Collider collider = GetComponentInChildren<Collider>(true);

        if (collider == null)
            Debug.LogError($"Obstacle \"{transform.name}\" nor any of it's children has a collider, obstacle will therefore not trigger!");

        _currentHealth = _health;
    }

    public void ReceiveDamage(Vector3 damageDirection)
    {
        _currentHealth--;

        if (_currentHealth <= 0)
        {
            DestroyObstacle(damageDirection);
        }
    }

    private void DestroyObstacle(Vector3 damageDirection)
    {
        // Destroy effects
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(transform.lossyScale * 1.2f, 0.1f));
        sequence.Append(transform.DOScale(Vector3.zero, 0.25f));

        transform.DOPunchPosition(damageDirection.normalized * 0.5f, 0.5f, 0, 0);
        // TODO: Find better way of cleanup (pooling)
        sequence.Play().OnComplete(() => Destroy(this));
    }
}
