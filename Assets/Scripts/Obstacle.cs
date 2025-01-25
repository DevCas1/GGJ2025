using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool CanBePlaced => _canBePlaced;

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

    public void ReceiveDamage()
    {
        _currentHealth--;

        if (_currentHealth <= 0)
        {
            DestroyObstacle();
        }
    }

    private void DestroyObstacle()
    {
        // Destroy effects

        Destroy(this);
    }
}
