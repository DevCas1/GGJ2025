using DG.Tweening;
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
            Expire();

        _currentDespawnTimer -= Time.deltaTime;
    }

    private void Expire()
    {
        Sequence sequence = DOTween.Sequence();
        // sequence.Append(transform.DOScale(transform.lossyScale * 1.2f, 0.1f));
        // sequence.Append(transform.DOScale(Vector3.zero, 0.25f));
        transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => Destroy(gameObject));
        // TODO: Find better way of cleanup (pooling)
        // sequence.Play().OnComplete(() => Destroy(gameObject));
    }

    public void PickedUp()
    {
        // Destroy effects
        Destroy(gameObject);
    }
}
