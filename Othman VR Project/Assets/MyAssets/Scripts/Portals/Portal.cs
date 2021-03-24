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
    public void Render()
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
        // uncomment this line when testing visibility of portals
        //linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        screen.enabled = false;
        CreateViewTexture();
        // changes the position of the portal camera
        var m = transform.localToWorldMatrix 
            * linkedPortal.transform.worldToLocalMatrix 
            * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        // handle near clipping for portal camera's
        SetNearClipPlane();
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

    // Called once all portals have been rendered, but before the player camera renders
    public void PostPortalRender()
    {
        //foreach (var traveller in trackedTravellers)
        //{
        //    UpdateSliceParams(traveller);
        //}
        ProtectScreenFromClipping();
    }

    // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
    void ProtectScreenFromClipping()
    {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;

        // set optimal screen thickness based on player camera's nearclip plane
        float screenThickness = new Vector3(halfWidth,halfHeight,playerCam.nearClipPlane).magnitude;

        Transform screenT = screen.transform;
        bool isCamFacingPortalDirection = Vector3.Dot(transform.forward,transform.position-playerCam.transform.position) > 0;

        //float pushValue = 0.01f;
        float pushValue = 0.01f;

        // change the scaleZ to be the new thickness
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
        // push the screen back to remove flicker effect
        screenT.localPosition = Vector3.forward * screenThickness * (isCamFacingPortalDirection ? pushValue : -pushValue);
    }

    // Function for setting the portal camera's near clip plane to avoid objects obstructing its view
    // Technique is called Oblique projection and was found from http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
    void SetNearClipPlane()
    {
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward)*dot;

        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x,camSpaceNormal.y,camSpaceNormal.z,camSpaceDst);

        portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
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
