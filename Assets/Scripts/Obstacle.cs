using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        Collider collider = GetComponentInChildren<Collider>(true);

        if (collider == null)
            Debug.LogError($"Obstacle \"{transform.name}\" nor any of it's children has a collider, obstacle will therefore not trigger!");
    }
}
