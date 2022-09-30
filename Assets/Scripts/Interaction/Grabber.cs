using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public ExhibitManagerSO exhibitManager;

    public bool debug;
    public Transform debugObj;
    public Transform debugHolder1;
    public Transform debugHolder2;

    // The main object can have 1 or 2 holders (assuming people have a max of 2 hands)
    private Transform grabbedObjectMain;
    private Transform holderMain1;
    private Transform holderMain2;

    // Since alt only used if grabbed with a second hand, it hsould never have 2 holders
    private Transform grabbedObjectAlt;
    private Transform holderAlt;

    private Vector3 grabbedOffsetMain;
    private Quaternion rotOffsetMain;
    private Vector3 holderDisplacement;
    private Vector3 holdersCenter;

    private Vector3 grabbedOffsetAlt;
    private Quaternion rotOffsetAlt;

    // Start is called before the first frame update
    void Start()
    {
        if (debug)
        {
            if (debugObj)
            {
                GrabObjectMain(debugObj, debugHolder1);
            }
            if (debugHolder2)
            {
                AddHolderMain(debugHolder2);
            }
        }

        exhibitManager.onGrab += onGrab;
        exhibitManager.onRelease += onRelease;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbedObjectMain != null)
        {
            if (holderMain2 == null)
            {
                // simple case
                grabbedObjectMain.rotation = holderMain1.rotation * rotOffsetMain;
                grabbedObjectMain.position = holderMain1.TransformPoint(grabbedOffsetMain);
            }
            else
            {
                Vector3 newDisplacement = holderMain1.position - holderMain2.position;

                // first handle scaling object
                float scale = newDisplacement.magnitude / holderDisplacement.magnitude;
                grabbedObjectMain.localScale *= scale;

                // now handle rotation
                // adapted from https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
                Quaternion rot;
                Vector3 v1 = holderDisplacement.normalized;
                Vector3 v2 = newDisplacement.normalized;
                Vector3 cross = Vector3.Cross(v1, v2);
                if (cross.sqrMagnitude > 0f)
                {
                    rot.x = cross.x;
                    rot.y = cross.y;
                    rot.z = cross.z;
                    rot.w = 1 + Vector3.Dot(v1, v2);
                    rot.Normalize();
                    grabbedObjectMain.rotation *= rot;
                }

                // now handle offset
                Vector3 newCenter = (holderMain1.position + holderMain2.position) / 2f;
                grabbedObjectMain.position += newCenter - holdersCenter;

                // update variables
                holderDisplacement = newDisplacement;
                holdersCenter = newCenter;
            }
        }

        if (grabbedObjectAlt != null)
        {
            grabbedObjectAlt.rotation = holderAlt.rotation * rotOffsetAlt;
            grabbedObjectAlt.position = holderAlt.TransformPoint(grabbedOffsetAlt);
        }
    }

    /// <summary>
    /// Sets an object as the main grabbed obj with a main holder
    /// </summary>
    /// <param name="obj"></param> Object to grab
    /// <param name="holder"></param> Main holder
    void GrabObjectMain(Transform obj, Transform holder)
    {
        grabbedObjectMain = obj;
        holderMain1 = holder;
        holderMain2 = null;

        grabbedOffsetMain = holder.InverseTransformPoint(obj.position);
        rotOffsetMain = Quaternion.Inverse(holder.rotation) * obj.rotation;

        exhibitManager.InvokeUpdateMainObj(obj);
    }

    /// <summary>
    /// Grab a second object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="holder"></param>
    void GrabObjectAlt(Transform obj, Transform holder)
    {
        grabbedObjectAlt = obj;
        holderAlt = holder;

        grabbedOffsetAlt = holder.InverseTransformPoint(obj.position);
        rotOffsetAlt = Quaternion.Inverse(holder.rotation) * obj.rotation;
    }

    /// <summary>
    /// Adds a second holder to main grabbed object
    /// </summary>
    /// <param name="holder"></param> 
    void AddHolderMain(Transform holder)
    {
        holderMain2 = holder;
        holderDisplacement = holderMain1.position - holder.position;
        holdersCenter = (holderMain1.position + holder.position) / 2f;
    }

    /// <summary>
    /// Transfers main object form holder2 to holder1
    /// </summary>
    void TransferHoldersMain()
    {
        grabbedOffsetMain = holderMain2.InverseTransformPoint(grabbedObjectMain.position);
        rotOffsetMain = Quaternion.Inverse(holderMain2.rotation) * grabbedObjectMain.rotation;

        holderMain1 = holderMain2;
        holderMain2 = null;
    }

    /// <summary>
    /// Transfers alt grabbed object to main grabbed object
    /// </summary>
    void TransferGrabbedObj()
    {
        GrabObjectMain(grabbedObjectAlt, holderAlt);

        grabbedObjectAlt = null;
        holderAlt = null;
    }

    /// <summary>
    /// Clears all variables in Grabber
    /// </summary>
    void Clear()
    {
        grabbedObjectMain = null;
        holderMain1 = null;
        holderMain2 = null;

        grabbedObjectAlt = null;
        holderAlt = null;

        exhibitManager.InvokeUpdateMainObj(null);
    }

    /// <summary>
    /// Removes holder2 from main grabbed object
    /// </summary>
    void ReleaseSecondHolderMain()
    {
        holderMain1 = holderMain2;
        holderMain2 = null;

        grabbedOffsetMain = holderMain1.InverseTransformPoint(position: grabbedObjectMain.position);
        rotOffsetMain = Quaternion.Inverse(holderMain1.rotation) * grabbedObjectMain.rotation;
    }

    /// <summary>
    /// Release alternate object hold
    /// </summary>
    void ReleaseAltObject()
    {
        holderAlt = null;
        grabbedObjectAlt = null;
    }

    /// <summary>
    /// Handler for when a holder grabs an object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="holder"></param>
    void onGrab(Transform obj, Transform holder)
    {
        // if we're grabbing same object with other hand, add as holder2
        if (obj == grabbedObjectMain)
        {
            AddHolderMain(holder);
            return;
        }

        // otherwise if we're not grabbing anything add as main
        if (grabbedObjectMain == null)
        {
            GrabObjectMain(obj, holder);
            return;
        }

        if (grabbedObjectAlt == null)
        {
            GrabObjectAlt(obj, holder);
            return;
        }
    }

    /// <summary>
    /// Handler for when a controller releases
    /// </summary>
    /// <param name="holder"></param>
    void onRelease(Transform holder)
    {
        if (holder == holderMain1)
        {
            if (holderMain2 != null)
            {
                TransferHoldersMain();
            }
            else if (grabbedObjectAlt != null)
            {
                TransferGrabbedObj();
            }
            else {
                Clear();
            }
        }
        else if (holder == holderMain2)
        {
            ReleaseSecondHolderMain();
        }
        else if (holder == holderAlt)
        {
            ReleaseAltObject();
        }
    }
}
