using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public int StartingAmount = 1;

    [SerializeField] private Vector2 _spawnSafeZone = new (7.5f, 7.5f);
    [SerializeField] private GameObject[] _spawnableHazards;
    [SerializeField] private float _spawnTimer = 7.5f;

    private readonly int _spawnRotationsOptions = 23;

    private float _currentSpawnTimer;

    private void Start()
    {
        for (int index = 0; index < StartingAmount; index++)
            SpawnHazard();

        _currentSpawnTimer = _spawnTimer;
    }

    private void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;

        if (_currentSpawnTimer <= 0)
            SpawnHazard();
    }

    private void SpawnHazard()
    {
        var newObstacle = Instantiate(_spawnableHazards[Random.Range(0, _spawnableHazards.Length)]);

        newObstacle.transform.SetPositionAndRotation(
            new Vector3(
                Random.Range(-_spawnSafeZone.x, _spawnSafeZone.x), 
                0, 
                Random.Range(-_spawnSafeZone.y, _spawnSafeZone.y)),
            Quaternion.Euler(
                0, 
                15 * Random.Range(0, _spawnRotationsOptions), 
                0)
        );

        _currentSpawnTimer = _spawnTimer;
    }
}
