using UnityEngine;

namespace RockVR.SDK
{
    [RequireComponent(typeof(Collider))]
    public class RVInteractiveItem : MonoBehaviour
    {
        protected RVInteractiveItemType m_InteractiveItemType;

        public RVInteractiveItemType InteractiveItemType
        {
            get
            {
                return m_InteractiveItemType;
            }
        }
    }

    public enum RVInteractiveItemType
    {
        _2DCameraItem,
        _360CameraItem,
    }
}
