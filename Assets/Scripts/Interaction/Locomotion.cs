using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    public float speed = 2f;
    public float turnDegrees = 30f;
    public float turnDeadzone = 0.5f;
    public Transform head;

    Vector2 curMove;
    Vector2 curTurn;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        InputHandler.instance.onMove += OnMove;
        InputHandler.instance.onTurn += OnTurn;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDelta = head.forward * curMove.y + head.right * curMove.x;
        float amt = moveDelta.magnitude;
        moveDelta.y = 0f;
        moveDelta = moveDelta.normalized * amt * Time.deltaTime * speed;
        if (rb)
        {
            rb.MovePosition(transform.position + moveDelta);
        }
        else
        {
            transform.position += moveDelta;
        }
    }

    void OnMove(Vector2 newMove)
    {
        curMove = newMove;
    }

    void OnTurn(Vector2 newTurn)
    {
        if (Mathf.Abs(curTurn.x) <= turnDeadzone && Mathf.Abs(newTurn.x) > turnDeadzone)
        {
            Vector3 headPos = head.position;
            Vector3 newEuler = transform.localEulerAngles;
            newEuler.y += Mathf.Sign(newTurn.x) * 30f;
            transform.localEulerAngles = newEuler;
            Vector3 newHeadPos = head.position;

            Vector3 displacement = headPos - newHeadPos;
            transform.position += displacement;
        }
        curTurn = newTurn;
    }
}
