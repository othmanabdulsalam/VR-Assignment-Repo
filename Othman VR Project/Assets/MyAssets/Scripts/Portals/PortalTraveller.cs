using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public Vector3 previousOffsetFromPortal { get; set; }


    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        // push the y position up by
        transform.rotation = rot;
    }



    // Called when first touches portal
    public virtual void EnterPortalThreshold()
    {
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold()
    {
    }
}

