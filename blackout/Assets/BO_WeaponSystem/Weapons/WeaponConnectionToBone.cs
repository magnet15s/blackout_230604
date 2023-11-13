using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponConnectionToBone{
    public enum BoneType {
        model_root,
        head,
        arm_right,
        arm_left,
        forearm_right,
        forearm_left,
        hand_right,
        hand_left,
        chest,
        spine,
        hips,
        upper_leg_right,
        upper_leg_left,
        lower_leg_right,
        lower_leg_left
    }

    public WeaponConnectionToBone(BoneType boneType, GameObject bone) {
        this.boneType = boneType;
        this.bone = bone;
    }

    public BoneType boneType { get; private set; }

    public List<GameObject> weapon;
    public GameObject bone { get; private set; }
    public Vector3 connectionOffset;

    
}
