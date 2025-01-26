using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public int StartingAmount = 3;

    [SerializeField] private float _spawnTimer = 5;
    [SerializeField] private GameObject[] _spawnableObstacles;
    [SerializeField] private Transform[] _spawnLocations;

    private readonly int _spawnRotationsOptions = 7;

    private int _spawnLocationIndex = 0;
    private float _currentSpawnTimer;

    private void Start()
    {
        for (int index = 0; index < StartingAmount; index++)
        {
            SpawnObstacle();
        }
    }

    private void Update()
    {
        _currentSpawnTimer += Time.deltaTime;

        if (_currentSpawnTimer >= _spawnTimer)
            SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        var newObstacle = Instantiate(_spawnableObstacles[Random.Range(0, _spawnableObstacles.Length - 1)]);

        newObstacle.transform.SetPositionAndRotation(
            _spawnLocations[_spawnLocationIndex].position, 
            Quaternion.Euler(0, 45 * Random.Range(0, _spawnRotationsOptions), 0)
        );

        if (_spawnLocationIndex == _spawnLocations.Length - 1)
            _spawnLocationIndex = 0;
        else
            _spawnLocationIndex++;

        _currentSpawnTimer = 0;
    }
}
