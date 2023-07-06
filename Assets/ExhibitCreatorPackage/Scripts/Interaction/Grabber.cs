using UnityEngine;

public class Grabber : MonoBehaviour
{
    public ExhibitManagerSO exhibitManager;

    public bool debug;
    public GrabLogic debugObj;
    public Transform debugHolder1;
    public Transform debugHolder2;

    // The main object can have 1 or 2 holders (assuming people have a max of 2 hands)
    private GrabLogic grabbedObjectMain;
    private Transform holderMain1;
    private Transform holderMain2;

    // Since alt only used if grabbed with a second hand, it hsould never have 2 holders
    private GrabLogic grabbedObjectAlt;
    private Transform holderAlt;

    // Start is called before the first frame update
    void Start()
    {
        if (debug && debugObj)
        {
            if (debugHolder1)
            {
                debugObj.AddHolder(debugHolder1);
            }
            if (debugHolder2)
            {
                debugObj.AddHolder(debugHolder2);
            }
        }

        exhibitManager.onGrab += onGrab;
        exhibitManager.onRelease += onRelease;
    }

    /// <summary>
    /// Handler for when a holder grabs an object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="holder"></param>
    void onGrab(GrabLogic obj, Transform holder)
    {
        if (!obj || !holder) return;

        // if we're grabbing same object with other hand, add as holder2
        if (obj == grabbedObjectMain)
        {
            obj.AddHolder(holder);
            return;
        }

        // otherwise if we're not grabbing anything add as main
        if (grabbedObjectMain == null)
        {
            grabbedObjectMain = obj;
            obj.AddHolder(holder);
            exhibitManager.InvokeUpdateMainObj(obj);
            return;
        }

        // final case is we add it as a secondary grabbed object
        if (grabbedObjectAlt == null)
        {
            grabbedObjectAlt = obj;
            obj.AddHolder(holder);
            return;
        }
    }

    /// <summary>
    /// Handler for when a controller releases
    /// </summary>
    /// <param name="holder"></param>
    void onRelease(Transform holder)
    {
        if (!holder) return;

        if (grabbedObjectMain && grabbedObjectMain.HasHolder(holder))
        {
            grabbedObjectMain.RemoveHolder(holder);
            if (!grabbedObjectMain.IsBeingHeld())
            {
                grabbedObjectMain = null;
                if (grabbedObjectAlt)
                {
                    grabbedObjectMain = grabbedObjectAlt;
                    grabbedObjectAlt = null;
                    exhibitManager.InvokeUpdateMainObj(grabbedObjectMain);
                }
            }
        }
        else if (grabbedObjectAlt && grabbedObjectAlt.HasHolder(holder))
        {
            grabbedObjectAlt.RemoveHolder(holder);
            if (!grabbedObjectAlt.IsBeingHeld())
            {
                grabbedObjectAlt = null;
            }
        }
    }
}
