using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]private float speed;

    public void Launch()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        Invoke("Die", 1);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}