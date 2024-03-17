using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject _Turret;
    [SerializeField] private float _cooldown;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Slider _cooldownSlider; 
    private Queue<EnemyScript> _enemies = new Queue<EnemyScript>();
    private Transform _cannonFocus;


    private bool _inControl = true;
    private Animator _anim;
    private float _cooldownTimer;

    #region upgrade section

    [SerializeField] private GameObject _upgradeButton;
    private bool _upgradeButtonActive;

    #endregion
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _upgradeButtonActive = false;
    }
    void Update()
    {
        if (_enemies.Count > 0)
            _cannonFocus = _enemies.Peek().transform;
        
        

        _cooldownSlider.gameObject.SetActive(!GameManager.onBuild);


        if(_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        _cooldownSlider.value = 2 - _cooldownTimer;
       

        #region Turret Point to Enemy and shoot

        if (_cannonFocus != null && _inControl)
        {
            
            Vector3 enemyPos = _cannonFocus.position;
            enemyPos.x = enemyPos.x - _Turret.transform.position.x;
            enemyPos.y = enemyPos.y - _Turret.transform.position.y;
            float angle = Mathf.Atan2(enemyPos.y, enemyPos.x) * Mathf.Rad2Deg;

           
            _Turret.transform.rotation =  Quaternion.Euler(new Vector3(0, 0, angle));

            #region Shooting
            if (_cooldownTimer <= 0)
            {
                Shoot();
            }
            #endregion          
            
        }

        
        #endregion

        _anim.SetBool("active", _inControl && !GameManager.onBuild);

        var currentPos = transform.position;
        transform.position = new Vector3(Mathf.Round(currentPos.x),Mathf.Round(currentPos.y),Mathf.Round(currentPos.z));
         

    }

    private void Shoot()
    {
        Instantiate(_bullet, _firePoint.position, _Turret.transform.rotation);
        _cooldownTimer = _cooldown;
    }

    public void SetControl(bool b)
    {
        _inControl = b;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            _enemies.Enqueue(collision.GetComponent<EnemyScript>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            if(_enemies.Count>0)
            _enemies.Dequeue();
        }
    }

    public void enableButton()
    {
        _upgradeButton.GetComponent<Animator>().SetBool("enable", !_upgradeButtonActive);
        _upgradeButtonActive = !_upgradeButtonActive;
    }
}
