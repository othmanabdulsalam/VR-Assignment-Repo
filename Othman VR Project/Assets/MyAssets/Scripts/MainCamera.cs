using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Portal[] portals;
    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
    }

    void OnPreCull()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender();
        }
        for (int i = 0; i < portals.Length; i++)
        {

            portals[i].Render(); // handle rendering portal view
        }

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender(); // handle post rendering tasks such as clipping protection
        }
    }
}
