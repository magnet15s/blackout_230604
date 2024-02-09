using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyDestroyerOnAfterSecond : MonoBehaviour
{

    [SerializeField] float deleySecond;
    // Start is called before the first frame update
    public void DestroyOnAfterSecond()
    {
        EnemyCore ec;
        transform.TryGetComponent<EnemyCore> (out ec);
        if (ec.alive) {
            NavMeshAgent navAgent = GetComponent<NavMeshAgent> ();
            if (navAgent != null) {
                navAgent.destination = transform.position;
                navAgent.stoppingDistance = 100;
            }
            ec.alive = false;

        }
        StartCoroutine("Cor");
    }

    IEnumerator Cor()
    {
        yield return new WaitForSeconds(Mathf.Max(0.1f, deleySecond));
        Destroy(gameObject);
    }
}
