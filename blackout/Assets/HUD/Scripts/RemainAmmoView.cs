using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemainAmmoView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PlayerController pc;
    private Weapon selectWeapon;
    [SerializeField] private TextMeshProUGUI tmp;
    void Start()
    {
        selectWeapon = pc.selectWep;
    }

    // Update is called once per frame
    void Update()
    {
        selectWeapon = pc.selectWep;
        tmp.text = selectWeapon.remainAmmo.ToString();

    }
}
