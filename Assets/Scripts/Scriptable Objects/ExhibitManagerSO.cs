using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ExhibitManager", menuName = "ExhibitCreator/Managers/Exhibit Manager")]
public class ExhibitManagerSO : ScriptableObject
{
    public UnityAction<Transform, Transform> onGrab;
    public UnityAction<Transform> onRelease;

    public UnityAction<Transform> updateMainObj;

    public void InvokeOnGrab(Transform obj, Transform holder)
    {
        onGrab?.Invoke(obj, holder);
    }

    public void InvokeOnRelease(Transform holder)
    {
        onRelease?.Invoke(holder);
    }

    public void InvokeUpdateMainObj(Transform mainObj)
    {
        updateMainObj?.Invoke(mainObj);
    }
}
