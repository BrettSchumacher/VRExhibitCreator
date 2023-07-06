using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public ExhibitManagerSO manager;
    public Transform displayCanvasParent;

    Artifact curArtifact;
    GameObject curCanvas;
    DisplayBehavior curBehavior;

    Artifact prevArt;

    // Start is called before the first frame update
    void Start()
    {
        manager.updateDisplay += UpdateDisplay;
        manager.onMainInput += OnMainInput;
        manager.onSecondaryInput += OnSecondaryInput;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Helper function for clearing the display
    /// </summary>
    void ClearDisplay()
    {
        if (curCanvas)
        {
            curArtifact.InvokeOffDisplay();

            Destroy(curCanvas);

            curCanvas = null;
            curArtifact = null;
            curBehavior = null;
        }
    }

    /// <summary>
    /// Event Handler for updating a display with a new artifact, or to clear display if artifact is null
    /// </summary>
    /// <param name="art"> The new artifact to display (can be null) </param>
    void UpdateDisplay(Artifact art)
    {
        if (art == prevArt) return;

        // start by clearing display
        ClearDisplay();
        
        if (!art)
        {
            return;
        }

        // if there's an artifact to display, then display it
        art.InvokeOnDisplay();
        curArtifact = art;
        curCanvas = Instantiate(art.canvasPrefab, displayCanvasParent);
        curBehavior = curCanvas.GetComponent<DisplayBehavior>();
        prevArt = art;
    }

    void OnMainInput()
    {
        if (!curBehavior) return;

        curBehavior.OnMainInput();
    }

    void OnSecondaryInput()
    {
        if (!curBehavior) return;

        curBehavior.OnSecondaryInput();
    }
}
