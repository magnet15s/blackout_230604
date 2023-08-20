using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HUDAim : MonoBehaviour
{
    [SerializeField] private Transform trackObj;
    [SerializeField] private RectTransform Canvas;
    [SerializeField] private Transform player;
    private RectTransform rectTransform;
    public GameObject trackingIcon;

    private List<Enemy> enemiesInTrackingArea = new();
    private List<Enemy> trackingEnemies = new();
    private List<TrackingIcon> icons = new();

    [SerializeField] float trackingUpdateInterval = 0.3f;
    private float TUICnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if(trackingIcon == null)
        {
            trackingIcon = (GameObject)Resources.Load("HUD/EnemyTracking");
        }
        Debug.Log(rectTransform.sizeDelta.x + " " + rectTransform.sizeDelta.y );


    }

    // Update is called once per frame
    void Update()
    {
        //自身のスクリーン座標を計算
        Vector2 w2sPos = Camera.main.WorldToScreenPoint(trackObj.position);
        rectTransform.position = new Vector2(
            w2sPos.x,
            w2sPos.y);

        //トラッキング更新
        TUICnt -= Time.deltaTime;
        if(TUICnt < 0)
        {
            TUICnt = trackingUpdateInterval;


            //トラッキングエリア上にいる敵を探してenemiesInTrackingAreaに登録する
            foreach (Enemy enemy in Enemy.EnemiesList)
            {
                if (!enemiesInTrackingArea.Exists(x => x.Equals(enemy)))//すでに登録済みか
                {
                    if (InTheTrackingArea(Camera.main.WorldToScreenPoint(enemy.transform.position))) //enemyがトラッキングエリア上に存在するか
                    {
                        if(Vector3.Dot((enemy.transform.position - player.position).normalized, player.forward) >= 0)
                        enemiesInTrackingArea.Add(enemy);
                    }
                }else if (!InTheTrackingArea(Camera.main.WorldToScreenPoint(enemy.transform.position)))
                {
                    enemiesInTrackingArea.Remove(enemy);
                    if(trackingEnemies.Find(x => x.Equals(enemy)))
                    {
                        int teidx = icons.FindIndex(x => x.trackingTarget.Equals(enemy.gameObject));
                        Destroy(icons[teidx].gameObject);
                        icons.Remove(icons[teidx]);
                        trackingEnemies.Remove(enemy);

                    }
                    
                }
            }

            //enemiesInTrackingAreaのenemyに対してrayを発射→最初にそのenemyに当たればtrackingEnemiesに登録
            foreach(Enemy enemy in enemiesInTrackingArea)
            {
                if (!trackingEnemies.Exists(x => x.Equals(enemy)))
                {
                    Physics.Raycast(player.position + (enemy.transform.position - player.position).normalized * 5,enemy.transform.position - player.position + (enemy.transform.position - player.position).normalized * 5, out RaycastHit result);
                    if (result.transform != null && result.transform.Equals(enemy.transform))
                    {
                        trackingEnemies.Add(enemy);
                        TrackingIcon ti = Instantiate(trackingIcon, this.transform).GetComponent<TrackingIcon>();
                        icons.Add(ti);
                        ti.player = player.gameObject;
                        ti.trackingTarget = enemy.gameObject;

                    }
                }
                else
                {
                    Physics.Raycast(player.position + (enemy.transform.position - player.position).normalized * 5, enemy.transform.position - player.position + (enemy.transform.position - player.position).normalized * 5, out RaycastHit result);

                    if (result.transform == null || !result.transform.Equals(enemy.transform))
                    {
                        int teidx = icons.FindIndex(x => x.trackingTarget.Equals(enemy.gameObject));
                        Destroy(icons[teidx].gameObject);
                        icons.Remove(icons[teidx]);
                        trackingEnemies.Remove(enemy);
                    }
                }
                
            }

            //Enemy.enemieslistから消えていた場合（撃破）
            try
            {
                foreach (Enemy enemy in enemiesInTrackingArea)
                {
                    if (!Enemy.EnemiesList.Find(x => x.Equals(enemy)))
                    {
                        enemiesInTrackingArea.Remove(enemy);
                        if (trackingEnemies.Find(x => x.Equals(enemy)))
                        {
                            int teidx = icons.FindIndex(x => x.trackingTarget.Equals(enemy.gameObject));
                            Debug.Log(teidx);
                            Destroy(icons[teidx].gameObject);
                            icons.Remove(icons[teidx]);
                            trackingEnemies.Remove(enemy);

                        }
                    }
                }

            }
            catch (InvalidOperationException e) {
                Debug.LogWarning("謎のエラー:" + e);
            } catch (Exception e) {
                throw e;
            }

        }

        /*string log = "";
        foreach(Enemy e in enemiesInTrackingArea)
        {
            log += $" {e.ToString()}";
        }
        log += $"\n////////////////////////\n{trackingEnemies.Count}";
        foreach(KeyValuePair<Enemy,GameObject> e in trackingEnemies)
        {
            log += $" {e.ToString()}";
        }
        Debug.LogWarning(log);*/

    }

    private bool InTheTrackingArea(Vector3 screenPoint)
    {
        //strech設定の場合うまく動かない
        /*Vector3 dist = rectTransform.position - screenPoint;
        if(Mathf.Abs(dist.x) < (Mathf.Abs(rectTransform.sizeDelta.x) * 0.5) / 2 && Mathf.Abs(dist.y) < (Mathf.Abs(rectTransform.sizeDelta.y) * 0.5) / 2)//sizeDeltaの係数は自身のサイズにおけるトラッキングエリアのサイズの割合
            return true;
        else return false;*/
        Vector2 areaOfsMin = rectTransform.offsetMin * 2;
        Vector2 areaOfsMax = new Vector2(Screen.width,Screen.height) + rectTransform.offsetMax * 2;
        
        //Debug.Log($"{screenPoint} {areaOfsMin} {areaOfsMax} {Screen.width}");
        if (screenPoint.x < areaOfsMax.x && screenPoint.y < areaOfsMax.y && screenPoint.x > areaOfsMin.x && screenPoint.y > areaOfsMin.y)
            return true;
        else return false;
    }
}
