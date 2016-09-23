using UnityEngine;
using System.IO;

namespace RockVR.Replay {
    public class RVReplayInput : MonoBehaviour {
        /// <summary>
        /// If record Standalone input, Keyboard, Mouse, etc.
        /// </summary>
        [SerializeField]
        private bool m_RecordStandalone;
        /// <summary>
        /// If record Steam VR input, VR controller.
        /// </summary>
        [SerializeField]
        private bool m_RecordSteamVR;
        /// <summary>
        /// The objects should record position, rotation.
        /// </summary>
        [SerializeField]
        private GameObject[] m_RecordPositionObject;
        /// <summary>
        /// The clone objects and apply replay data.
        /// </summary>
        private GameObject[] m_ReplayCloneObject;
        [SerializeField]
        private RVInputMode m_Mode = RVInputMode.Record;

        private StandaloneInput m_StandaloneInput;
        private SteamVRInput m_SteamVRInput;
        private PositionInput m_PositionInput;

        public bool RecordStandalone {
            get {
                return m_RecordStandalone;
            }
        }

        public bool RecordSteamVR {
            get {
                return m_RecordSteamVR;
            }
        }

        public bool HasRecordPositionObject() {
            return m_RecordPositionObject.Length > 0;
        }

        public RVInputMode Mode {
            get {
                return m_Mode;
            }
        }

        public StandaloneInput Standalone {
            get {
                if (m_StandaloneInput == null) {
                    m_StandaloneInput = new StandaloneInput(m_Mode);
                }
                return m_StandaloneInput;
            }
        }

        public SteamVRInput SteamVR {
            get {
                if (m_SteamVRInput == null) {
                    m_SteamVRInput = new SteamVRInput(m_Mode);
                }
                return m_SteamVRInput;
            }
        }

        private PositionInput ObjectPosition {
            get {
                if (!HasRecordPositionObject()) {
                    throw new UnityException("Did not find object to record!");
                }
                if (m_PositionInput == null) {
                    m_PositionInput = new PositionInput(m_Mode);
                }
                return m_PositionInput;
            }
        }

        private void Start() {
            if (m_Mode == RVInputMode.Playback) {
                // set maximum allowed timestep correct
                Time.maximumDeltaTime = Time.fixedDeltaTime;
                if (m_RecordStandalone) {
                    Standalone.LoadRecordFile ();
                }
                if (m_RecordSteamVR) {
                    SteamVR.LoadRecordFile();
                }
                if (HasRecordPositionObject()) {
                    ObjectPosition.LoadRecordFile();
                    m_ReplayCloneObject = new GameObject[m_RecordPositionObject.Length];
                    // clone recorded object
                    for (int i = 0; i < m_RecordPositionObject.Length; i++) {
                        m_ReplayCloneObject[i] = (GameObject)Instantiate(m_RecordPositionObject[i], Vector3.zero, Quaternion.identity);
                    }
                }
            }
        }

        /// <summary>
        /// Create a new frame.
        /// </summary>
        private void NewFrame() {
            if (m_RecordStandalone) {
                Standalone.CurrentRecording.NewFrame();
            }
            if (m_RecordSteamVR) {
                SteamVR.CurrentRecording.NewFrame();
            }
            if (HasRecordPositionObject())
            {
                ObjectPosition.CurrentRecording.NewFrame();
            }
        }

        /// <summary>
        /// Move to next record frame.
        /// </summary>
        private void NextFrame() {
            if (m_RecordStandalone) {
                Standalone.CurrentRecording.NextFrame();
            }
            if (m_RecordSteamVR) {
                SteamVR.CurrentRecording.NextFrame();
            }
            if (HasRecordPositionObject())
            {
                ObjectPosition.CurrentRecording.NextFrame();
            }
        }

        /// <summary>
        /// Check if any replay input has more record frame.
        /// </summary>
        /// <returns></returns>
        private bool HasCurrentFrame() {
            bool hasCurrentFrame = false;
            if (
                m_RecordStandalone &&
                (Standalone.CurrentRecording != null && Standalone.CurrentRecording.HasCurrentFrame())
            ) {
                hasCurrentFrame = true;
            }
            if (
                m_RecordSteamVR &&
                (SteamVR.CurrentRecording != null && SteamVR.CurrentRecording.HasCurrentFrame())
            ) {
                hasCurrentFrame = true;
            }
            if (
                HasRecordPositionObject() &&
                (ObjectPosition.CurrentRecording != null && ObjectPosition.CurrentRecording.HasCurrentFrame())
            )
            {
                hasCurrentFrame = true;
            }
            return hasCurrentFrame;
        }

        /// <summary>
        /// Saves all record files as JSON format.
        /// </summary>
        private void SaveRecordFiles() {
            if (m_RecordStandalone) {
                if (Directory.Exists(Standalone.SaveFolderName())) {
                    Directory.CreateDirectory(Standalone.SaveFolderName());
                }
                Standalone.SaveRecordFile ();
            }
            if (m_RecordSteamVR) {
                if (Directory.Exists(SteamVR.SaveFolderName())) {
                    Directory.CreateDirectory(SteamVR.SaveFolderName());
                }
                SteamVR.SaveRecordFile();
            }
            if (HasRecordPositionObject()) {
                if (Directory.Exists(ObjectPosition.SaveFolderName())) {
                    Directory.CreateDirectory(ObjectPosition.SaveFolderName());
                }
                ObjectPosition.SaveRecordFile();
            }
        }

        /// <summary>
        /// Record or Replay in fixed frame rate.
        /// </summary>
        private void FixedUpdate() {
            if (m_Mode == RVInputMode.Record) {
                NewFrame();
                if (HasRecordPositionObject())
                {
                    foreach (GameObject gameObject in m_RecordPositionObject)
                    {
                        ObjectPosition.SetPosition(gameObject);
                    }
                }
            }
            if (m_Mode == RVInputMode.Playback) {
                NextFrame();
                if (!HasCurrentFrame()) {
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Application.Quit ();
                    #endif
                }
                if (HasRecordPositionObject())
                {
                    foreach (GameObject gameObject in m_ReplayCloneObject)
                    {
                        // hack, cloned object will add (Clone) at end, remove to match recorded object
                        string name = gameObject.name.Replace("(Clone)", "");
                        PositionInputInfo positionInfo = ObjectPosition.GetPosition(name);
                        if (positionInfo != null)
                        {
                            gameObject.transform.position = positionInfo.Positon;
                            gameObject.transform.rotation = positionInfo.Rotation;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save records if in record mode.
        /// </summary>
        private void OnApplicationQuit() {
            if (m_Mode == RVInputMode.Record) {
                SaveRecordFiles ();
            }
        }
    }

    public enum RVInputMode {
        Normal,
        Record,
        Playback,
    }

    public enum RVInputMethod {
        Standalone,
        SteamVR,
        ObjectPosition,
    }
}
