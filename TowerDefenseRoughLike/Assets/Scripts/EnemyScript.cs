using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    private float _speed;
    private int _maxHp = 20;
    [SerializeField] private int _hp = 20;
    [SerializeField] private int _damage = 5;
    [SerializeField] private Slider _hpBar;
    private void Awake()
    {
        _hp = _maxHp;
    }

    private void Update()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        bool onCamera = viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0;

        if (!onCamera)
        {
            GameManager.damageHP(_damage);
            Destroy(gameObject);
        }


        _hpBar.gameObject.SetActive(_hp < _maxHp);
        _hpBar.maxValue = _maxHp;
        _hpBar.value = _hp;
        transform.position -= new Vector3(0, _speed * Time.deltaTime);
        if (_hp <= 0)
        {
            Destroy(gameObject);
            GameManager.GetCoin(20);
        }
    }
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    public void SetHp(int hp)
    {
        _maxHp = hp;
        _hp = _maxHp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            _hp -= collision.GetComponent<Cannon_Bullet>().getDamage();
            Destroy(collision.gameObject);
            
        }
    }
}
