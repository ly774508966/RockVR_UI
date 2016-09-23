using UnityEngine;

namespace RockVR.SDK
{
    [RequireComponent(typeof(Collider), typeof(SteamVR_TrackedObject))]
    public class RVItemController : MonoBehaviour
    {
        private SteamVR_Controller.Device m_Controller
        {
            get
            {
                return SteamVR_Controller.Input((int)m_TrackedObj.index);
            }
        }
        private SteamVR_TrackedObject m_TrackedObj;
        private RVInteractiveItem m_CurrentHolding;
        private RV2DCamera m_Current2DCamera;

        private void Awake()
        {
            m_TrackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            RVInteractiveItem item = collider.GetComponent<RVInteractiveItem>();
            if (item && item.InteractiveItemType == RVInteractiveItemType._2DCameraItem)
            {
                RV2DCamera _2DCameraItem = collider.GetComponent<RV2DCamera>();
                if (!_2DCameraItem.Enabled())
                {
                    _2DCameraItem.EnableCamera();
                }
                if (m_CurrentHolding == null)
                {
                    _2DCameraItem.gameObject.transform.SetParent(this.transform, true);
                    _2DCameraItem.gameObject.transform.localPosition = new Vector3(0.0f, 0.11f, 0.11f);
                    _2DCameraItem.gameObject.transform.localRotation = Quaternion.identity;

                    m_CurrentHolding = _2DCameraItem;
                }
                m_Current2DCamera = _2DCameraItem;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
        }

        private void Update()
        {
            if (m_Controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) && m_Current2DCamera != null)
            {
                if (m_Current2DCamera.Capturing())
                {
                    m_Current2DCamera.FinishCapture();
                }
                else
                {
                    m_Current2DCamera.StartCapture();
                }
            }
            if (m_Controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) && m_CurrentHolding != null)
            {
                m_CurrentHolding.gameObject.transform.SetParent(null, true);
                m_CurrentHolding = null;
            }
        }
    }
}