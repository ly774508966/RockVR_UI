using UnityEngine;
using System.Collections;

namespace RockVR.Utils
{
    public class ObjectFaceAt : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_FacingObject;

        private void Awake()
        {
            if (m_FacingObject == null)
            {
                throw new MissingComponentException("FacingObject not attached!");
            }
        }

        private void Update()
        {
            this.gameObject.transform.LookAt(m_FacingObject.transform);
        }
    }
}