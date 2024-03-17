using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using TMPro;
public class GameManager : MonoBehaviour
{
    [Header("Cannons")]
    [SerializeField] private GameObject _cannonObject;
    [SerializeField] private Button _cannonButton;
    [SerializeField] private TMP_Text _cannonCost;
    [Header("TileMap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase green;
    [SerializeField] private TileBase red;
    [SerializeField] private TilemapRenderer rend;
    [Header("Scores")]
    [SerializeField] private Slider _hitpointsSlider;
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private static int _hitpoints = 100;
    [SerializeField] private static int _coin = 0;
    [Header("Enemy Manager")]
    [SerializeField] private Transform _enemySpawnPoint;
    [SerializeField] private GameObject _normalEnemy;
    [SerializeField] private TMP_Text _waveCounterText;
    [SerializeField] private Slider _waveCooldown;
    [SerializeField] private int _normalEnemyCooldown1 = 1;
    [SerializeField] private int _normalEnemyCooldown2 = 3;
    [SerializeField] private int _normalEnemyCooldown3 = 7;
    [SerializeField] private int _simpleCannonCost;
    [SerializeField] private int _startingCoin = 30;


    private float _normalEnemyTimer1;
    private float _normalEnemyTimer2;
    private float _normalEnemyTimer3;
    public static bool onBuild = false;
    private GameObject _OnBuildingObject;
    private List<GameObject> _cannons = new List<GameObject>();
    private Vector3 _mousePos;
    private bool _canBuild,_onBuildZone;
    private int _cannonInControl = 0;
    private float distanceBetweenBuildings = 0.1f;


    private float _enemyWaveSpeed = 0.5f;
    private int _enemyWaveHp = 20;
    private int _enemyNumber;
    private int _enemySetNumber;
    private int _enemyWaveNumber = 4;
    private float _enemySpeedIncreaseRate = 0.25f;
    private int _enemyHpIncreaseRate = 9;
    private int _waveCounter = 0;
    private int _cannonCounter = 0;

    private void Start()
    {
        _coin = _startingCoin;
        
        _enemySetNumber = Random.Range(1, 2);
        _normalEnemyTimer3 = _normalEnemyCooldown3;
    }
    void Update()
    {
        #region Build And Control Cannons
        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos = new Vector3(m.x, m.y, 0);

        rend.enabled = onBuild;

        if (onBuild)
        {
            bool hitOtherBuilding = false;
            
            for (int i = 0;i < _cannons.Count; i++)
            {
                if(Vector2.Distance(_OnBuildingObject.transform.position, _cannons[i].transform.position) < distanceBetweenBuildings)
                {
                    hitOtherBuilding = true;
                }
            }

            _canBuild = !hitOtherBuilding && !EventSystem.current.IsPointerOverGameObject() && _onBuildZone && _coin >= _simpleCannonCost;
     

            _OnBuildingObject.transform.position = _mousePos;
            var currentPos = _OnBuildingObject.transform.position;
            _OnBuildingObject.transform.position = new Vector3(Mathf.Round(currentPos.x),Mathf.Round(currentPos.y),Mathf.Round(currentPos.z));

            Vector3 pos = new Vector3(_OnBuildingObject.transform.position.x - 1, _OnBuildingObject.transform.position.y-1,0);
            Vector3Int cell = tilemap.WorldToCell(pos);
            TileBase tile = tilemap.GetTile(cell);
            _onBuildZone = (tile == green);



            _OnBuildingObject.GetComponent<Cannon>().SetControl(false);

            if (Input.GetMouseButtonDown(0) && _canBuild)
            {
                _OnBuildingObject.GetComponent<Cannon>().SetControl(true);
                _cannons.Add(Instantiate(_OnBuildingObject, _mousePos, Quaternion.identity));
                _OnBuildingObject.GetComponent<Cannon>().SetControl(false);
                _cannonCounter++;
                _coin -= _simpleCannonCost;
                Vector3 npos = new Vector3(pos.x, pos.y + 1, 0);
                Vector3Int ncell = tilemap.WorldToCell(npos);
                tilemap.SetTile(ncell, red);

                npos = new Vector3(pos.x, pos.y - 1, 0);
                ncell = tilemap.WorldToCell(npos);
                tilemap.SetTile(ncell, red);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(_OnBuildingObject);
                onBuild = false;
            }
        }
        /*
        for (int i = 0; i < _cannons.Count; i++)
        {
            if (i != _cannonInControl)
                _cannons[i].GetComponent<Cannon>().SetControl(false);
            else
                _cannons[i].GetComponent<Cannon>().SetControl(true);
        }
        */
        #endregion


        _hitpointsSlider.value = _hitpoints;
        _coinText.text = _coin.ToString();
        _simpleCannonCost = 70 * _cannonCounter;
        _cannonCost.text = _simpleCannonCost.ToString();
        _cannonButton.image.color = (_coin >= _simpleCannonCost) ? new Color(119f / 255f, 1f, 98f / 255f) : new Color(1, 143f / 255f, 138f / 255f);


        #region Enemy Spawn
        _waveCooldown.value = _normalEnemyTimer3;
        _waveCounterText.text = _waveCounter.ToString();

        if (_normalEnemyTimer3 > 0)
            _normalEnemyTimer3 -= Time.deltaTime;
        if (_normalEnemyTimer2 > 0)
            _normalEnemyTimer2 -= Time.deltaTime;
        if (_normalEnemyTimer1 > 0)
            _normalEnemyTimer1 -= Time.deltaTime;

        if (_enemySetNumber <= 0 && _enemyWaveNumber > 0)
        {
            _enemySetNumber = Random.Range(4,6);

            _enemyWaveSpeed += _enemySpeedIncreaseRate;
            _enemyWaveHp += _enemyHpIncreaseRate;

            _enemyWaveNumber--;
            _waveCounter++;

            _normalEnemyTimer3 = _normalEnemyCooldown3;
        }

        if (_enemyNumber <= 0 && _enemySetNumber > 0)
        {
            _enemyNumber = Random.Range(_waveCounter, _waveCounter + Random.Range(1,2));
            
            _enemySetNumber--;
            _normalEnemyTimer2 = _normalEnemyCooldown2;
        }

        if(_normalEnemyTimer3 <= 0 && _normalEnemyTimer2 <= 0 && _normalEnemyTimer1 <= 0 && _enemyNumber > 0)
        {
            SpawnEnemy(_normalEnemy, _enemyWaveSpeed, _enemyWaveHp);
        }
        
        #endregion

    }

    public void BuildCannon()
    {
        if (onBuild)
            return;
        _OnBuildingObject = Instantiate(_cannonObject, _mousePos,Quaternion.identity);

        onBuild = true;
    }

    private void SpawnEnemy(GameObject enemy,float speed,int hp)
    {
        GameObject a = Instantiate(enemy, _enemySpawnPoint.transform.position, Quaternion.identity);
        a.GetComponent<EnemyScript>().SetSpeed(speed);
        a.GetComponent<EnemyScript>().SetHp(hp);
        _enemyNumber--;
        _normalEnemyTimer1 = _normalEnemyCooldown1;
    }

    public static void GetCoin(int value)
    {
        _coin += value;
    }

    public static void damageHP(int a)
    {
        _hitpoints -= a;
    }
}
