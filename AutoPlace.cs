using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoPlace : MonoBehaviour
{

    private GameObject currentTarget;
    public GameObject PointerTarget;
    public bool IsRight;
    // Start is called before the first frame update
    void Start()
    {
        PointerTarget = GameObject.FindWithTag("EyePointer");
        FindAndSetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentTarget) return;

        //transform.position = currentTarget.transform.position;
    }
    private void FindAndSetTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Destructible");
        if (targets.Length > 0)
        {
            currentTarget = GetClosestTarget(targets);
            //Place the wall at the point where its edges would touch the edges of the other nearby wall
            //For that, we need:
            //The position of the nearby wall
            //The width of the nearby wall
            //The orientation of the nearby wall
            //The position of the pointer
            //   Dragged towards the point where the edge of the nearby wall would be
            //   But not dragged all the way, so the orientation of the new wall will be such that a line drawn from its center to its edge will meet a line drawn from the center of the other wall to its edge

            if (Vector3.Distance(currentTarget.transform.position, PointerTarget.transform.position) < 7f)
            {
                if (IsRight == true)
                {
                    BoxCollider collider = currentTarget.GetComponent<BoxCollider>();
                    Vector3 nearestWallRightEdge = currentTarget.transform.position + (currentTarget.transform.right * (collider.size.x / 2));

                    // Get the offset to the left edge of the new wall
                    BoxCollider colliderMine = gameObject.GetComponent<BoxCollider>();
                    Vector3 directionToNearestWall = nearestWallRightEdge - gameObject.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToNearestWall);
                    Vector3 newWallLeftEdgeOffset = gameObject.transform.right * (colliderMine.size.x / 2);

                    // Calculate the position for the new wall
                    transform.position = nearestWallRightEdge + newWallLeftEdgeOffset;
                }
                else
                {
                    BoxCollider collider = currentTarget.GetComponent<BoxCollider>();
                    Vector3 nearestWallLeftEdge = currentTarget.transform.position - (currentTarget.transform.right * (collider.size.x / 2));

                    // Get the offset to the left edge of the new wall
                    BoxCollider colliderMine = gameObject.GetComponent<BoxCollider>();
                    Vector3 directionToNearestWall = nearestWallLeftEdge - gameObject.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToNearestWall);
                    Vector3 newWallLeftEdgeOffset = -gameObject.transform.right * (colliderMine.size.x / 2);

                    // Calculate the position for the new wall
                    transform.position = nearestWallLeftEdge + newWallLeftEdgeOffset;
                }

            }
        }
        gameObject.tag = "Destructible";
    }

    GameObject GetClosestTarget(GameObject[] targets)
    {
        return targets
            .OrderBy(t => (t.transform.position - PointerTarget.transform.position).sqrMagnitude)
            .FirstOrDefault();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Destructible"))
        {
            BoxCollider colliderMine = gameObject.GetComponent<BoxCollider>();
            if(Vector3.Distance(collision.transform.position, gameObject.transform.position) < (colliderMine.size.x))
            {
                Destroy(gameObject);
            }
        }
    }
}
