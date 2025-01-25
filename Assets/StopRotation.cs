using UnityEngine;

public class StopRotation : MonoBehaviour
{
    void Update() => transform.rotation = Quaternion.identity;
}
