using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance; 

    public ExhibitManagerSO exhibitManager;
    public Transform leftHand;
    public Transform rightHand;
    public float grabRad;
    public LayerMask artifactLayer;

    private bool leftPressed = false;
    private bool rightPressed = false;

    public UnityAction<Vector2> onMove;
    public UnityAction<Vector2> onTurn;

    private void Awake()
    {
        instance = this;
    }

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
            exhibitManager.InvokeOnGrab(closest.GetComponent<GrabLogic>(), hand);
            print("GRABBING " + closest.name);
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
            print("RELEASING LEFT");
        }
    }

    public void OnRightTrigger(InputValue value)
    {
        print("RIGHT");
        if (value.isPressed && !rightPressed)
        {
            rightPressed = true;
            Grab(rightHand);
        }
        else if (!value.isPressed && rightPressed)
        {
            rightPressed = false;
            exhibitManager.InvokeOnRelease(rightHand);
            print("RELEASING RIGHT");
        }
    }

    public void OnNextSlide(InputValue value)
    {
        print("NEXT");
        if (value.isPressed)
        {
            exhibitManager.InvokeOnNextSlide();
        }
    }

    public void OnBackSlide(InputValue value)
    {
        print("BACK");
        if (value.isPressed)
        {
            exhibitManager.InvokeOnBackSlide();
        }
    }

    public void OnMovement(InputValue value)
    {
        onMove?.Invoke(value.Get<Vector2>());
    }

    public void OnTurn(InputValue value)
    {
        onTurn?.Invoke(value.Get<Vector2>());
    }
}
