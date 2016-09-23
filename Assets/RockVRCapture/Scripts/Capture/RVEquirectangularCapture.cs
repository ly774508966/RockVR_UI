using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using RockVR.Utils;

namespace RockVR.Capture {
    
    [RequireComponent(typeof(Camera))]
    public class RVEquirectangularCapture : RVBaseCapture {
        
        [SerializeField]
        private int m_CubemapSize = 1024;
        [SerializeField]
        private int m_JPGQuality = 75;
        [SerializeField]
        private int m_OutputWidth = 4096;
        [SerializeField]
        private int m_OutputHeight = 2048;
        [SerializeField]
        private bool m_EncodeImage = true;
        [SerializeField]
        private ImageFormat m_ImageFormat = ImageFormat.jpg;

        private Cubemap m_Cubemap;
        private Shader m_TransformShader;
        private Material m_TransformMaterial;
        private Camera m_Camera;

        private new void Start() {
            base.Start();
            // Clamp JPG image quality
            if (m_JPGQuality < 0) m_JPGQuality = 0;
            if (m_JPGQuality > 100) m_JPGQuality = 100;
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
            m_Cubemap = new Cubemap(m_CubemapSize, TextureFormat.RGB24, false);
            m_Camera = GetComponent<Camera>();
            // Set up transform shader
            m_TransformShader = Shader.Find("RockVR/CubemapToEquirectangular");
            m_TransformMaterial = new Material(m_TransformShader);
        }

        private bool IsReady() {
            if (m_Camera == null || m_Cubemap == null || !m_StartCapture ||
                m_TransformShader == null || m_TransformMaterial == null) {
                return false;
            }
            return true;
        }

        private void LateUpdate() {
            if (!IsReady()) return;
            // Render into cubemap
            m_Camera.RenderToCubemap(m_Cubemap);

            // Change to gamma color space
            //http://docs.unity3d.com/Manual/LinearLighting.html
            ColorSpace initialColorSpace = PlayerSettings.colorSpace;
            PlayerSettings.colorSpace = ColorSpace.Gamma;

            RenderTexture renderTexture = new RenderTexture(m_OutputWidth, m_OutputHeight, 24);
            RenderTexture.active = renderTexture;
            Graphics.Blit(m_Cubemap, renderTexture, m_TransformMaterial);

            Texture2D equirectangularTexture = new Texture2D(m_OutputWidth, m_OutputHeight, TextureFormat.ARGB32, false);

            equirectangularTexture.ReadPixels(new Rect(0, 0, m_OutputWidth, m_OutputHeight), 0, 0, false);
            equirectangularTexture.Apply();

            byte[] bytes = null;
            string extension = null;
            if (m_EncodeImage) {
                if (m_ImageFormat == ImageFormat.jpg) {
                    bytes = equirectangularTexture.EncodeToJPG (m_JPGQuality);
                } else {
                    bytes = equirectangularTexture.EncodeToPNG ();
                }
                extension = "." + m_ImageFormat;
            } else {
                bytes = equirectangularTexture.GetRawTextureData ();
                extension = ".rvt";
            }

            File.WriteAllBytes(m_SaveFolder + "/" + m_SaveFileName + m_FrameIndex + extension, bytes);

            //Revert color space
            PlayerSettings.colorSpace = initialColorSpace;

            Debug.Log("Record Image Frame: " + m_FrameIndex);
            m_FrameIndex++;

            // clean up
            RenderTexture.active = null;
            DestroyImmediate(renderTexture);
            DestroyImmediate(equirectangularTexture);
        }
    }
}