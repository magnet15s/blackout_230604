using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssenbleData : MonoBehaviour
{
    public static List<WeaponData> mountWeapons;

    public struct WeaponData
    {
        public string prefabName;
        public bool onWeaponSlot;
        public WeaponConnectionToBone.BoneType connectBone;
        public List<WeaponData> ancillaryParts;
    }
    
}
