using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    private GameObject currentTarget;
    public GameObject PointerTarget;
    // Start is called before the first frame update
    void Start()
    {
        FindAndSetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentTarget) return;
        transform.position = currentTarget.transform.position;
    }
    private void FindAndSetTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        if (targets.Length > 0)
        {
            currentTarget = GetClosestTarget(targets);
            transform.position = currentTarget.transform.position;
        }
    }

    GameObject GetClosestTarget(GameObject[] targets)
    {
        return targets
            .OrderBy(t => (t.transform.position - PointerTarget.transform.position).sqrMagnitude)
            .FirstOrDefault();
    }
}
