using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    public float StaminaRegainAmount = 30;

    public void PickedUp()
    {
        Destroy(gameObject);
    }
}
