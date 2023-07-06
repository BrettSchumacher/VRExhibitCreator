using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSelect : MonoBehaviour
{
    public ExhibitManagerSO exhibitManager;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if (!cam)
        {
            cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo);
            print("Clicked on: " + hitInfo.collider?.name);
            exhibitManager.InvokeUpdateMainObj(hitInfo.collider?.GetComponent<GrabLogic>());
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            exhibitManager.InvokeOnMainInput();
        }
    }

}
