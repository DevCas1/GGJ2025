using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    internal class SpawnLocation
    {
        internal int SpawnLocationIndex;
        internal int? AssignedObstacleID => _assignedObstacleID;
        internal bool Available => AssignedObstacleID == null;

        private int? _assignedObstacleID;

        internal void AssignObstacle(int obstacleID) => _assignedObstacleID = obstacleID;
        internal void Clear() => _assignedObstacleID = null;
    }

    public int StartingAmount = 3;

    [SerializeField, Tooltip("This is the amount of time before the initial Obstacle is spawned")]
    private float _initialSpawnDelay = 2.5f;
    [SerializeField, Tooltip("The amount of time after an Obstacle spawned, before the next will spawn")]
    private float _spawnCooldown = 5;
    [SerializeField]
    private Obstacle[] _spawnableObstacles;
    [SerializeField]
    private Transform[] _spawnLocationTransforms;

    private static readonly int _spawnRotationsOptions = 8;
    private static readonly int _spawnRotationIncrements = 360 / _spawnRotationsOptions;

    // Currently unsupported, feature might be available later
    // private SpawnLocation[] _availableSpawnLocations { get; init {_availableSpawnLocations = value;} }
    private SpawnLocation[] _availableSpawnLocations;
    private int _currentSpawnLocationIndex = -1;

    private float _initialSpawnTimer = 0;
    private bool _initialSpawnDone = false;
    private float _spawnTimer = 0;

    private void Start()
    {
        _initialSpawnTimer = _initialSpawnDelay;
        _availableSpawnLocations = new SpawnLocation[_spawnLocationTransforms.Length];

        for (int index = 0; index < _spawnLocationTransforms.Length; index++)
            _availableSpawnLocations[index] = new SpawnLocation { SpawnLocationIndex = index };

        if (_availableSpawnLocations.Length == 0)
            Debug.LogError("_availableSpawnLocations is empty at Start() !");

        SetSpawnLocationIndex();

        if (_initialSpawnDelay > 0)
            return;

        DoInitialSpawn();
    }

    private void DoInitialSpawn()
    {
        for (int index = 0; index < StartingAmount; index++)
            SpawnObstacle();

        _initialSpawnDone = true;
    }

    private void Update()
    {
        if (_initialSpawnDelay > 0 && _initialSpawnDone == false)
        {
            if (_initialSpawnTimer > 0)
            {
                _initialSpawnTimer -= Time.deltaTime;
                return;
            }

            DoInitialSpawn();
            return;
        }

        // Only decrement spawn timer if it's above 0, i.e. there's remaining cooldown
        if (_spawnTimer > 0)
        {
            _spawnTimer -= Time.deltaTime;
            return;
        }

        // Check if _currentSpawnLocationIndex has been properly set, indicating a location is available
        if (_currentSpawnLocationIndex >= 0)
            SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        var newObstacle = Instantiate(_spawnableObstacles[Random.Range(0, _spawnLocationTransforms.Length)]);
        newObstacle.Spawner = this;

        _availableSpawnLocations[_currentSpawnLocationIndex].AssignObstacle(newObstacle.GetInstanceID());

        // Set rotation in increments by deviding 360 degrees by the rotation options.
        // Take options = 8, realistically options will be 7, as 1 and 8 are identical:
        // first rotation will be 0, then 360 / 8 = 45 degrees, and 8 * 45 = 360 == 0
        newObstacle.transform.SetPositionAndRotation(
            _spawnLocationTransforms[_currentSpawnLocationIndex].position,
            Quaternion.Euler(
                0,
                // Random.Range's max is exclusive, so if _spawnRotationsOptions is 8, it'll be random between 0 and 7
                _spawnRotationIncrements * Random.Range(0, _spawnRotationsOptions),
                0
            )
        );

        SetSpawnLocationIndex();
        SetSpawnTimer();
    }

    private void SetSpawnLocationIndex()
    {
        for (int index = 0; index < _availableSpawnLocations.Length; index++)
        {
            // Loop over available spawn locations, and if one is available, set that as next spawn location
            if (_availableSpawnLocations[index].Available == true)
            {
                _currentSpawnLocationIndex = index;
                return;
            }
        }

        _currentSpawnLocationIndex = -1;
    }

    private void SetSpawnTimer()
    {
        _spawnTimer = _spawnCooldown;

        // Handle spawn cooldown indicator
    }

    public void OnObstaclePlaced(Obstacle obstacle)
    {
        foreach (var availableLocation in _availableSpawnLocations)
        {
            if (availableLocation.AssignedObstacleID == obstacle.GetInstanceID())
            {
                availableLocation.Clear();
                SetSpawnLocationIndex();
                return;
            }
        }

        string registeredIDs = string.Empty;

        foreach (var location in _availableSpawnLocations)
            registeredIDs += registeredIDs == string.Empty ? location.AssignedObstacleID : ", " + location.AssignedObstacleID;

        Debug.LogError(
            $"Obstacle {obstacle}'s InstanceID ("+
            obstacle.GetInstanceID() +
            ") could not be found as assigned ID in availableLocation!'" +
            "\n\nRegistered IDs:" + registeredIDs
        );
    }

#if DEBUG
    private void OnDrawGizmosSelected()
    {
        foreach (var spawnLocation in _spawnLocationTransforms)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(spawnLocation.position, Vector3.one);
        }
    }
#endif
}
