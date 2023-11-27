using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTransmitter : MonoBehaviour , DamageReceiver
{
    [SerializeField] private GameObject TargetDamageReceiver;
    private DamageReceiver TransTarget;
    private void Start()
    {
        TransTarget = TargetDamageReceiver.GetComponent<DamageReceiver>();
    }
    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType)
    {
        if(TransTarget != null)TransTarget.Damage(damage, hitPosition, source, damageType);
    }
}
