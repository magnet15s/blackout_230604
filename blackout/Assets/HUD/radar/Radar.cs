using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Radar : MonoBehaviour
{

    [Tooltip("GroundMeshPlaneの1辺の大きさの逆数")]
    [SerializeField] private float groundMeshPlaneSizeFct = 0.5f;
    private float world2radarSize;

    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerController pc;
    [SerializeField] private Transform camPivot;
    [SerializeField] private Transform playerIcon;

    [SerializeField] private float playerIconSizeFactor = 1;
    private float playerIconInitialSize;
    [SerializeField] private float enemyIconSizeFactor = 1;
    private float enemyIconInitialSize;

    [SerializeField] private Material groundMat;

    private GameObject enemyIcon;

    [Tooltip("網の目の実寸上の大きさ[m]")]
    public float groundMeshSize = 5;

    [Tooltip("レーダーの縮尺[%]")]
    public float radarScale = 1;

    [SerializeField] private Color enemyColor;

    private Dictionary<Enemy, Transform> enemies = new();
    void AddEnemy(Enemy enemy)
    {
        this.enemies.Add(enemy, ((GameObject)Instantiate(enemyIcon, this.transform)).transform);
    }

    void RemoveEnemy(Enemy enemy)
    {
        //Debug.Log("");
        Transform e;
        if(enemies.TryGetValue(enemy, out e))
        {
            this.enemies.Remove(enemy);
            Destroy(e.gameObject);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyIcon = (GameObject)Resources.Load("HUD/RadarEnemyIcon");
        
        playerIconInitialSize = playerIcon.localScale.x;
        enemyIconInitialSize = enemyIcon.transform.localScale.x;
        world2radarSize = 0.001f * radarScale;
        groundMat.SetFloat("_Size", groundMeshPlaneSizeFct * world2radarSize * groundMeshSize);

        foreach(Enemy e in Enemy.EnemiesList)
        {
            enemies.Add(e, ((GameObject)Instantiate(enemyIcon,this.transform)).transform);
        }
        Enemy.EnemySpawn += AddEnemy;
        Enemy.EnemyDestroy += RemoveEnemy;

    }

    // Update is called once per frame
    void Update()
    {
        world2radarSize = 0.001f * radarScale;
        groundMat.SetFloat("_Size", groundMeshPlaneSizeFct * world2radarSize * groundMeshSize);

        //各アイコンサイズ
        playerIcon.localScale = new Vector3(playerIconInitialSize * playerIconSizeFactor, playerIconInitialSize * playerIconSizeFactor, playerIconInitialSize * playerIconSizeFactor);
        Vector3 enemyIconScale = new Vector3(enemyIconInitialSize * enemyIconSizeFactor, enemyIconInitialSize * enemyIconSizeFactor, enemyIconInitialSize * enemyIconSizeFactor);

        playerIcon.eulerAngles = new Vector3(90, player.eulerAngles.y, 0);
        camPivot.eulerAngles = new Vector3(0, playerCamera.eulerAngles.y, 0);
        groundMat.SetFloat("_Xtpl", 1 - player.position.x % groundMeshSize / groundMeshSize);
        groundMat.SetFloat("_Ytpl", 1 - player.position.z % groundMeshSize / groundMeshSize);

        foreach(KeyValuePair<Enemy,Transform> e in enemies)
        {
            e.Value.localScale = enemyIconScale;
            if(Mathf.Abs((e.Key.transform.position - player.position).magnitude) < e.Key.onRadarDist)
            {
                e.Value.GetComponent<SpriteRenderer>().color = enemyColor;
                Vector3 ePos = (e.Key.transform.position - player.position) * world2radarSize;
                e.Value.localPosition = new Vector3(ePos.x, e.Value.localPosition.y, ePos.z);
                e.Value.eulerAngles = new Vector3(90, e.Key.transform.eulerAngles.y, 0);
            }
            else
            {
                e.Value.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    
}
