using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mobileTextView;
    [SerializeField] List<TextMeshProUGUI> wepTextViews;
    [SerializeField] PlayerController pc;
    private string text;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mobileTextView.text = CreateMobileViewText();
        CreateWepViewText();
    }

    string CreateMobileViewText()
    {
        return $"YLD-3s";
    }

    void CreateWepViewText()
    {
        string text = "";
        for(int i = 0; i < pc.weapons.Count; i++)
        {
            wepTextViews[i].text = wepText(pc.weapons[i]);
        }
    }

    string wepText(Weapon w)
    {
        string a = w.remainAmmo == null ? " / " : w.remainAmmo.ToString();
        return $"{w.weaponName}\nammo : {a}";
    }
}
