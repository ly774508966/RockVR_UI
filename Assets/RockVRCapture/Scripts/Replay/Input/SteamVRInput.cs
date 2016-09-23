using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR;
using Newtonsoft.Json;

namespace RockVR.Replay {

    /// <summary>
    /// Steam VR input.
    /// Record Steam VR controller button state, touchpad axis, etc.
    /// </summary>
    public class SteamVRInput : BaseInput<SteamVRInputInfo> {

        private Dictionary<int, SteamVRDevice> m_Devices;

        public new Recording<SteamVRInputInfo> CurrentRecording {
            get {
                if (m_CurrentRecording == null) {
                    m_CurrentRecording = new Recording<SteamVRInputInfo>();
                }
                return m_CurrentRecording;
            }
            set {
                m_CurrentRecording = value;
            }
        }

        public SteamVRInput(RVInputMode inputMode): base(inputMode) {
            m_Devices = new Dictionary<int, SteamVRDevice>();
        }

        public SteamVRDevice Input(int deviceIndex) {
            SteamVRDevice device;
            if (!m_Devices.ContainsKey(deviceIndex))
            {
                device = new SteamVRDevice((uint)deviceIndex, this);
            } else
            {
                device = m_Devices[deviceIndex];
            }
            return device;
        }

        public override RVInputMethod InputMethod() {
            return RVInputMethod.SteamVR;
        }

        public override string SaveFileName () {
            return "SteamVRInputFile";
        }

        public override void SaveRecordFile() {
            string json = JsonConvert.SerializeObject(CurrentRecording);
            StreamWriter writer = File.CreateText(SaveFilePath());
            writer.WriteLine(json);
            writer.Close();
            Debug.Log("SaveRecordFile : " + SaveFileName());
        }

        public override void LoadRecordFile() {
            if (!File.Exists(SaveFilePath())) {
                throw new FileNotFoundException(SaveFilePath() + " not found!");
            }
            StreamReader reader = new StreamReader(SaveFilePath());
            string content = reader.ReadToEnd();
            reader.Close();
            CurrentRecording = JsonConvert.DeserializeObject<Recording<SteamVRInputInfo>>(content);
            Debug.Log("LoadRecordFile : " + SaveFileName());
        }
    }

    public class SteamVRDevice
    {
        public uint m_Index;
        public SteamVRInput m_InputHolder;
        private SteamVR_Controller.Device m_DeviceImpl;

        public SteamVRDevice(uint index, SteamVRInput input) {
            m_Index = index;
            m_InputHolder = input;
            m_DeviceImpl = SteamVR_Controller.Input((int)m_Index);
        }

        public int DeviceIndex {
            get {
                return (int)m_Index;
            }
        }

        /// <summary>
        /// Steam VR Input holder.
        /// </summary>
        public SteamVRInput InputHolder {
            get {
                return m_InputHolder;
            }
        }

        /// <summary>
        /// Recording Object from Steam VR Input.
        /// </summary>
        public Recording<SteamVRInputInfo> CurrentRecording {
            get {
                return InputHolder.CurrentRecording;
            }
        }

        /// <summary>
        /// Inpute mode from Steam VR Input.
        /// </summary>
        public RVInputMode CurrentInputMode {
            get {
                return InputHolder.CurrentInputMode;
            }
        }

        /// <summary>
        /// Get the Steam VR button press state.
        /// </summary>
        /// <param name="buttonMask">Button Mask</param>
        /// <returns>Press state</returns>
        public bool GetPress(ulong buttonMask) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString()) : null;
                return inputInfo == null ? false : inputInfo.PressState;
            }
            bool stateValue = m_DeviceImpl.GetPress(buttonMask);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.PressState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonMask.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button press down state.
        /// </summary>
        /// <param name="buttonMask">Button Mask</param>
        /// <returns>Press down state</returns>
        public bool GetPressDown(ulong buttonMask) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString()) : null;
                return inputInfo == null ? false : inputInfo.PressDownState;
            }
            bool stateValue = m_DeviceImpl.GetPressDown(buttonMask);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.PressDownState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonMask.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button press up state.
        /// </summary>
        /// <param name="buttonMask">Button Mask</param>
        /// <returns>Press up state</returns>
        public bool GetPressUp(ulong buttonMask) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString()) : null;
                return inputInfo == null ? false : inputInfo.PressUpState;
            }
            bool stateValue = m_DeviceImpl.GetPressUp(buttonMask);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.PressUpState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonMask.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button press state.
        /// </summary>
        /// <param name="buttonId">Button ID</param>
        /// <returns>Press state</returns>
        public bool GetPress(EVRButtonId buttonId) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? false : inputInfo.PressState;
            }
            bool stateValue = m_DeviceImpl.GetPress(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.PressState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button press down state.
        /// </summary>
        /// <param name="buttonId">Button ID</param>
        /// <returns>Press down state</returns>
        public bool GetPressDown(EVRButtonId buttonId) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? false : inputInfo.PressDownState;
            }
            bool stateValue = m_DeviceImpl.GetPressDown(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.PressDownState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button press up state.
        /// </summary>
        /// <param name="buttonId">Button ID</param>
        /// <returns>Press up state</returns>
        public bool GetPressUp(EVRButtonId buttonId) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? false : inputInfo.PressUpState;
            }
            bool stateValue = m_DeviceImpl.GetPressUp(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.PressUpState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button touch state.
        /// </summary>
        /// <returns>Touch state.</returns>
        /// <param name="buttonMask">Button mask.</param>
        public bool GetTouch(ulong buttonMask) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString()) : null;
                return inputInfo == null ? false : inputInfo.TouchState;
            }
            bool stateValue = m_DeviceImpl.GetTouch(buttonMask);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.TouchState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonMask.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button touch down state.
        /// </summary>
        /// <returns>Touch down state.</returns>
        /// <param name="buttonMask">Button mask.</param>
        public bool GetTouchDown(ulong buttonMask) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString()) : null;
                return inputInfo == null ? false : inputInfo.TouchDownState;
            }
            bool stateValue = m_DeviceImpl.GetTouchDown(buttonMask);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.TouchDownState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonMask.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button touch up state.
        /// </summary>
        /// <returns>Touch up state.</returns>
        /// <param name="buttonMask">Button mask.</param>
        public bool GetTouchUp(ulong buttonMask) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString()) : null;
                return inputInfo == null ? false : inputInfo.TouchUpState;
            }
            bool stateValue = m_DeviceImpl.GetTouchUp(buttonMask);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonMask.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.TouchUpState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonMask.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button touch state.
        /// </summary>
        /// <returns>Touch state.</returns>
        /// <param name="buttonId">Button identifier.</param>
        public bool GetTouch(EVRButtonId buttonId) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? false : inputInfo.TouchState;
            }
            bool stateValue = m_DeviceImpl.GetTouch(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.TouchState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button touch down state.
        /// </summary>
        /// <returns>Touch down state.</returns>
        /// <param name="buttonId">Button identifier.</param>
        public bool GetTouchDown(EVRButtonId buttonId) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? false : inputInfo.TouchDownState;
            }
            bool stateValue = m_DeviceImpl.GetTouchDown(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.TouchDownState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR button touch up state.
        /// </summary>
        /// <returns>Touch up state.</returns>
        /// <param name="buttonId">Button identifier.</param>
        public bool GetTouchUp(EVRButtonId buttonId) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? false : inputInfo.TouchUpState;
            }
            bool stateValue = m_DeviceImpl.GetTouchUp(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.TouchUpState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR axis value.
        /// </summary>
        /// <returns>The axis.</returns>
        /// <param name="buttonId">Button identifier.</param>
        public Vector2 GetAxis(EVRButtonId buttonId = EVRButtonId.k_EButton_SteamVR_Touchpad) {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString()) : null;
                return inputInfo == null ? Vector2.zero : inputInfo.AxisValue;
            }
            Vector2 axisValue = m_DeviceImpl.GetAxis(buttonId);
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, buttonId.ToString());
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.AxisValue = axisValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, buttonId.ToString(), inputInfo);
            }
            return axisValue;
        }

        public void TriggerHapticPulse(ushort durationMicroSec = 500, EVRButtonId buttonId = EVRButtonId.k_EButton_SteamVR_Touchpad) {
            m_DeviceImpl.TriggerHapticPulse(durationMicroSec, buttonId);
        }

        /// <summary>
        /// Get the Steam VR hair trigger state.
        /// </summary>
        /// <returns>Trigger state.</returns>
        public bool GetHairTrigger() {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, "HairTrigger") : null;
                return inputInfo == null ? false : inputInfo.HairTriggerState;
            }
            bool stateValue = m_DeviceImpl.GetHairTrigger();
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, "HairTrigger");
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.HairTriggerState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, "HairTrigger", inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR hair trigger down state.
        /// </summary>
        /// <returns>Trigger down state.</returns>
        public bool GetHairTriggerDown() {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, "HairTriggerDown") : null;
                return inputInfo == null ? false : inputInfo.HairTriggerDownState;
            }
            bool stateValue = m_DeviceImpl.GetHairTriggerDown();
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, "HairTriggerDown");
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.HairTriggerDownState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, "HairTriggerDown", inputInfo);
            }
            return stateValue;
        }

        /// <summary>
        /// Get the Steam VR hair trigger up state.
        /// </summary>
        /// <returns>Trigger up state.</returns>
        public bool GetHairTriggerUp() {
            if (CurrentInputMode == RVInputMode.Playback) {
                SteamVRInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, "HairTriggerUp") : null;
                return inputInfo == null ? false : inputInfo.HairTriggerUpState;
            }
            bool stateValue = m_DeviceImpl.GetHairTriggerUp();
            if (CurrentInputMode == RVInputMode.Record) {
                SteamVRInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(DeviceIndex, "HairTriggerUp");
                if (inputInfo == null) {
                    inputInfo = new SteamVRInputInfo();
                }
                inputInfo.HairTriggerUpState = stateValue;
                CurrentRecording.CurrentFrame.AddInputInfo(DeviceIndex, "HairTriggerUp", inputInfo);
            }
            return stateValue;
        }
    }
}