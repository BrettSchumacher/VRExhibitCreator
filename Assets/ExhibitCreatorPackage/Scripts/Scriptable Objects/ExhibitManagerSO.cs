using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ExhibitManager", menuName = "ExhibitCreator/Managers/Exhibit Manager")]
public class ExhibitManagerSO : ScriptableObject
{
    public UnityAction<GrabLogic, Transform> onGrab;
    public UnityAction<Transform> onRelease;
    public UnityAction<Artifact> updateDisplay;
    public UnityAction onMainInput;
    public UnityAction onSecondaryInput;

    public UnityAction<Transform> updateMainObj;

    public void InvokeOnGrab(GrabLogic obj, Transform holder)
    {
        onGrab?.Invoke(obj, holder);
    }

    public void InvokeOnRelease(Transform holder)
    {
        onRelease?.Invoke(holder);
    }

    public void InvokeUpdateMainObj(GrabLogic mainObj)
    {
        if (!mainObj) return;
        updateMainObj?.Invoke(mainObj.transform);

        Artifact art = mainObj.GetComponent<Artifact>();
        if (art != null)
        {
            updateDisplay?.Invoke(art);
        }
    }

    public void InvokeUpdateDisplay(Artifact art)
    {
        updateDisplay?.Invoke(art);
    }

    public void InvokeOnMainInput() { onMainInput?.Invoke(); }
    public void InvokeOnSecondaryInput() { onSecondaryInput?.Invoke(); }

}
