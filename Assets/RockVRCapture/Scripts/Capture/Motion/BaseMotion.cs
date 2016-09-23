using UnityEngine;
using System.Collections;

namespace RockVR.Capture {

    public abstract class BaseMotion : MonoBehaviour {

        public abstract string SaveFileName();
    }

    public enum MoveDirection {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }
}