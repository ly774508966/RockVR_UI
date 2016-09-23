using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace RockVR.Capture {
    public class RVBaseCapture : MonoBehaviour {
        [SerializeField]
        protected string m_SaveFolder = "RVCaptures";
        [SerializeField]
        protected string m_SaveFileName = "FileName";
        [SerializeField]
        protected bool m_StartCapture = false;
        [SerializeField]
        protected bool m_UseMotion = false;
        protected bool m_UsingMotion = false;

        protected int m_FrameIndex = 0;
        protected Vector3[] m_RecordedMovement = null;

        protected void Start() {
            // Can run in background during capture
            if (m_StartCapture) {
                PlayerSettings.runInBackground = true;
            }
            // Load motion replay file
            if (m_UseMotion) {
                m_RecordedMovement = MotionCapture.LoadRecordFile ();
                if (m_RecordedMovement != null && m_RecordedMovement.Length > 1) {
                    m_UsingMotion = true;
                }
            }
            // Check save folder
            if (m_SaveFolder.Length == 0) {
                throw new Exception("Save folder not valid!");
            }
            if (!Directory.Exists(m_SaveFolder)) {
                Directory.CreateDirectory(m_SaveFolder);
            }
            // Disable custom motions if in motion replay mode
            BaseMotion[] motions = GetComponents<BaseMotion>();
            foreach (BaseMotion motion in motions) {
                if (m_UseMotion) {
                    motion.enabled = false;
                }
            }
        }

        protected void FixedUpdate() {
            // Use motion replay mode
            if (m_UsingMotion) {
                if (m_FrameIndex < m_RecordedMovement.Length) {
                    transform.position = m_RecordedMovement [m_FrameIndex];
                } else {
                    Debug.Log ("Replay Finish!");
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Application.Quit ();
                    #endif
                }
            }
        }
    }

    public enum ImageFormat {
        jpg,
        png,
    }

    public enum AudioFormat {
        wav,
    }
}