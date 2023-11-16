using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ItemKeeper : MonoBehaviour
{
    public List<Collider> collidersToTrack = new List<Collider>();
    private List<ColliderData> colliderDataList = new List<ColliderData>(); 
    private int Index;
    public GameObject Player;
    public Magic MainMagic;
    private GameObject childToFind;
    private GameObject foundObject;
    private GameObject childToFindL;
    private GameObject foundObjectL;

    void FixedUpdate()
    {
        if (MainMagic.HeldR != null)
        {
            childToFind = MainMagic.HeldR;
            foundObject = FindInChildren(gameObject, childToFind.name);
        } else { foundObject = null; }

        if (MainMagic.HeldL != null)
        {
            childToFindL = MainMagic.HeldL;
            foundObjectL = FindInChildren(gameObject, childToFindL.name);
        }
        else { foundObjectL = null; }


        if (foundObject != null)
        {
            Debug.Log("I AM TRYING TO RELEASE YOU");
           // MainMagic.HeldR.transform.position = MainMagic.rightHand.transform.position;
            MainMagic.HeldR.transform.GetComponent<Rigidbody>().isKinematic = false;
            MainMagic.HeldR.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            MainMagic.HeldR.layer = LayerMask.NameToLayer("Objects");
            MainMagic.HeldR.transform.parent.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            MainMagic.HeldR.transform.parent = null;
            MainMagic.HeldR.transform.localScale = Vector3.one;
            //MainMagic.ProcessTelekinesisLogic(.GetComponent<Rigidbody>(), null);
            Debug.Log("I RELEASE YOU");// Found the object within the hierarchy of parentObject
        }
        if (foundObjectL != null)
        {
            Debug.Log("I AM TRYING TO RELEASE YOU");
            // MainMagic.HeldR.transform.position = MainMagic.rightHand.transform.position;
            MainMagic.HeldL.transform.GetComponent<Rigidbody>().isKinematic = false;
            MainMagic.HeldL.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            MainMagic.HeldL.layer = LayerMask.NameToLayer("Objects");
            MainMagic.HeldL.transform.parent.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            MainMagic.HeldL.transform.parent = null;
            MainMagic.HeldL.transform.localScale = Vector3.one;
            //MainMagic.ProcessTelekinesisLogic(.GetComponent<Rigidbody>(), null);
            Debug.Log("I RELEASE YOU");// Found the object within the hierarchy of parentObject
        }



        foreach (var collider in collidersToTrack)
        {        
            if (collider.transform.childCount == 0)
            {
                Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Objects") && hitCollider.gameObject != MainMagic.HeldR && hitCollider.gameObject != MainMagic.HeldL)
                    {
                        HandleCollision(collider, hitCollider.gameObject, Index);
                    }                      
                }
            }       
            if (Index < collidersToTrack.Count)
            {       
                Index++;
            }  
            else 
            {         
                Index = 0; 
            }
        }

    }

    GameObject FindInChildren(GameObject parent, string nameToFind)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == nameToFind)
            {
                return child.gameObject;
            }

            GameObject found = FindInChildren(child.gameObject, nameToFind);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private void HandleCollision(Collider collider, GameObject hitObject, int Slot)
    {
        
        hitObject.transform.parent = collider.transform;
        //hitObject.transform.localScale = Vector3.one;

        float cubeSize = collider.transform.localScale.x;

        // Calculating the bounding box size of the object
        Bounds objectBounds = hitObject.transform.gameObject.GetComponent<Renderer>().bounds;
        Vector3 objectSize = objectBounds.size;

        // Finding the largest dimension of the object
        float maxObjectSize = Mathf.Max(objectSize.x, objectSize.y, objectSize.z);

        // Calculating the scale factor
        float scaleFactor = cubeSize / maxObjectSize;

        // Scaling the object
        if (maxObjectSize > cubeSize)
        {
            hitObject.transform.localScale *= scaleFactor;
        }
        // Centering the object
        hitObject.transform.localPosition = Vector3.zero;
        hitObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        Rigidbody rb = hitObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;


        hitObject.layer = LayerMask.NameToLayer("Default");
        collider.gameObject.layer = LayerMask.NameToLayer("Default");

        SaveColliderData(collider, hitObject, Slot, objectSize);
    }

    private void SaveColliderData(Collider collider, GameObject hitObject, int Slot, Vector3 objectSize)
    {
        ColliderData data = new ColliderData
        {
            prefabName = hitObject.name,
            prefab = hitObject,
            position = collider.transform.position,
            isKinematic = collider.attachedRigidbody ? collider.attachedRigidbody.isKinematic : false,
            slotNumber = Slot,
            OriginalSize = objectSize
        };
        colliderDataList.Add(data);
    }

    public void SaveState()
    {
        string json = JsonUtility.ToJson(new ColliderDataList { colliderData = colliderDataList });
        File.WriteAllText(Application.persistentDataPath + "/InventoryList.json", json);
    }

    public void LoadState()
    {
        if (File.Exists(Application.persistentDataPath + "/InventoryList.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/InventoryList.json");
            ColliderDataList data = JsonUtility.FromJson<ColliderDataList>(json);
            colliderDataList = data.colliderData;

            foreach (ColliderData colliderData in colliderDataList)
            {
                GameObject prefab = Resources.Load<GameObject>(colliderData.prefabName);
                if (prefab != null)
                {
                    GameObject instance = Instantiate(prefab, colliderData.position, Quaternion.identity);
                    Rigidbody rb = instance.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = colliderData.isKinematic;
                    }
                }
            }
        }
    }
}

[Serializable]
public class ColliderData
{
    public string prefabName;
    public GameObject prefab;
    public Vector3 position;
    public bool isKinematic;
    public int slotNumber;
    public Vector3 OriginalSize;
}

[Serializable]
public class ColliderDataList
{
    public List<ColliderData> colliderData;
}
