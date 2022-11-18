using UnityEngine;

public class SpindleGrabLogic : GrabLogic
{
    // The main object can have 1 or 2 holders (assuming people have a max of 2 hands)
    private Transform holder1;
    private Transform holder2;

    private Vector3 grabbedOffset;
    private Quaternion rotationOffset;

    private Vector3 startLocalScale;
    private Vector3 startDisplacement;
    private Vector3 startLocalHolder1;
    private Vector3 startLocalDisplacment;
    private Vector3 startLocalOffset;
    private Vector3 startSpindUnit;
    private Vector3 startPlaneUnit;
    private Vector3 startNormUnit;

    float offsetAlongSpindle;
    float offsetAlongNorm;

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
        Vector3 displacement = holder2.position - holder1.position;

        // first handle scaling object
        float scaleFac = displacement.magnitude / startDisplacement.magnitude;
        print("scale fac: " + scaleFac);
        transform.localScale = startLocalScale * scaleFac;

        Vector3 center = (holder1.position + holder2.position) * 0.5f;
        Vector3 offset = transform.position - center;
        Vector3 spinUnit = displacement.normalized;
        Vector3 planeUnit = Vector3.Cross(spinUnit, offset).normalized;
        Vector3 normUnit = Vector3.Cross(planeUnit, spinUnit).normalized;
        Vector3 newOff = (spinUnit * offsetAlongSpindle + normUnit * offsetAlongNorm) * scaleFac;
        transform.position = center + newOff;

        Vector3 curOff = transform.TransformVector(startLocalOffset);

        lastFrom = -curOff;
        lastTo = -newOff;
        transform.rotation = Quaternion.FromToRotation(-curOff, -newOff) * transform.rotation;
        Vector3 curPlaneUnit = transform.TransformVector(startPlaneUnit).normalized;

        transform.rotation = Quaternion.FromToRotation(curPlaneUnit, planeUnit) * transform.rotation;

        return;
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
            startDisplacement = holder2.position - holder1.position;
            startLocalHolder1 = transform.InverseTransformPoint(holder1.position);
            startLocalDisplacment = transform.InverseTransformVector(startDisplacement);
            startLocalScale = transform.localScale;

            Vector3 center = startLocalHolder1 + 0.5f * startLocalDisplacment;
            startLocalOffset = -center;
            startSpindUnit = startLocalDisplacment.normalized;
            startPlaneUnit = Vector3.Cross(startSpindUnit, startLocalOffset);
            startNormUnit = Vector3.Cross(startPlaneUnit, startSpindUnit).normalized;

            Vector3 scaleVec = transform.lossyScale;
            offsetAlongSpindle = Vector3.Dot(startSpindUnit, startLocalOffset) * Vector3.Dot(Vector3.Scale(startSpindUnit, startSpindUnit), scaleVec);
            offsetAlongNorm = Vector3.Dot(startNormUnit, startLocalOffset) * Vector3.Dot(Vector3.Scale(startNormUnit, startNormUnit), scaleVec);

            print("along spin: " + offsetAlongSpindle + ", along norm: " + offsetAlongNorm);
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

        Vector3 h1 = transform.TransformPoint(startLocalHolder1);
        Vector3 h2 = h1 + transform.TransformVector(startLocalDisplacment);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(h1, 0.02f);
        Gizmos.DrawSphere(h2, 0.02f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastFrom);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + lastTo);
    }
}
