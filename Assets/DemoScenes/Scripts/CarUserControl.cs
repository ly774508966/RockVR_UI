using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
using RockVR.Replay;

/// <summary>
/// Car user control.
/// Same as CarUserControl with Unity Standard Assets.
/// To avoid complie order issue here.
/// see: http://docs.unity3d.com/Manual/ScriptCompileOrderFolders.html
/// </summary>
namespace RockVR.Demo
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public RVReplayInput RVInput;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            if (RVInput == null) {
                throw new MissingComponentException ("RVInput is not setup!");
            }
        }

        private void FixedUpdate()
        {
            // pass the input to the car!
//            float h = CrossPlatformInputManager.GetAxis("Horizontal");
//            float v = CrossPlatformInputManager.GetAxis("Vertical");
            float h = RVInput.Standalone.GetAxis("Horizontal");
            float v = RVInput.Standalone.GetAxis ("Vertical");
            #if !MOBILE_INPUT
//            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            float handbrake = RVInput.Standalone.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
            #else
            m_Car.Move(h, v, v, 0f);
            #endif
        }
    }
}
