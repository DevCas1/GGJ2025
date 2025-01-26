using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Min(0)]
    public int StartingAmount = 0;

    [SerializeField] private Vector2 _spawnSafeZone = new(8.5f, 8.5f);
    [SerializeField] private GameObject[] _spawnablePickups;
    [SerializeField] private float _spawnTimer = 7.5f;

    private float _currentSpawnTimer;

    private void Start()
    {
        for (int index = 0; index < StartingAmount; index++)
            SpawnPickup();

        _currentSpawnTimer = _spawnTimer;
    }

    private void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;

        if (_currentSpawnTimer <= 0)
            SpawnPickup();
    }

    private void SpawnPickup()
    {
        var newObstacle = Instantiate(_spawnablePickups[Random.Range(0, _spawnablePickups.Length)]);

        newObstacle.transform.SetPositionAndRotation(
            new Vector3(
                Random.Range(-_spawnSafeZone.x, _spawnSafeZone.x),
                0,
                Random.Range(-_spawnSafeZone.y, _spawnSafeZone.y)),
            Quaternion.identity
        );

        _currentSpawnTimer = _spawnTimer;
    }
}
