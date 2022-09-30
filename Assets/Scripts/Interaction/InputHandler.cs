using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public ExhibitManagerSO exhibitManager;
    public Transform leftHand;
    public Transform rightHand;
    public float grabRad;
    public LayerMask artifactLayer;

    private bool leftPressed = false;
    private bool rightPressed = false;

    void Grab(Transform hand)
    {
        Collider[] colliders = Physics.OverlapSphere(hand.position, grabRad, artifactLayer);

        Collider closest = null;
        float closeDist = Mathf.Infinity;
        foreach (Collider col in colliders)
        {
            float dist = Vector3.Distance(col.transform.position, hand.position);
            if (dist < closeDist)
            {
                closest = col;
                closeDist = dist;
            }
        }

        if (closest)
        {
            exhibitManager.InvokeOnGrab(closest.transform, hand);
        }
    }

    public void OnLeftTrigger(InputValue value)
    {
        if (value.isPressed && !leftPressed)
        {
            leftPressed = true;
            Grab(leftHand);
        }
        else if (!value.isPressed && leftPressed)
        {
            leftPressed = false;
            exhibitManager.InvokeOnRelease(leftHand);
        }
    }

    public void OnRightTrigger(InputValue value)
    {
        if (value.isPressed && !rightPressed)
        {
            rightPressed = true;
            Grab(leftHand);
        }
        else if (!value.isPressed && rightPressed)
        {
            rightPressed = false;
            exhibitManager.InvokeOnRelease(rightHand);
        }
    }
}
