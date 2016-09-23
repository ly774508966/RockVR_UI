using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using RockVR.Capture;
using RockVR.Utils;

namespace RockVR.SDK
{
    public class RV2DCamera : RVInteractiveItem
    {
        [SerializeField]
        public  Camera m_CaptureCamera;
        [SerializeField]
        private GameObject m_CameraScreen;
        [SerializeField]
        private GameObject m_CameraSphere;
        [SerializeField]
        private GameObject m_FacingObject;
        [SerializeField]
        private GameObject m_CaptureText;
        // TODO: Refactor with RVCameraCapture
        [SerializeField]
        private int m_FrameInterval = 3;    // take one capture for every m_FrameInterval frames
        [SerializeField]
        private bool m_EncodeImage = true;
        [SerializeField]
        private ImageFormat m_ImageFormat = ImageFormat.jpg;
        [SerializeField]
        private int m_JPGQuality = 75;
        [SerializeField]
        private bool m_EncodeVideo = false;

        private string m_SaveFolder = "RVCaptures";
        private string m_SaveFileName = "2DCamera";
        private string m_CurrentTempFolder;

        private bool m_Enabled;
        private bool m_Capturing;   // if current is captureing
        private bool m_CapturingImage;  // if current is capturing a image frame
        private int m_CaptureIndex;
        private int m_FrameIndex;
        private float m_TotalCaptureTime;

        private void Awake()
        {
            if (m_CaptureCamera == null)
            {
                throw new MissingComponentException("CaptureCamera not attached!");
            }
            if (m_CameraScreen == null)
            {
                throw new MissingComponentException("CameraScreen not attached!");
            }
            if (m_CameraSphere == null)
            {
                throw new MissingComponentException("CameraSphere not attached!");
            }
            if (m_FacingObject == null)
            {
                throw new MissingComponentException("FacingObject not attached!");
            }
            if (m_CaptureText == null)
            {
                throw new MissingComponentException("CaptureText not attached!");
            }
        }

        private void Start()
        {
            m_InteractiveItemType = RVInteractiveItemType._2DCameraItem;
        }

        private void Update()
        {
            if (m_Enabled)
            {
                this.gameObject.transform.LookAt(m_FacingObject.transform);
                this.m_CaptureCamera.transform.LookAt(this.transform.parent.parent);
            }
        }

        private IEnumerator CaptureImage()
        {
            m_CapturingImage = true;
            RenderTexture renderTexture = m_CaptureCamera.targetTexture;
            Texture2D cameraTexture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            cameraTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
            RenderTexture.active = null;
            yield return null;

            cameraTexture.Apply();
            yield return null;

            byte[] bytes = null;
            string extension = null;
            if (m_EncodeImage)
            {
                if (m_ImageFormat == ImageFormat.jpg)
                {
                    bytes = cameraTexture.EncodeToJPG(m_JPGQuality);
                }
                else
                {
                    bytes = cameraTexture.EncodeToPNG();
                }
                extension = "." + m_ImageFormat;
            }
            else
            {
                bytes = cameraTexture.GetRawTextureData();
                extension = ".rvt";
            }
            yield return null;

            // Create new thread then save image 
            string savePath = m_SaveFolder + "/" + m_CurrentTempFolder + "/" + m_SaveFileName + m_FrameIndex + extension;
            new Thread(() => {
                File.WriteAllBytes(savePath, bytes);
            }).Start();
            // Clean up
            // DestroyImmediate(renderTexture, true);
            DestroyImmediate(cameraTexture, true);
            m_FrameIndex++;
            m_CapturingImage = false;
        }

        private void LateUpdate()
        {
            if (!m_Enabled || !m_Capturing) return;

            m_TotalCaptureTime += Time.deltaTime;
            if (m_CaptureIndex++ % m_FrameInterval != 0) return;

            if (!m_CapturingImage)
                StartCoroutine(CaptureImage());
            else
                Debug.LogWarning("Last frame image capture not finish!");
        }

        public bool Enabled()
        {
            return m_Enabled;
        }

        public bool Capturing()
        {
            return m_Capturing;
        }

        private static string CreateTempFolder(string saveFolder)
        {
            string currentTempFolder = StringUtils.RandomString();
            while (Directory.Exists(saveFolder + "/" + currentTempFolder))
            {
                currentTempFolder = StringUtils.RandomString();
            }
            Debug.Log("CreateTempFolder: " + saveFolder + "/" + currentTempFolder);
            Directory.CreateDirectory(saveFolder + "/" + currentTempFolder);
            return currentTempFolder;
        }

        private static void DeleteTempFolder(string saveFolder, string tempFolder)
        {
            if (Directory.Exists(saveFolder + "/" + tempFolder))
            {
                Directory.Delete(saveFolder + "/" + tempFolder, true);
            }
            // Delete Audio, TODO: remove hack
            if (File.Exists(saveFolder + "/Audio.wav"))
            {
                File.Delete(saveFolder + "/Audio.wav");
            }
        }

        public void EnableCamera()
        {
            m_CameraScreen.SetActive(true);
            m_CameraSphere.SetActive(false);
            m_Enabled = true;
        }

        public void StartCapture()
        {
            m_Capturing = true;
            m_CaptureIndex = 0;
            m_FrameIndex = 0;
            m_TotalCaptureTime = 0;
            m_CaptureText.SetActive(true);
            m_CurrentTempFolder = CreateTempFolder(m_SaveFolder);
        }

        public void FinishCapture()
        {
            m_Capturing = false;
            m_CaptureText.SetActive(false);
            if (m_EncodeVideo && m_EncodeImage)
            {
                // Calculate video framerate
                int framerate = (int)(1 / (m_TotalCaptureTime / m_CaptureIndex) / m_FrameInterval);
                #if UNITY_STANDALONE_OSX
                string platform = "OSX";
                #endif
                #if UNITY_STANDALONE_WIN
                string platform = "WIN";
                #endif
                string ffmpegPath = Application.dataPath + "/RockVRCapture/ThirdParty/FFmpeg/" + platform + "/";
                string capturesPath = Path.GetFullPath(string.Format(@"{0}/", m_SaveFolder));
                var thread = new Thread(delegate ()
                {
                    SaveVideo(framerate, ffmpegPath, capturesPath, m_SaveFolder, m_CurrentTempFolder, m_SaveFileName);
                });
                thread.Start();
            }
        }

        private static void SaveVideo(int framerate, string ffmpegPath, string capturesPath, string saveFolder, string tempFolder, string fileName)
        {
            string ffmpegArgs =
                    "-f image2 " +
                    "-r " + framerate + " " +
                    "-i \"" + capturesPath + tempFolder + "/" + fileName + "%d.jpg\" " +
                    "-vcodec libx264 " +
                    "-crf 25 " +
                    "-pix_fmt yuv420p \"" + capturesPath + fileName + "-" + tempFolder + ".mp4\"";
            if (File.Exists(capturesPath + "Audio.wav"))
            {
                ffmpegArgs =
                    "-f image2 " +
                    "-r " + framerate + " " +
                    "-i \"" + capturesPath + tempFolder + "/" + fileName + "%d.jpg\" " +
                    "-i \"" + capturesPath + "Audio.wav\" " +
                    "-vcodec libx264 " +
                    "-crf 25 " +
                    "-pix_fmt yuv420p " +
                    "-b:v 12000k " +
                    "-c:a aac " +
                    "-strict experimental " +
                    "-b:a 192k -shortest " +
                    "\"" + capturesPath + fileName + "-" + tempFolder + ".mp4\"";
                Debug.Log("Merging Audio...");
            }
            var processInfo = new System.Diagnostics.ProcessStartInfo(ffmpegPath + "ffmpeg", ffmpegArgs);
            processInfo.CreateNoWindow = false;
            var process = System.Diagnostics.Process.Start(processInfo);
            process.WaitForExit();
            process.Close();

            DeleteTempFolder(saveFolder, tempFolder);
            Debug.Log("Record Video Finish!");
        }
    }
}
