using DG.Tweening;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool CanBePlaced => _canBePlaced;
    public ObstacleSpawner Spawner;

    [SerializeField] private int _health = 1;

    private int _currentHealth;
    private bool _canBePlaced = true;

    private void Start()
    {
        Collider collider = GetComponentInChildren<Collider>(true);

        if (collider == null)
            Debug.LogError($"Obstacle \"{transform.name}\" nor any of it's children has a collider, obstacle will therefore not trigger!");
        
        _currentHealth = _health;
    }

    public void OnObstaclePlaced()
    {
        Spawner.OnObstaclePlaced(this);
    }

    private void OnTriggerEnter(Collider col)
    {
        _canBePlaced = false;
        Debug.Log("Dragged Obstacle \"" + name + " entered other object \"" + col.transform.name + "\" and can't be placed here!");
        // if (col.TryGetComponent<Obstacle>(out var other))
        // {

        // }
    }

    private void OnTriggerExit(Collider col)
    {
        _canBePlaced = true;
        Debug.Log("Dragged Obstacle \"" + name + " exited other object \"" + col.transform.name + "\" and can now be placed!");
    }

    public void ReceiveDamage(Vector3 damageDirection)
    {
        Debug.Log($"Obstacle \"{name}\" received damage");
        _currentHealth--;

        if (_currentHealth <= 0)
        {
            Debug.Log("And got destroyed");
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
        sequence.Play().OnComplete(() => DestroyImmediate(this));

        Destroy(this);
    }
}
