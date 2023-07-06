using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Artifact : MonoBehaviour
{
    public ExhibitManagerSO exhibitManager;
    public GameObject canvasPrefab;
    public UnityAction onDisplay;
    public UnityAction offDisplay;

    GrabLogic grabLogic;

    private void Start()
    {
        onDisplay += OnDisplay;
        offDisplay += OffDisplay;

        grabLogic = GetComponent<GrabLogic>();
        if (!grabLogic)
        {
            Debug.LogWarning("No grab logic found for artifact '" + name + "', using default Spindle logic");
            grabLogic = gameObject.AddComponent<SpindleGrabLogic>();
        }
    }

    public GrabLogic GetGrabLogic() { return grabLogic; }

    /// <summary>
    /// Event Handler invoked when artifact is added to a display
    /// </summary>
    void OnDisplay()
    {

    }

    /// <summary>
    /// Event Handler invoked when artifact is taken off a display
    /// </summary>
    void OffDisplay()
    {

    }

    public void InvokeOnDisplay() { onDisplay?.Invoke(); }
    public void InvokeOffDisplay() { offDisplay?.Invoke(); }
}
