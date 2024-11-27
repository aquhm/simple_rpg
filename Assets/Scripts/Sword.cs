using UnityEngine;

public class Sword : MonoBehaviour
{
    private bool collided;

    private void OnCollisionEnter(Collision other)
    {
        if (collided) return;

        other.gameObject.SendMessage("Damage", 5, SendMessageOptions.DontRequireReceiver);
    }
}