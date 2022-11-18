using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DisplayManager", menuName = "ExhibitCreator/Managers/Display Manager")]
public class DisplayManagerSO : ScriptableObject
{
    public ExhibitManagerSO exhibitManager;

    public UnityAction<Artifact> updateDisplay;
    public UnityAction goNextSlide;
    public UnityAction goBackSlide;

    bool addedToExhibit = false;

    public void Awake()
    {
        Debug.Log("Adding handler to exhibit manager");
        if (exhibitManager)
        {
            exhibitManager.updateMainObj += handleUpdateMainObj;
            exhibitManager.onNextSlide += handleOnNext;
            exhibitManager.onBackSlide += handleOnBack;
            addedToExhibit = true;
        }
        else
        {
            addedToExhibit = false;
        }
    }

    /// <summary>
    /// Event handler for when user's main object is changed, sends info out for exhibit displays to use
    /// </summary>
    /// <param name="obj"></param>
    void handleUpdateMainObj(Transform obj)
    {
        // will invoke updateDisplay with new artifact info or null if display should be cleared
        updateDisplay?.Invoke(obj.GetComponent<Artifact>());
    }

    void handleOnNext()
    {
        goNextSlide?.Invoke();
    }

    void handleOnBack()
    {
        goBackSlide?.Invoke();
    }
}
