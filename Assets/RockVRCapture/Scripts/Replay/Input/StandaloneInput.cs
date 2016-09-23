using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace RockVR.Replay {

    /// <summary>
    /// Standalone Input.
    /// Record keyboard, mouse button state, mouse position, etc.
    /// </summary>
    public class StandaloneInput : BaseInput<StandaloneInputInfo> {

        public StandaloneInput(RVInputMode inputMode): base(inputMode) {}

        /// <summary>
        /// Returns the platform appropriate axis for the given name.
        /// </summary>
        /// <param name="name">Input Name</param>
        /// <returns>Axis Value</returns>
        public float GetAxis (string name) {
            if (CurrentInputMode == RVInputMode.Playback) {
                StandaloneInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    (StandaloneInputInfo)CurrentRecording.CurrentFrame.GetInputInfo (name) : null;
                return inputInfo == null ? 0.0f : inputInfo.AxisValue;
            }
            float axisValue = Input.GetAxis(name);
            if (CurrentInputMode == RVInputMode.Record) {
                BaseInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(name);
                if (inputInfo == null) {
                    inputInfo = new StandaloneInputInfo ();
                }
                StandaloneInputInfo standaloneInputInfo = (StandaloneInputInfo)inputInfo;
                standaloneInputInfo.AxisValue = axisValue;
                CurrentRecording.CurrentFrame.AddInputInfo (name, standaloneInputInfo);
            }
            return axisValue;
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public float GetAxisRaw (string name) {
            return Input.GetAxisRaw (name);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetButton (string name) {
            return Input.GetButton (name);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetButtonDown(string name) {
            return Input.GetButtonDown (name);
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetButtonUp(string name) {
            return Input.GetButtonUp (name);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Vector3 MousePosition() {
            return Input.mousePosition;
        }

        public override RVInputMethod InputMethod() {
            return RVInputMethod.Standalone;
        }

        public override string SaveFileName () {
            return "StandaloneInputFile";
        }

        public override void SaveRecordFile() {
            string json = JsonConvert.SerializeObject (CurrentRecording);
            StreamWriter writer = File.CreateText(SaveFilePath());
            writer.WriteLine (json);
            writer.Close();
            Debug.Log ("SaveRecordFile : " + SaveFileName ());
        }

        public override void LoadRecordFile() {
            if (!File.Exists(SaveFilePath())) {
                throw new FileNotFoundException(SaveFilePath() + " not found!");
            }
            StreamReader reader = new StreamReader(SaveFilePath());
            string content = reader.ReadToEnd();
            reader.Close();
            CurrentRecording = JsonConvert.DeserializeObject<Recording<StandaloneInputInfo>> (content);
            Debug.Log ("LoadRecordFile : " + SaveFileName ());
        }
    }
}
