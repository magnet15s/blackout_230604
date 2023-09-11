using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, DamageReceiver
{
    public delegate void enemySpawnHandler(Enemy enemy);
    public static event enemySpawnHandler EnemySpawn;

    public delegate void enemyDestroyHandler(Enemy enemy);
    public static event enemyDestroyHandler EnemyDestroy;

    private static int lastId = 0;

    
    public static GameObject sharedTarget { get; set; } = null;
    public static Vector3? sharedTargetPosition { get; protected set; } = null;
    public static Enemy targetReporter { get; protected set; } = null;
    public static List<Enemy> EnemiesList { get; protected set; } = new();


    public int enemyId { get; private set; } = -1;
    [SerializeField] private int id; //unity�C���X�y�N�^�[��ŕ\������p

    [SerializeField] public string modelName;
    [SerializeField] public int maxArmorPoint;
    [SerializeField] public int armorPoint;
    [SerializeField] public int onRadarDist;


    public virtual void MainFire() {
        Debug.LogWarning($"���C������������ : {this}");
    }

    public virtual void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        Debug.LogWarning($"�_���[�W���������� : {this} <- {source}[source] ");
    }

    public virtual void Awake()
    {
        Enemy.EnemiesList.Add(this);
        enemyId = Enemy.lastId + 1;
        lastId++;
        id = enemyId;
        OnEnemySpawn(this);//�������X�|�[���������Ƃ�ʒm
    }



    protected virtual void OnEnemySpawn(Enemy enemy)
    {
        EnemySpawn?.Invoke(enemy);
    }

    protected virtual void OnEnemyDestroy(Enemy enemy)
    {
        int idx = EnemiesList.FindIndex(x => x.Equals(enemy));
        if(idx != -1) EnemiesList.Remove(EnemiesList[idx]);
        if (targetReporter != null && targetReporter.Equals(enemy)) targetReporter = null;
        EnemyDestroy?.Invoke(enemy);
    }
}
