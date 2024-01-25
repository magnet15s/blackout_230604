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

    public WeaponConnectionToBone(BoneType boneType, Transform bone) {
        this.boneType = boneType;
        this.bone = bone;
    }

    public bool Connect(Transform connectObj) {
        if(bone == null) {
            Debug.LogError("bone is null");
        }
        return false;
    }

    public BoneType boneType { get; private set; }

    public List<GameObject> weapon;
    public Transform bone { get; private set; }
    

    
}
