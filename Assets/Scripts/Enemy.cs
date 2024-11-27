using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;

    public void Damage(int value)
    {
        hp -= value;
        if (hp <= 0) Destroy(gameObject);
    }
}