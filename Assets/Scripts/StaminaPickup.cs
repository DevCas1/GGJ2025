using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    public float StaminaRegainAmount = 30;
    public float DespawnTime = 5;

    private float _currentDespawnTimer;

    private void Start() => _currentDespawnTimer = DespawnTime;

    private void Update()
    {
        if (_currentDespawnTimer <= 0)
            Destroy(gameObject);

        _currentDespawnTimer -= Time.deltaTime;
    }

    public void PickedUp()
    {
        Destroy(gameObject);
    }
}
