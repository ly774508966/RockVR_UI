using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RockVR.Capture {

    public class MotionCapture : MonoBehaviour {

        public bool m_StartRecord = false;
        public float m_MoveSpeed = 0.1f;
        public float m_RotateSpeed = 0.1f;

        private int m_FrameIndex = 1;

        private static string m_SaveFolder = "RVCaptures";

        private List<Vector3> m_MovePath = new List<Vector3> ();

        private void Start() {
            if (!Directory.Exists(m_SaveFolder)) {
                Directory.CreateDirectory(m_SaveFolder);
            }
        }
    	
    	private void Update () {

            if (m_StartRecord) {
                if (Input.GetKey (KeyCode.W)) {
                    transform.position += Vector3.forward * m_MoveSpeed;
                }
                if (Input.GetKey (KeyCode.S)) {
                    transform.position += Vector3.back * m_MoveSpeed;
                }
                if (Input.GetKey (KeyCode.A)) {
                    transform.position += Vector3.left * m_MoveSpeed;
                }
                if (Input.GetKey (KeyCode.D)) {
                    transform.position += Vector3.right * m_MoveSpeed;
                }
                if (Input.GetKey (KeyCode.LeftArrow)) {
                    transform.Rotate(Vector3.down * m_RotateSpeed);
                }
                if (Input.GetKey (KeyCode.RightArrow)) {
                    transform.Rotate(Vector3.up * m_RotateSpeed);
                }
                if (Input.GetKey (KeyCode.UpArrow)) {
                    transform.position += Vector3.up * m_MoveSpeed;
                }
                if (Input.GetKey (KeyCode.DownArrow)) {
                    transform.position += Vector3.down * m_MoveSpeed;
                }
                m_MovePath.Add (transform.position);
                Debug.Log ("Record Frame: " + m_FrameIndex++);
                return;
            }
    	}

        private void OnApplicationQuit() {
            if (!m_StartRecord) {
                return;
            }

            SaveRecordFile ();
        }

        public void SaveRecordFile() {
            string json = JsonConvert.SerializeObject (m_MovePath);
            StreamWriter writer = File.CreateText(m_SaveFolder + "/" + SaveFileName());
            writer.WriteLine (json);
            writer.Close();
            Debug.Log ("SaveRecordFile : " + SaveFileName ());
        }

        public static Vector3[] LoadRecordFile() {
            if (!File.Exists(m_SaveFolder + "/" + SaveFileName())) {
                throw new FileNotFoundException(m_SaveFolder + "/" + SaveFileName() + " not found!");
            }
            StreamReader reader = new StreamReader(m_SaveFolder + "/" + SaveFileName());
            string content = reader.ReadToEnd();
            reader.Close();
            Vector3[] motions = JsonConvert.DeserializeObject<Vector3[]> (content);
            Debug.Log ("LoadRecordFile : " + SaveFileName ());
            return motions;
        }

        public static string SaveFileName() {
            return "MotionCaptureFile";
        }
    }
}