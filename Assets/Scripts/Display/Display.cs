using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public DisplayManagerSO displayManager;
    public Transform displayCanvasParent;

    Artifact curArtifact;
    GameObject curCanvas;
    SlideShow slideShow;

    Artifact prevArt;

    // Start is called before the first frame update
    void Start()
    {
        displayManager.Awake();
        displayManager.updateDisplay += UpdateDisplay;
        displayManager.goNextSlide += NextSlide;
        displayManager.goBackSlide += BackSlide;
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
            slideShow = null;
        }
    }

    /// <summary>
    /// Event Handler for updating a display with a new artifact, or to clear display if artifact is null
    /// </summary>
    /// <param name="art"> The new artifact to display (can be null) </param>
    void UpdateDisplay(Artifact art)
    {
        if (art == prevArt) return;
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
        slideShow = curCanvas.GetComponent<SlideShow>();
        prevArt = art;
    }

    void NextSlide()
    {
        if (!slideShow || slideShow.IsComplete()) return;

        slideShow.NextSlide();
    }

    void BackSlide()
    {
        if (!slideShow) return;

        slideShow.BackSlide();
    }
}
