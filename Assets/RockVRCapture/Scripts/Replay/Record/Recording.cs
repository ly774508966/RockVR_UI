using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace RockVR.Replay {

    /// <summary>
    /// Recording, record all input instruction for each frame.
    /// </summary>
    [Serializable]
    public class Recording<T> where T : IRecordInputInfo {
        /// <summary>
        /// Current frame index.
        /// </summary>
        private int FrameIndex = 0;
        /// <summary>
        /// All record frames.
        /// </summary>
        public List<RecordingFrame<T>> Frames;
        /// <summary>
        /// Current record frame.
        /// </summary>
        public RecordingFrame<T> CurrentFrame { get; private set; }

        public Recording() {
            Frames = new List<RecordingFrame<T>>();

            NewFrame ();
        }

        /// <summary>
        /// Create a new record frame and add to frame list.
        /// </summary>
        public void NewFrame() {
            CurrentFrame = new RecordingFrame<T> ();
            Frames.Add (CurrentFrame);
        }

        /// <summary>
        /// Move to next frame based on frame internal index.
        /// </summary>
        public void NextFrame() {
            if (FrameIndex >= Frames.Count) {
                CurrentFrame = null;
                return;
            }
            CurrentFrame = Frames [FrameIndex++];
        }

        /// <summary>
        /// Determines whether has record frame currently.
        /// </summary>
        /// <returns>Whether still has record frame.</returns>
        public bool HasCurrentFrame() {
            return CurrentFrame != null;
        }
    }

    [Serializable]
    public class RecordingFrame<T> where T : IRecordInputInfo {
        /// <summary>
        /// Device Index => (Input Name => Input Info).
        /// </summary>
        public Dictionary<int, Dictionary<string, T>> Inputs;

        /// <summary>
        /// Get the first default input.
        /// </summary>
        /// <value>The input.</value>
        public Dictionary<string, T> Input {
            get {
                return Inputs[0];
            }
        }

        public RecordingFrame() {
            Inputs = new Dictionary<int, Dictionary<string, T>> ();
            // add default index 0
            Inputs.Add (0, new Dictionary<string, T> ());
        }

        public void AddInputInfo(string inputName, T inputInfo) {
            AddInputInfo (0, inputName, inputInfo);
        }

        public void AddInputInfo(int deviceIndex, string inputName, T inputInfo) {
            if (!Inputs.ContainsKey (deviceIndex)) {
                Inputs.Add (deviceIndex, new Dictionary<string, T> ());
            }

            if (Inputs[deviceIndex].ContainsKey (inputName)) {
                // Debug.LogWarning (inputName + " alreay exists");
                Inputs[deviceIndex][inputName].MergeInputInfo(inputInfo);
                return;
            }
            Inputs[deviceIndex].Add (inputName, inputInfo);
        }

        public T GetInputInfo(string inputName) {
            return GetInputInfo (0, inputName);
        }

        public T GetInputInfo(int deviceIndex, string inputName) {
            if (!Inputs.ContainsKey (deviceIndex) || !Inputs[deviceIndex].ContainsKey (inputName)) {
                return default(T);
            }
            return Inputs[deviceIndex] [inputName];
        }
    }
}
