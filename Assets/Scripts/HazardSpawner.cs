using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public int StartingAmount = 1;

    [SerializeField] private Vector2 _spawnSafeZone = new (7.5f, 7.5f);
    [SerializeField] private GameObject[] _spawnableHazards;
    [SerializeField, Tooltip("The amount of time after a hazard spawned, before the next will spawn")]
    private float _spawnTimer = 7.5f;
    [SerializeField, Tooltip("This is the amount of time before the initial hazard is spawned")]
    private float _initialSpawnTimer = 2.5f;

    private static readonly int _spawnRotationsOptions = 8;
    private static readonly int _spawnRotationIncrements = 360 / _spawnRotationsOptions;

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
            // Set rotation in increments by deviding 360 degrees by the rotation options.
            // Take options = 8, realistically options will be 7, as 1 and 8 are identical:
            // first rotation will be 0, then 360 / 8 = 45 degrees, and 8 * 45 = 360 == 0
            Quaternion.Euler(
                0,
                // Random.Range's max is exclusive, so if _spawnRotationsOptions is 8, it'll be random between 0 and 7
                _spawnRotationIncrements * Random.Range(0, _spawnRotationsOptions),
                0)
        );

        _currentSpawnTimer = _spawnTimer;
    }
}
