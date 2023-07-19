using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParts : MonoBehaviour, DamageReceiver
{
    [SerializeField] ShieldRoot shieldRoot;
    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        if (shieldRoot != null) {
            shieldRoot.HitReceive(this, damage, hitPosition, source, damageType);
        } else {
            Debug.LogError($"{this.ToString()} > ShieldRoot not set reference");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
