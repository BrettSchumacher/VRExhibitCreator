using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLogic : MonoBehaviour
{
    /// <summary>
    /// Adds a holder to the object
    /// </summary>
    /// <param name="holder"></param> the added holder
    public virtual void AddHolder(Transform holder) { }

    /// <summary>
    /// Removes a holder from the object
    /// </summary>
    /// <param name="holder"></param> the removed holder
    public virtual void RemoveHolder(Transform holder) { }

    /// <summary>
    /// Removes all holders
    /// </summary>
    public virtual void ClearHolders() { }

    /// <summary>
    /// Returns if the object is being held by holder
    /// </summary>
    /// <param name="holder"> the holder we are interested in </param>
    /// <returns></returns>
    public virtual bool HasHolder(Transform holder) { return false; }

    /// <summary>
    /// Returns if the object is currently being held
    /// </summary>
    /// <returns></returns>
    public virtual bool IsBeingHeld() { return false; }
}
