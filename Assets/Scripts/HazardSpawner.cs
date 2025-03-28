using DG.Tweening;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public int StartingAmount = 1;

    [SerializeField] private Vector2 _spawnSafeZone = new (7.5f, 7.5f);
    [SerializeField] private GameObject[] _spawnableHazards;
    [SerializeField, Tooltip("This is the amount of time before the initial hazard is spawned"), Min(0)]
    private float _initialSpawnDelay = 2.5f;
    [SerializeField, Tooltip("The amount of time after a hazard spawned, before the next will spawn"), Min(float.Epsilon)]
    private float _spawnDelay = 7.5f;

    private static readonly int _spawnRotationsOptions = 8;
    private static readonly int _spawnRotationIncrements = 360 / _spawnRotationsOptions;

    private float _initialSpawnTimer = 0;
    private bool _initialSpawnDone = false;
    private float _spawnTimer;

    private void Start()
    {
        _initialSpawnTimer = _initialSpawnDelay;

        if (_initialSpawnDelay > 0)
            return;

        if (StartingAmount > 0)
            DoInitialSpawn();
    }

    private void DoInitialSpawn()
    {
        for (int index = 0; index < StartingAmount; index++)
            SpawnHazard();

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
        }

        if (_spawnTimer > 0)
            _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
            SpawnHazard();
    }

    private void SpawnHazard()
    {
        GameObject newObstacle = Instantiate(_spawnableHazards[Random.Range(0, _spawnableHazards.Length)]);
        Transform newObstacleTransform = newObstacle.transform;
        Vector3 originalScale = newObstacleTransform.lossyScale;

        newObstacleTransform.localScale = Vector3.zero;

        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(newObstacleTransform.DOScale(originalScale * 1.2f, 0.25f));
        scaleSequence.Append(newObstacleTransform.DOScale(originalScale, 0.1f));

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
        scaleSequence.Play();

        _spawnTimer = _spawnDelay;
    }

#if DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.75f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(_spawnSafeZone.x * 2, 0.01f, _spawnSafeZone.y * 2));
    }
#endif
}
