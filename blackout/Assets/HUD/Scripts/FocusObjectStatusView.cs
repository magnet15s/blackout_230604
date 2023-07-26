using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FocusObjectStatusView : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] TextMeshProUGUI tmp;
    public string noFocusText;
    private string enemyFocusText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TrackingIcon.closestIconToCenter != null)
        {
            Enemy enemy;
            if ((enemy = TrackingIcon.closestIconToCenter.trackingTarget.transform.GetComponent<Enemy>()) != null)
            {
                string damageLevel;
                if ((float)enemy.armorPoint / (float)enemy.maxArmorPoint == 1) damageLevel = "not damaged";
                else if ((float)enemy.armorPoint / (float)enemy.maxArmorPoint > 0.7) damageLevel = "minor";
                else if ((float)enemy.armorPoint / (float)enemy.maxArmorPoint > 0.5) damageLevel = "some degree";
                else if ((float)enemy.armorPoint / (float)enemy.maxArmorPoint > 0.3) damageLevel = "major";
                else if ((float)enemy.armorPoint / (float)enemy.maxArmorPoint > 0.1) damageLevel = "serious";
                else damageLevel = "fatal";

                float relativeDistance = (enemy.transform.position - player.position).magnitude;
                int rdDec = (int)(relativeDistance * 100f % 100);
                int rdInt = (int)relativeDistance;

                enemyFocusText = 
                    $"Focus : {enemy.modelName}\n" +
                    $"Armor : {enemy.maxArmorPoint}\n" +
                    $"Damage level : {damageLevel}\n" +
                    $"Relative distance : {rdInt}.{rdDec}m";
                tmp.text = enemyFocusText;
            } else {
                enemyFocusText =
                    $"Focus : {TrackingIcon.closestIconToCenter.gameObject.name.ToString()}";
            }
        }
        else
        {
            tmp.text = noFocusText;
        }
    }
}
