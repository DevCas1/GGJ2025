using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        Collider collider = GetComponent<Collider>();

        if (collider == null)
            Debug.LogError($"Obstacle {transform.name} has no collider, and will therefore not trigger!");
    }
}
