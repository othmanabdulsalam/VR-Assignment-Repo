using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")]
    public Portal linkedPortal;
    public MeshRenderer screen;


    [Header("Testing Settings")]
    Camera playerCam;
    Camera portalCam;
    [SerializeField] RenderTexture viewTexture;

    void Awake()
    {
        // get the main camera that the player see's the world through
        playerCam = Camera.main; 
        // get the camera that is attached to the portal child camera object
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false; // disabled on awake for manipulation

    }

    void FixedUpdate()
    {
        Render();
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






    void OnValidate()
    {
        if (linkedPortal != null)
        {
            linkedPortal.linkedPortal = this;
        }
    }
}
