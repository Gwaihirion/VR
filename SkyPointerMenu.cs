using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class SkyPointerMenu : MonoBehaviour
{
    public GameObject circularMenuPrefab; // Assign your circular menu prefab
    private GameObject circularMenuInstance;
    public GameObject InventoryShelf;
    public GameObject InventoryChest;
    public GameObject InventoryDesk;
    public GameObject InventoryTable;
    public SpawnEffect ShelfSpawnFade;
    public SpawnEffect ChestSpawnFade;
    public SpawnEffect DeskSpawnFade;
    public SpawnEffect TableSpawnFade;
    private OVRInput.Controller leftController = OVRInput.Controller.LTouch;
    private OVRInput.Controller rightController = OVRInput.Controller.RTouch;
    public GameObject Player;
    public GameObject rightHand;
    public GameObject CenterEyeTracker;
    public GameObject NosePointer;
    public GameObject TeleportFlash;
    private float TeleportTimer;

    void Start()
    {
        ShelfSpawnFade = InventoryShelf.GetComponent<SpawnEffect>();
        //ChestSpawnFade = InventoryChest.GetComponent<SpawnEffect>();
        DeskSpawnFade = InventoryDesk.GetComponent<SpawnEffect>();
        TableSpawnFade = InventoryTable.GetComponent<SpawnEffect>();
        NosePointer = Player.GetComponent<Magic>().NosePointer;
        CenterEyeTracker = Player.GetComponent<Magic>().CenterEyeTracker;
        rightHand = Player.GetComponent<Magic>().rightHand;
    }

    void Update()
    {
        if (IsControllerPointingUp(leftController) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, leftController) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController))
        {
            InventoryShelf.transform.position = new Vector3(NosePointer.transform.position.x, NosePointer.transform.position.y + .1f, NosePointer.transform.position.z);
            InventoryShelf.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y + 180f, 0);
            ShelfSpawnFade.Spawn();
            GameObject Flash = Instantiate(TeleportFlash, InventoryShelf.transform.position, Quaternion.identity);
        } else if (IsControllerPointingUp(leftController) && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, leftController) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController))
        {
            InventoryChest.transform.position = new Vector3(NosePointer.transform.position.x, NosePointer.transform.position.y + .1f, NosePointer.transform.position.z);
            InventoryChest.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y + 180f, 0);
           // ChestSpawnFade.Spawn();
            GameObject Flash = Instantiate(TeleportFlash, InventoryChest.transform.position, Quaternion.identity);
        } else if (IsControllerPointingUp(rightController) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, rightController) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController))
        {
            InventoryDesk.transform.position = new Vector3(NosePointer.transform.position.x, NosePointer.transform.position.y + .1f, NosePointer.transform.position.z);
            InventoryDesk.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y, 0);
            DeskSpawnFade.Spawn();
            GameObject Flash = Instantiate(TeleportFlash, InventoryDesk.transform.position, Quaternion.identity);
        } else if (IsControllerPointingUp(rightController) && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, rightController) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController))
        {
            InventoryTable.transform.position = new Vector3(NosePointer.transform.position.x, NosePointer.transform.position.y + .1f, NosePointer.transform.position.z);
            InventoryTable.transform.rotation = Quaternion.Euler(0, Player.transform.eulerAngles.y, 0);
            TableSpawnFade.Spawn();
            GameObject Flash = Instantiate(TeleportFlash, InventoryTable.transform.position, Quaternion.identity);
        } else if (IsControllerPointingUp(rightController) && IsControllerPointingUp(leftController) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, rightController) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, leftController) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController))
        {
            
            
            Ray EyeRay = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
            RaycastHit hit1;
            LayerMask EyeMask = LayerMask.GetMask("Ground");
            if (Physics.Raycast(EyeRay, out hit1, Mathf.Infinity, EyeMask))
            {
                TeleportTimer += 1f * Time.deltaTime;
                if (TeleportTimer > 1f)
                {
                    transform.position = new Vector3(hit1.point.x, hit1.point.y + .5f, hit1.point.z);
                    TeleportTimer = 0f;
                }
            }
        }
    }

    bool IsControllerPointingUp(OVRInput.Controller Hand)
    {
        Vector3 controllerForward = OVRInput.GetLocalControllerRotation(Hand) * Vector3.forward;
        return Vector3.Dot(controllerForward, Vector3.up) > 0.7f;
    }

    void SelectMenuItem(Vector2 input)
    {
        // Implement your menu selection logic here
    }
}
