using UnityEngine;

public class SpindleGrabLogic : GrabLogic
{
    // The main object can have 1 or 2 holders (assuming people have a max of 2 hands)
    private Transform holder1;
    private Transform holder2;

    private Vector3 grabbedOffset;
    private Quaternion rotationOffset;

    private Vector3 startLocalScale;
    private Vector3 startGlobalDisplacement;
    private Vector3 startSpindle;
    private Vector3 startCenter;

    Vector3 lastFrom;
    Vector3 lastTo;

    Rigidbody rb;
    bool wasKinematic;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wasKinematic = rb ? rb.isKinematic : false;
    }

    void Update()
    {
        if (holder1 && holder2)
        {
            SpindleUpdate();
        }
        else if (holder1)
        {
            transform.rotation = holder1.rotation * rotationOffset;
            transform.position = holder1.TransformPoint(grabbedOffset);
        }

    }

    void SpindleUpdate()
    {
        Vector3 newSpindle = holder2.position - holder1.position;

        // first handle scaling object
        float scaleFac = newSpindle.magnitude / startGlobalDisplacement.magnitude;

        transform.localScale = startLocalScale * scaleFac;

        // then align the spindles
        Vector3 curSpinUnit = newSpindle.normalized;
        Vector3 lastSpindleUnit = transform.TransformVector(startSpindle).normalized;

        transform.rotation = Quaternion.FromToRotation(lastSpindleUnit, curSpinUnit) * transform.rotation;

        //finally align the centers
        Vector3 lastCenter = transform.TransformPoint(startCenter);
        Vector3 spindleCenter = (holder1.position + holder2.position) * 0.5f;

        transform.position += spindleCenter - lastCenter;
    }

    public override void AddHolder(Transform holder)
    {
        if (!holder1)
        {
            holder1 = holder;
            grabbedOffset = holder.InverseTransformPoint(transform.position);
            rotationOffset = Quaternion.Inverse(holder.rotation) * transform.rotation;
            if (rb) rb.isKinematic = true;
        }
        else if (!holder2)
        {
            holder2 = holder;
            startGlobalDisplacement = holder2.position - holder1.position;
            startCenter = transform.InverseTransformPoint(0.5f * (holder1.position + holder2.position));
            startLocalScale = transform.localScale;
            startSpindle = transform.InverseTransformVector(startGlobalDisplacement).normalized;
        }
    }

    public override void RemoveHolder(Transform holder)
    {
        if (holder == holder2)
        {
            holder2 = null;
            grabbedOffset = holder1.InverseTransformPoint(transform.position);
            rotationOffset = Quaternion.Inverse(holder1.rotation) * transform.rotation;
        }
        else if (holder == holder1)
        {
            holder1 = null;
            if (holder2)
            {
                holder1 = holder2;
                holder2 = null;
                grabbedOffset = holder1.InverseTransformPoint(transform.position);
                rotationOffset = Quaternion.Inverse(holder1.rotation) * transform.rotation;
            }
            else if (rb)
            {
                rb.isKinematic = wasKinematic;
            }
        }
    }

    public override void ClearHolders()
    {
        holder1 = null;
        holder2 = null;
    }

    public override bool HasHolder(Transform holder)
    {
        return (holder == holder1) || (holder == holder2);
    }

    public override bool IsBeingHeld()
    {
        return (holder1 || holder2);
    }

    private void OnDrawGizmos()
    {
        if (!holder1 || !holder2) return;

        Vector3 center = transform.TransformPoint(startCenter);
        Vector3 disp = transform.TransformVector(startSpindle);
        Vector3 h1 = center + 0.5f * disp;
        Vector3 h2 = center - 0.5f * disp;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(h1, 0.02f);
        Gizmos.DrawSphere(h2, 0.02f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastFrom);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + lastTo);
    }
}
