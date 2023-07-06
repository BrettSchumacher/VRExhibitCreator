using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnGround : MonoBehaviour
{
    public LayerMask groundMask;

    Vector3 startPos;
    Vector3 startScale;
    Quaternion startRot;

    private void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
        startRot = transform.localRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layerNum = 1 << collision.collider.gameObject.layer;
        if ((layerNum & groundMask.value) != 0)
        {
            transform.localPosition = startPos;
            transform.localScale = startScale;
            transform.localRotation = startRot;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
