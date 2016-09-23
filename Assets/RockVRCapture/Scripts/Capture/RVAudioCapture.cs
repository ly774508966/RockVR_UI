using UnityEngine;
using System;
using System.Collections;
using System.IO;
using RockVR.Utils;

namespace RockVR.Capture {

    [RequireComponent (typeof(AudioListener))]
    public class RVAudioCapture : RVBaseCapture {

//        private int m_BufferSize;
//        private int m_NumBuffers;
        private bool m_StartCapturing = false;
        private int m_OutputRate = 44100;
        private int m_HeaderSize = 44; //default for uncompressed wav
        private FileStream m_FileStream;

        private void Awake() {
//            AudioSettings.outputSampleRate = m_OutputRate;
        }

        private new void Start() {
            base.Start ();
//            AudioSettings.GetDSPBufferSize(out m_BufferSize, out m_NumBuffers);
            if (m_StartCapture) {
                string savePath = m_SaveFolder + "/" + m_SaveFileName + "." + AudioFormat.wav;
                StartWriting(savePath);
                m_StartCapturing = true;
            }
        }

        private void Update() {
            // record by movement recorder
            if (m_StartCapturing && m_RecordedMovement != null) {
                if (m_FrameIndex == m_RecordedMovement.Length) {
                    m_StartCapturing = false;
                    WriteHeader();
                    Debug.Log ("Record Audio Finish!");
                }
                m_FrameIndex++;
            }
        }

        private void OnApplicationQuit() {
            if (!m_StartCapturing || m_UsingMotion) {
                return;
            }
            m_StartCapturing = false;
            WriteHeader();
            Debug.Log ("Record Audio Finish!");
        }

        private void StartWriting(string name) {
            m_FileStream = new FileStream(name, FileMode.Create);
            byte emptyByte = new byte();

            for (int i = 0; i < m_HeaderSize; i++) { //preparing the header
                m_FileStream.WriteByte(emptyByte);
            }
        }

        private void  OnAudioFilterRead(float[] data, int channels) {
            if (m_StartCapturing) {
                ConvertAndWrite(data); //audio data is interlaced
            }
        }

        private void ConvertAndWrite(float[] dataSource) {
            Int16[] intData = new Int16[dataSource.Length];
            //converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]

            Byte[] bytesData = new Byte[dataSource.Length * 2];
            //bytesData array is twice the size of
            //dataSource array because a float converted in Int16 is 2 bytes.

            float rescaleFactor = 32767; //to convert float to Int16
            for (int i = 0; i < dataSource.Length; i++) {
                intData[i] = (Int16)(dataSource[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }
            m_FileStream.Write(bytesData, 0, bytesData.Length);
        }

        private void  WriteHeader() {
            m_FileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            m_FileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(m_FileStream.Length - 8);
            m_FileStream.Write(chunkSize,0,4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            m_FileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            m_FileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            m_FileStream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            m_FileStream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(two);
            m_FileStream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(m_OutputRate);
            m_FileStream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(m_OutputRate * 4);
            // sampleRate * bytesPerSample*number of channels, here 44100*2*2

            m_FileStream.Write(byteRate, 0, 4);

            UInt16 four = 4;
            Byte[] blockAlign = BitConverter.GetBytes(four);
            m_FileStream.Write(blockAlign, 0, 2);

            UInt16 sixteen = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(sixteen);
            m_FileStream.Write(bitsPerSample, 0, 2);

            Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            m_FileStream.Write(dataString, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(m_FileStream.Length - m_HeaderSize);
            m_FileStream.Write(subChunk2, 0, 4);

            m_FileStream.Close();
        }
    }
}