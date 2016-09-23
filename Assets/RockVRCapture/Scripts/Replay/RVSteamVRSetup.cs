using UnityEngine;
using System.Collections.Generic;

namespace RockVR.Replay
{
    [RequireComponent(typeof(RVReplayInput))]
    public class RVSteamVRSetup : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_CameraRig;

        private void Awake()
        {
            RVReplayInput RVInput = GetComponent<RVReplayInput>();
            if (RVInput.RecordSteamVR && RVInput.Mode == RVInputMode.Playback && RVInput.HasRecordPositionObject())
            {
                if (m_CameraRig == null)
                {
                    throw new MissingComponentException("CameraRig not attached in Steam VR replay mode!");
                }
                Transform[] childTransforms = m_CameraRig.GetComponentsInChildren<Transform>();
                // disable all Steam VR related
                foreach(Transform transform in childTransforms)
                {
                    DisableSteamVRRelatedComponent(transform.gameObject);
                }
                m_CameraRig.SetActive(false);
                Debug.Log("Steam VR setup finish");
            }
        }

        private void DisableSteamVRRelatedComponent(GameObject gameObject)
        {
            if (gameObject.GetComponent<Camera>() != null)
                gameObject.GetComponent<Camera>().enabled = false;
            if (gameObject.GetComponent<AudioListener>() != null)
                gameObject.GetComponent<AudioListener>().enabled = false;
            if (gameObject.GetComponent<SteamVR_ControllerManager>() != null)
                gameObject.GetComponent<SteamVR_ControllerManager>().enabled = false;
            if (gameObject.GetComponent<SteamVR_PlayArea>() != null)
                gameObject.GetComponent<SteamVR_PlayArea>().enabled = false;
            if (gameObject.GetComponent<SteamVR_TrackedObject>() != null)
                gameObject.GetComponent<SteamVR_TrackedObject>().enabled = false;
            if (gameObject.GetComponent<SteamVR_RenderModel>() != null)
                gameObject.GetComponent<SteamVR_RenderModel>().enabled = false;
            if (gameObject.GetComponent<SteamVR_GameView>() != null)
                gameObject.GetComponent<SteamVR_GameView>().enabled = false;
            if (gameObject.GetComponent<SteamVR_CameraFlip>() != null)
                gameObject.GetComponent<SteamVR_CameraFlip>().enabled = false;
            if (gameObject.GetComponent<SteamVR_Camera>() != null)
                gameObject.GetComponent<SteamVR_Camera>().enabled = false;
            if (gameObject.GetComponent<SteamVR_Ears>() != null)
                gameObject.GetComponent<SteamVR_Ears>().enabled = false;
        }
    }
}