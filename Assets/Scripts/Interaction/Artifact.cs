using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    public ArtifactInfoSO artifactInfo;
    public ExhibitManagerSO exhibitManager;

    private void Start()
    {
        artifactInfo.onDisplay += OnDisplay;
        artifactInfo.offDisplay += OffDisplay;
    }

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
}
