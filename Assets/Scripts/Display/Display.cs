using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public DisplayManagerSO displayManager;
    public Transform displayCanvasParent;

    ArtifactInfoSO curArtifact;
    GameObject curCanvas;

    // Start is called before the first frame update
    void Start()
    {
        displayManager.Awake();
        displayManager.updateDisplay += UpdateDisplay;
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
        }
    }

    /// <summary>
    /// Event Handler for updating a display with a new artifact, or to clear display if artifact is null
    /// </summary>
    /// <param name="art"> The new artifact to display (can be null) </param>
    void UpdateDisplay(ArtifactInfoSO art)
    {
        print("Updating display");
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
    }
}
