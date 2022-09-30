using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DisplayManager", menuName = "ExhibitCreator/Display Manager")]

public class DisplayManagerSO : ScriptableObject
{
    public ExhibitManagerSO exhibitManager;

    public UnityAction<ArtifactInfoSO> updateDisplay;

    public void Awake()
    {
        exhibitManager.updateMainObj += handleUpdateMainObj;
    }

    /// <summary>
    /// Event handler for when user's main object is changed, sends info out for exhibit displays to use
    /// </summary>
    /// <param name="obj"></param>
    void handleUpdateMainObj(Transform obj)
    {
        // will invoke updateDisplay with new artifact info or null if display should be cleared
        updateDisplay?.Invoke(obj.GetComponent<Artifact>()?.artifactInfo);
    }
}
