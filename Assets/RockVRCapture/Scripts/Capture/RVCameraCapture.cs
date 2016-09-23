using UnityEngine;
using System.Collections;
using System.IO;
using RockVR.Utils;

namespace RockVR.Capture {

    [RequireComponent (typeof(Camera))]
    public class RVCameraCapture : RVBaseCapture {

        private int m_RecordIndex = 0;
        [SerializeField]
        private int m_FrameInterval = 3; // take one capture for every m_FrameInterval frames
        [SerializeField]
        private int m_OutputWidth = 1280;
        [SerializeField]
        private int m_OutputHeight = 720;
        [SerializeField]
        private bool m_EncodeImage = true;
        [SerializeField]
        private ImageFormat m_ImageFormat = ImageFormat.jpg;
        [SerializeField]
        private int m_AntiAliasing = 2;
        [SerializeField]
        private int m_JPGQuality = 75;
        [SerializeField]
        private bool m_EncodeVideo = false;

        private Camera m_Camera;
        private float m_TotalTime = 0.0f;
        private string m_CurrentTempFolder;
        private bool m_CapturingImage = false;

        private bool IsReady() {
            if (m_Camera == null || !m_StartCapture) {
                return false;
            }
            return true;
        }

        private void CreateTempFolder() {
            m_CurrentTempFolder = StringUtils.RandomString ();
            while (Directory.Exists(m_SaveFolder + "/" + m_CurrentTempFolder)) {
                m_CurrentTempFolder = StringUtils.RandomString ();
            }
            Directory.CreateDirectory(m_SaveFolder + "/" + m_CurrentTempFolder);
        }

        private void DeleteTempFolder() {
            if (Directory.Exists(m_SaveFolder + "/" + m_CurrentTempFolder))
            {
                Directory.Delete(m_SaveFolder + "/" + m_CurrentTempFolder, true);
            }
            // Delete Audio, TODO: remove hack
            if (File.Exists(m_SaveFolder + "/Audio.wav"))
            {
                File.Delete(m_SaveFolder + "/Audio.wav");
            }
        }

        private IEnumerator CaptureImage() {
            m_CapturingImage = true;
            RenderTexture renderTexture = new RenderTexture(m_OutputWidth, m_OutputHeight, 24);
            renderTexture.antiAliasing = m_AntiAliasing;
            m_Camera.targetTexture = renderTexture;
            Texture2D cameraTexture = new Texture2D(m_OutputWidth, m_OutputHeight);
            m_Camera.Render();
            RenderTexture.active = renderTexture;
            cameraTexture.ReadPixels(new Rect(0, 0, m_OutputWidth, m_OutputHeight), 0, 0, false);
            RenderTexture.active = null;
            m_Camera.targetTexture = null;
            yield return null;

            cameraTexture.Apply ();
            yield return null;

            byte[] bytes = null;
            string extension = null;
            if (m_EncodeImage) {
                if (m_ImageFormat == ImageFormat.jpg) {
                    bytes = cameraTexture.EncodeToJPG (m_JPGQuality);
                } else {
                    bytes = cameraTexture.EncodeToPNG ();
                }
                extension = "." + m_ImageFormat;
            } else {
                bytes = cameraTexture.GetRawTextureData ();
                extension = ".rvt";
            }
            yield return null;

            // Create new thread then save image 
            string savePath = m_SaveFolder + "/" + m_CurrentTempFolder + "/" + m_SaveFileName + m_FrameIndex + extension;
            new System.Threading.Thread(() => {
                File.WriteAllBytes(savePath, bytes);
            }).Start();

            // Clean up
            DestroyImmediate(renderTexture);
            DestroyImmediate(cameraTexture);
            m_FrameIndex++;
            m_CapturingImage = false;
        }

        private new void Start () {
            base.Start ();
            if (!m_StartCapture) return;
            m_Camera = GetComponent<Camera>();
            CreateTempFolder ();
        }

        private void LateUpdate() {
            m_TotalTime += Time.deltaTime;
            if (!IsReady()) return;
            if (m_RecordIndex++ % m_FrameInterval != 0) return;

            if (!m_CapturingImage)
                StartCoroutine (CaptureImage ());
            else
                Debug.LogWarning ("Last frame image capture not finish!");
        }

        private void OnApplicationQuit() {
            if (m_StartCapture && m_EncodeVideo && m_EncodeImage) {
                // Calculate video framerate
                int framerate = (int)(1 / (m_TotalTime / m_RecordIndex) / m_FrameInterval);
                #if UNITY_STANDALONE_OSX
                string platform = "OSX";
                #endif
                #if UNITY_STANDALONE_WIN
                string platform = "WIN";
                #endif
                string ffmpegPath = Application.dataPath + "/RockVRCapture/ThirdParty/FFmpeg/" + platform + "/";
                string capturesPath = Path.GetFullPath(string.Format(@"{0}/", "RVCaptures"));
                string ffmpegArgs =
                    "-f image2 " +
                    "-r " + framerate + " " +
                    "-i \"" + capturesPath + m_CurrentTempFolder + "/" + m_SaveFileName + "%d.jpg\" " +
                    "-vcodec libx264 " +
                    "-crf 25 " +
                    "-pix_fmt yuv420p \"" + capturesPath + m_SaveFileName + ".mp4\"";
                if (File.Exists (capturesPath + "Audio.wav")) {
                    ffmpegArgs =
                        "-f image2 " +
                        "-r " + framerate + " " +
                        "-i \"" + capturesPath + m_CurrentTempFolder + "/" + m_SaveFileName + "%d.jpg\" " +
                        "-i \"" + capturesPath + "Audio.wav\" " +
                        "-vcodec libx264 " +
                        "-crf 25 " +
                        "-pix_fmt yuv420p " +
                        "-b:v 12000k " +
                        "-c:a aac " +
                        "-strict experimental " + 
                        "-b:a 192k -shortest " +
                        "\"" + capturesPath + m_SaveFileName + ".mp4\"";
                    Debug.Log ("Merging Audio...");
                }
                var processInfo = new System.Diagnostics.ProcessStartInfo(ffmpegPath + "ffmpeg", ffmpegArgs);
                processInfo.CreateNoWindow = false;
                var process = System.Diagnostics.Process.Start(processInfo);
                process.WaitForExit();
                process.Close();
                DeleteTempFolder ();
                Debug.Log ("Record Video Finish!");
            }
        }
    }
}
