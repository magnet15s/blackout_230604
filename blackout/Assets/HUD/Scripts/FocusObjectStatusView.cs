using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FocusObjectStatusView : MonoBehaviour
{
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
                enemyFocusText = $"Focus : {enemy.modelName}\n" +
                    $"Armor : {enemy.maxArmorPoint}\n" +
                    $"Damage level : {damageLevel}";
                tmp.text = enemyFocusText;
            }
        }
        else
        {
            tmp.text = noFocusText;
        }
    }
}
