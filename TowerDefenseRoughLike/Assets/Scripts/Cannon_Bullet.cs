using UnityEngine;

public class Cannon_Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private int _bulletSpeed = 20;
    [SerializeField] private int _bulletDamage = 9;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.AddForce(transform.right * _bulletSpeed * 100 * Time.deltaTime);
    }
    

    public int getDamage()
    {
        return _bulletDamage;
    }
}
