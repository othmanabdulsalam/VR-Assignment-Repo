using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")]
    public Portal linkedPortal;
    public MeshRenderer screen;


    //[Header("Testing Settings")]
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;
    List<PortalTraveller> trackedTravellers;

    void Awake()
    {
        // get the main camera that the player see's the world through
        playerCam = Camera.main; 
        // get the camera that is attached to the portal child camera object
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false; // disabled on awake for manipulation
        trackedTravellers = new List<PortalTraveller>();
        screen.material.SetInt("displayMask", 1);

    }

    private void Update()
    {
        Render();
    }

    void FixedUpdate()
    {
        HandleTravellers();
    }

    void HandleTravellers() 
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            // set traveller and traveller transform
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            

            // calculate offset from portal from the traveller transform to the portals transform
            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

            // Teleport Traveller if it has crossed one side of the portal to the other
            if (portalSide != portalSideOld) // check the portal side isnt the old side
            {
                Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;
                // traveller can teleport
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                

                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;
            }
            else
            {
                // if portal side is same as old portal side set previous offset equal to offset
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
            {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 1);
            // Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            // Display the view texture on the screen of the linked portal
            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    // Manually render the camera attached to this portal
    void Render()
    {
        // check visibility of linked portal, if not visibile then skip render
        if(!VisibleFromCamera(linkedPortal.screen,playerCam))
        {
            //var testTexture = new Texture2D(1, 1);
            //testTexture.SetPixel(0, 0,Color.red);
            //testTexture.Apply();
            //linkedPortal.screen.material.SetTexture("_MainTex", testTexture);
            return;
        }

        //linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        screen.enabled = false;
        CreateViewTexture();
        // changes the position of the portal camera
        var m = transform.localToWorldMatrix 
            * linkedPortal.transform.worldToLocalMatrix 
            * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        // Render the Camera
        portalCam.Render();
        // re-enable screen
        screen.enabled = true;
    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        bool isVisible = GeometryUtility.TestPlanesAABB(frustrumPlanes,renderer.bounds);
        return isVisible;
    }



    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }


    /*
     ** Some helper/convenience stuff:
     */

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }

    Vector3 portalCamPos
    {
        get
        {
            return portalCam.transform.position;
        }
    }

    void OnValidate()
    {
        if (linkedPortal != null)
        {
            linkedPortal.linkedPortal = this;
        }
    }
}
