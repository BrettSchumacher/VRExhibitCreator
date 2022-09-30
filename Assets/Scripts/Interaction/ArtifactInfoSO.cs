using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Artifact", menuName = "ExhibitCreator/Objects/Artifact")]
public class ArtifactInfoSO : ScriptableObject
{
    public GameObject canvasPrefab;

    public UnityAction onDisplay;
    public UnityAction offDisplay;

    public void InvokeOnDisplay()
    {
        onDisplay?.Invoke();
    }

    public void InvokeOffDisplay()
    {
        offDisplay?.Invoke();
    }
}
