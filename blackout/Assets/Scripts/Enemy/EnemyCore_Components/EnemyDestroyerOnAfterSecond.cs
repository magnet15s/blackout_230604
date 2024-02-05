using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyerOnAfterSecond : MonoBehaviour
{

    [SerializeField] float deleySecond;
    // Start is called before the first frame update
    public void DestroyOnAfterSecond()
    {
        EnemyCore ec;
        transform.TryGetComponent<EnemyCore> (out ec);
        ec.DefaultDestroy();
        StartCoroutine("Cor");
    }

    IEnumerator Cor()
    {
        yield return new WaitForSeconds(Mathf.Max(0.1f, deleySecond));
        Destroy(gameObject);
    }
}
