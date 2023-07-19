using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ShieldRoot
{
    public abstract void HitReceive(ShieldParts receiver, int damage, Vector3 hitPosition, GameObject source, string damageType);
}
