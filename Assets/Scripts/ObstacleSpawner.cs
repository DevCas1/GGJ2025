using System.Linq;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public int StartingAmount = 3;

    [SerializeField] private float _spawnCooldown = 5;
    [SerializeField] private GameObject[] _spawnableObstacles;
    [SerializeField] private Transform[] _spawnLocations;

    private static readonly int _spawnRotationsOptions = 8;
    private static readonly int _spawnRotationIncrements = 360 / _spawnRotationsOptions;

    private bool _spawnLocationAvailable;
    private bool[] _availableSpawnLocations;

    private int _currentSpawnLocationIndex = 0;
    private float _spawnTimer;

    private void Start()
    {
        _availableSpawnLocations = Enumerable.Repeat(true, _spawnLocations.Length).ToArray();
        _spawnLocationAvailable = true;

        for (int index = 0; index < StartingAmount; index++)
            SpawnObstacle();

        _spawnTimer = _spawnCooldown;
    }

    private void Update()
    {
        // Only decrement spawn timer if it's above 0, i.e. there's remaining cooldown
        if (_spawnTimer > 0)
            _spawnTimer -= Time.deltaTime;

        // If spawn timer is 0 or less, and there's a spawn location available, spawn a new obstacle
        if (_spawnTimer <= 0 && _spawnLocationAvailable)
            SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        var newObstacle = Instantiate(_spawnableObstacles[Random.Range(0, _spawnLocations.Length)]);

        // Set rotation in increments by deviding 360 degrees by the rotation options.
        // Take options = 8, realistically options will be 7, as 1 and 8 are identical:
        // first rotation will be 0, then 360 / 8 = 45 degrees, and 8 * 45 = 360 == 0
        newObstacle.transform.SetPositionAndRotation(
            _spawnLocations[_currentSpawnLocationIndex].position,
            Quaternion.Euler(
                0,
                // Random.Range's max is exclusive, so if _spawnRotationsOptions is 8, it'll be random between 0 and 7
                _spawnRotationIncrements * Random.Range(0, _spawnRotationsOptions),
                0
            )
        );

        SetSpawnLocationIndex();
    }

    private void SetSpawnLocationIndex()
    {
        // Make current spawn location unavailable, as SetSpawnLocationIndex is only called after spawning obstacles
        _availableSpawnLocations[_currentSpawnLocationIndex] = false;

        for (int index = 0; index < _spawnLocations.Length; index++)
        {
            // Loop over available spawn locations, and if one is available, set that as next spawn location
            if (_availableSpawnLocations[index] == true)
            {
                _currentSpawnLocationIndex = index;
                _spawnLocationAvailable = true;
                SetSpawnTimer();
                return;
            }
        }

        _spawnLocationAvailable = false;
    }

    private void SetSpawnTimer()
    {
        _spawnTimer = _spawnCooldown;
    }

    public void OnObstaclePlaced(Obstacle obstacle)
    {

    }
}
