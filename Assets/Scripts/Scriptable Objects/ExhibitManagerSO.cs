using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ExhibitManager", menuName = "ExhibitCreator/Managers/Exhibit Manager")]
public class ExhibitManagerSO : ScriptableObject
{
    public UnityAction<GrabLogic, Transform> onGrab;
    public UnityAction<Transform> onRelease;
    public UnityAction onNextSlide;
    public UnityAction onBackSlide;

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
    }

    public void InvokeOnNextSlide() { onNextSlide?.Invoke(); }
    public void InvokeOnBackSlide() { onBackSlide?.Invoke(); }

}
