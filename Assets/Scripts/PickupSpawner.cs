using DG.Tweening;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Min(0)]
    public int StartingAmount = 0;

    [SerializeField] private Vector2 _spawnSafeZone = new(8.5f, 8.5f);
    [SerializeField] private GameObject[] _spawnablePickups;
    [SerializeField, Tooltip("This is the amount of time before the initial pickup is spawned"), Min(0)]
    private float _initialSpawnDelay = 2.5f;
    [SerializeField, Tooltip("The amount of time after a pickup spawned, before the next will spawn"), Min(float.Epsilon)]
    private float _spawnDelay = 7.5f;

    private float _initialSpawnTimer;
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
            SpawnPickup();

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
            SpawnPickup();
    }

    private void SpawnPickup()
    {
        GameObject newPickup = Instantiate(_spawnablePickups[Random.Range(0, _spawnablePickups.Length)]);
        Transform newPickupTransform = newPickup.transform;
        Vector3 originalScale = newPickupTransform.lossyScale;

        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(newPickupTransform.DOScale(originalScale * 1.2f, 0.25f));
        scaleSequence.Append(newPickupTransform.DOScale(originalScale, 0.1f));

        newPickup.transform.SetPositionAndRotation(
            new Vector3(
                Random.Range(-_spawnSafeZone.x, _spawnSafeZone.x),
                0,
                Random.Range(-_spawnSafeZone.y, _spawnSafeZone.y)),
            Quaternion.identity
        );

        scaleSequence.Play();

        _spawnTimer = _spawnDelay;
    }

#if DEBUG
    // Draw gizmos in Editor to indicate area where Pickups can spawn inside of
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.75f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(_spawnSafeZone.x * 2, 0.01f, _spawnSafeZone.y * 2));
    }
#endif
}
