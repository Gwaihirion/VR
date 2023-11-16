using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagOfHolding : MonoBehaviour
{
    public GameObject Player;
    public Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, Player.transform.rotation.y, 0f);

        Vector3 Syzygy = new Vector3(transform.position.x, 0f, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(Syzygy + Vector3.up * 10000, Vector3.down, out hit, Mathf.Infinity))
        {
            position.y = hit.point.y - 30f; // Set y to the hit point's y coordinate
        }
        transform.position = new Vector3(Player.transform.position.x, position.y, Player.transform.position.z);

    }
}
