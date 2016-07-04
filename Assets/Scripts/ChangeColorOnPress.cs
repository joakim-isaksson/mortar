using UnityEngine;
using System.Collections;

public class ChangeColorOnPress : MonoBehaviour {

    public Color defaultColor;
    public Color activeColor;

    SteamVR_TrackedObject trackedObject;
    Material material;

    void Awake()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        material = GetComponent<MeshRenderer>().material;
        material.color = defaultColor;
    }

	void Update () {
        if (trackedObject.index == SteamVR_TrackedObject.EIndex.None) return;
        int index = (int)trackedObject.index;

        if (SteamVR_Controller.Input(index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            SteamVR_Controller.Input(index).TriggerHapticPulse(1000);
            material.color = activeColor;
        }
        else if (SteamVR_Controller.Input(index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            SteamVR_Controller.Input(index).TriggerHapticPulse(1000);
            material.color = defaultColor;
        }
    }
}
