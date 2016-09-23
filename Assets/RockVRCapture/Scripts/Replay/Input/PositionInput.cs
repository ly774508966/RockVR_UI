using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace RockVR.Replay
{
    /// <summary>
    /// Position Input.
    /// Record game object's position, rotation, etc.
    /// </summary>
    public class PositionInput : BaseInput<PositionInputInfo>
    {
        public PositionInput(RVInputMode inputMode): base(inputMode) {}

        public void SetPosition(GameObject gameObject)
        {
            BaseInputInfo inputInfo = CurrentRecording.CurrentFrame.GetInputInfo(gameObject.name);
            if (inputInfo == null)
            {
                inputInfo = new PositionInputInfo();
            }
            PositionInputInfo positionInputInfo = (PositionInputInfo)inputInfo;
            positionInputInfo.Positon = gameObject.transform.position;
            positionInputInfo.Rotation = gameObject.transform.rotation;
            CurrentRecording.CurrentFrame.AddInputInfo(gameObject.name, positionInputInfo);
        }

        public PositionInputInfo GetPosition(GameObject gameObject)
        {
            PositionInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    (PositionInputInfo)CurrentRecording.CurrentFrame.GetInputInfo(gameObject.name) : null;
            return inputInfo;
        }

        public PositionInputInfo GetPosition(string objectName)
        {
            PositionInputInfo inputInfo = CurrentRecording.HasCurrentFrame() ?
                    (PositionInputInfo)CurrentRecording.CurrentFrame.GetInputInfo(objectName) : null;
            return inputInfo;
        }

        public override RVInputMethod InputMethod()
        {
            return RVInputMethod.ObjectPosition;
        }

        public override string SaveFileName()
        {
            return "PositionInputFile";
        }

        public override void SaveRecordFile() {
            string json = JsonConvert.SerializeObject(CurrentRecording);
            StreamWriter writer = File.CreateText(SaveFilePath());
            writer.WriteLine(json);
            writer.Close();
            Debug.Log("SaveRecordFile : " + SaveFileName());
        }

        public override void LoadRecordFile() {
            if (!File.Exists(SaveFilePath()))
            {
                throw new FileNotFoundException(SaveFilePath() + " not found!");
            }
            StreamReader reader = new StreamReader(SaveFilePath());
            string content = reader.ReadToEnd();
            reader.Close();
            CurrentRecording = JsonConvert.DeserializeObject<Recording<PositionInputInfo>>(content);
            Debug.Log("LoadRecordFile : " + SaveFileName());
        }
    }
}