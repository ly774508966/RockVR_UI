using UnityEngine;
using System;

namespace RockVR.Replay {

    /// <summary>
    /// Define RecordInputInfo interface.
    /// </summary>
    public interface IRecordInputInfo {
        RVInputMethod InputMethod();
        void MergeInputInfo(IRecordInputInfo inputInfo);
    }

    /// <summary>
    /// Base class for input info.
    /// </summary>
    public abstract class BaseInputInfo : IRecordInputInfo {
        private RVInputMethod m_InputMethod;

        public abstract RVInputMethod InputMethod();
        public abstract void MergeInputInfo(IRecordInputInfo inputInfo);
    }

    /// <summary>
    /// Input info for Standalone platform.
    /// </summary>
    [Serializable]
    public class StandaloneInputInfo: BaseInputInfo {
        private float m_AxisValue;

        public float AxisValue {
            get {
                return m_AxisValue;
            }
            set {
                m_AxisValue = value;
            }
        }

        public override RVInputMethod InputMethod() {
            return RVInputMethod.Standalone;
        }

        public override void MergeInputInfo(IRecordInputInfo inputInfo)
        {
            throw new UnityException("MergeInputInfo method not implemented!");
        }
    }

    /// <summary>
    /// Input info for Steam VR platform.
    /// </summary>
    [Serializable]
    public class SteamVRInputInfo: BaseInputInfo {
        public bool PressState;
        public bool PressDownState;
        public bool PressUpState;

        public bool TouchState;
        public bool TouchDownState;
        public bool TouchUpState;

        public Vector2 AxisValue;

        public bool HairTriggerState;
        public bool HairTriggerDownState;
        public bool HairTriggerUpState;

        public override RVInputMethod InputMethod() {
            return RVInputMethod.SteamVR;
        }

        public override void MergeInputInfo(IRecordInputInfo inputInfo) {
            if (inputInfo.InputMethod() != RVInputMethod.SteamVR) {
                throw new UnityException("Not valid input info!");
            }
            SteamVRInputInfo steamVRInputInfo = (SteamVRInputInfo)inputInfo;
            PressState = PressState | steamVRInputInfo.PressState;
            PressDownState = PressDownState | steamVRInputInfo.PressDownState;
            PressUpState = PressUpState | steamVRInputInfo.PressUpState;

            TouchState = TouchState | steamVRInputInfo.TouchState;
            TouchDownState = PressDownState | steamVRInputInfo.TouchDownState;
            TouchUpState = PressUpState | steamVRInputInfo.TouchUpState;

            AxisValue = new Vector2 (
                Mathf.Max (AxisValue.x, steamVRInputInfo.AxisValue.x),
                Mathf.Max (AxisValue.y, steamVRInputInfo.AxisValue.y));

            HairTriggerState = TouchState | steamVRInputInfo.HairTriggerState;
            HairTriggerDownState = PressDownState | steamVRInputInfo.HairTriggerDownState;
            HairTriggerUpState = PressUpState | steamVRInputInfo.HairTriggerUpState;
        }
    }

    /// <summary>
    /// Input info for object position.
    /// </summary>
    [Serializable]
    public class PositionInputInfo: BaseInputInfo {
        private Vector3 m_Position;
        private Quaternion m_Rotation;

        public Vector3 Positon {
            get {
                return m_Position;
            }
            set {
                m_Position = value;
            }
        }

        public Quaternion Rotation
        {
            get {
                return m_Rotation;
            }
            set {
                m_Rotation = value;
            }
        }

        public override RVInputMethod InputMethod() {
            return RVInputMethod.ObjectPosition;
        }

        public override void MergeInputInfo(IRecordInputInfo inputInfo) {
            throw new UnityException("MergeInputInfo method not implemented!");
        }
    }
}