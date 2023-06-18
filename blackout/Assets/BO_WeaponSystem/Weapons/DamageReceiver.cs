using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public interface DamageReceiver
{
    public abstract void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType);
}
