using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public sealed class HomingSample : MonoBehaviour {

    [SerializeField]
    Transform target;
    [SerializeField, Min(0)]
    float time = 0.5f;
    [SerializeField]
    float lifeTime = 2;
    [SerializeField]
    bool limitAcceleration = true;
    [SerializeField, Min(0)]
    float maxAcceleration = 1;
    [SerializeField]
    Vector3 minInitVelocity;
    [SerializeField]
    Vector3 maxInitVelocity;

    Vector3 position;
    Vector3 velocity;
    Vector3 acceleration;
    Transform thisTransform;

    public Transform Target {
        set {
            target = value;
        }
        get {
            return target;
        }
    }

    void Start() {
        thisTransform = transform;
        position = thisTransform.position;
        velocity = new Vector3(Random.Range(minInitVelocity.x, maxInitVelocity.x), Random.Range(minInitVelocity.y, maxInitVelocity.y), Random.Range(minInitVelocity.z, maxInitVelocity.z));

        StartCoroutine(nameof(Timer));
    }

    public void Update() {
        if (target == null) {
            return;
        }

        acceleration = 2f / (time * time) * (target.position - position - time * velocity);

        if (limitAcceleration && acceleration.sqrMagnitude > maxAcceleration * maxAcceleration) {
            acceleration = acceleration.normalized * maxAcceleration;
        }

        time -= Time.deltaTime;

        if (time < 0f) {
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        thisTransform.position = position;
        thisTransform.rotation = Quaternion.LookRotation(velocity);
    }
    void OnCollisionEnter(Collision collision) {
        Destroy(gameObject);
    }


    IEnumerator Timer() {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

}