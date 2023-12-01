using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour, DamageReceiver
{
    [SerializeField] public int maxArmorPoint;
    [SerializeField] public int ArmorPoint;

    [SerializeField] private UnityEvent OnDestroyEvent;

    [SerializeField] private UnityEvent DamageEvent;
    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        ArmorPoint -= damage;
        DamageEvent.Invoke();
        if(ArmorPoint <= 0) {
            OnDestroyEvent.Invoke();
        }
    }

}
