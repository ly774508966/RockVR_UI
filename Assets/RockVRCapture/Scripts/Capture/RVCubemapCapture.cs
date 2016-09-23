using UnityEngine;
using UnityEditor;
using System.IO;
using RockVR.Utils;

namespace RockVR.Capture {

    [RequireComponent (typeof(Camera))]
    public class RVCubemapCapture : RVBaseCapture {

        [SerializeField]
        private int m_CubemapSize = 1024;
        [SerializeField]
        private bool m_EncodeImage = true;
        [SerializeField]
        private int m_JPGQuality = 75;
        [SerializeField]
        private ImageFormat m_ImageFormat = ImageFormat.jpg;
        [SerializeField]
        private int m_CaptureFrameSize = 3000;

        private Cubemap m_Cubemap;
        private Camera m_Camera;

        private new void Start() {
            base.Start ();
            // Clamp JPG image quality
            if (m_JPGQuality < 0) m_JPGQuality = 0;
            if (m_JPGQuality > 100) m_JPGQuality = 100;
            Camera[] cameras = Camera.allCameras;
            // disable other cameras except the capture camera
            foreach (Camera camera in cameras) {
                if (camera.gameObject.name != gameObject.name) {
                    camera.gameObject.SetActive (false);
                }
            }
            // Set maximum allowed timestep correct
            Time.maximumDeltaTime = Time.fixedDeltaTime;
            // Correct cubemap size if not power of 2
            if (!MathematicsUtils.IsPowerOfTwo(m_CubemapSize)) {
                int powerOf2 = 2;
                while (powerOf2 < m_CubemapSize) {
                    powerOf2 *= 2;
                }
                m_CubemapSize = powerOf2;
            }
            // Create render cubemap
            m_Cubemap = new Cubemap (m_CubemapSize, TextureFormat.RGB24, false);
            m_Camera = GetComponent<Camera> ();
        }

        private bool IsReady() {
            if (m_Camera == null || m_Cubemap == null || !m_StartCapture) {
                return false;
            }
            return true;
        }

        private void LateUpdate () {
            if (!IsReady ()) {
                return;
            }
            // render into cubemap
            m_Camera.RenderToCubemap (m_Cubemap);
            // save to texture2d
            Texture2D texture = new Texture2D (m_Cubemap.width * 4, m_Cubemap.height * 3, TextureFormat.RGB24, false);
            texture.SetPixels (
                m_Cubemap.width,
                0,
                m_Cubemap.width,
                m_Cubemap.height,
                ImageUtils.FlipPixelsY(m_Cubemap.GetPixels (CubemapFace.NegativeY), m_Cubemap.width, m_Cubemap.height)
            );
            texture.SetPixels (
                0,
                m_Cubemap.height,
                m_Cubemap.width,
                m_Cubemap.height,
                ImageUtils.FlipPixelsY(m_Cubemap.GetPixels (CubemapFace.NegativeX), m_Cubemap.width, m_Cubemap.height)
            );
            texture.SetPixels (
                m_Cubemap.width,
                m_Cubemap.height,
                m_Cubemap.width,
                m_Cubemap.height,
                ImageUtils.FlipPixelsY(m_Cubemap.GetPixels (CubemapFace.PositiveZ), m_Cubemap.width, m_Cubemap.height)
            );
            texture.SetPixels (
                m_Cubemap.width * 2,
                m_Cubemap.height,
                m_Cubemap.width,
                m_Cubemap.height,
                ImageUtils.FlipPixelsY(m_Cubemap.GetPixels (CubemapFace.PositiveX), m_Cubemap.width, m_Cubemap.height)
            );
            texture.SetPixels (
                m_Cubemap.width * 3,
                m_Cubemap.height,
                m_Cubemap.width,
                m_Cubemap.height,
                ImageUtils.FlipPixelsY(m_Cubemap.GetPixels (CubemapFace.NegativeZ), m_Cubemap.width, m_Cubemap.height)
            );
            texture.SetPixels (
                m_Cubemap.width,
                m_Cubemap.height * 2,
                m_Cubemap.width,
                m_Cubemap.height,
                ImageUtils.FlipPixelsY(m_Cubemap.GetPixels (CubemapFace.PositiveY), m_Cubemap.width, m_Cubemap.height)
            );
            // convert to png
            byte[] bytes = null;
            string extension = null;
            if (m_EncodeImage) {
                if (m_ImageFormat == ImageFormat.jpg) {
                    bytes = texture.EncodeToJPG (m_JPGQuality);
                } else {
                    bytes = texture.EncodeToPNG ();
                }
                extension = "." + m_ImageFormat;
            } else {
                bytes = texture.GetRawTextureData ();
                extension = ".rvt";
            }
            File.WriteAllBytes(m_SaveFolder + "/" + m_FrameIndex + extension, bytes);
            DestroyImmediate(texture);
            float captureSize = m_UsingMotion ? m_RecordedMovement.Length : m_CaptureFrameSize;
            Debug.Log ("Record Image Frame: " + m_FrameIndex + " (" + m_FrameIndex / captureSize * 100 + "%)");
            m_FrameIndex++;
        }
    }
}