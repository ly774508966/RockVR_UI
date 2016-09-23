using UnityEngine;
using System.Collections;
using RockVR.Utils;

namespace RockVR.Capture {

    public class LinearMotion : BaseMotion {

        public MoveDirection m_MoveDirection = MoveDirection.Forward;
        public float m_MoveSpeed = 0.1f;

        private Vector3 m_DirectionVector = Vector3.forward;

        public void Awake() {
            if (m_MoveDirection == MoveDirection.Forward) {
                m_DirectionVector = Vector3.forward;
            } else if (m_MoveDirection == MoveDirection.Backward) {
                m_DirectionVector = Vector3.back;
            } else if (m_MoveDirection == MoveDirection.Up) {
                m_DirectionVector = Vector3.up;
            } else if (m_MoveDirection == MoveDirection.Down) {
                m_DirectionVector = Vector3.down;
            } else if (m_MoveDirection == MoveDirection.Left) {
                m_DirectionVector = Vector3.left;
            } else {
                m_DirectionVector = Vector3.right;
            }
        }
    	
        private void Update () {
            transform.position += m_DirectionVector * m_MoveSpeed;
    	}

        public override string SaveFileName() {
            return "LinearMotionFile";
        }
    }
}