using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 Speed = Vector3.one;

    void Update() => transform.Rotate(Speed * Time.deltaTime);
}
