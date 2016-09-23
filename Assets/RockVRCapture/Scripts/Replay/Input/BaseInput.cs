using System;

namespace RockVR.Replay {

    public abstract class BaseInput<T> where T : IRecordInputInfo
    {
        private RVInputMode m_InputMode;
        protected Recording<T> m_CurrentRecording;

        public BaseInput(RVInputMode inputMode)
        {
            m_InputMode = inputMode;
        }

        public RVInputMode CurrentInputMode
        {
            get
            {
                return m_InputMode;
            }
            set
            {
                m_InputMode = value;
            }
        }

        public Recording<T> CurrentRecording
        {
            get {
                if (m_CurrentRecording == null) {
                    m_CurrentRecording = new Recording<T>();
                }
                return m_CurrentRecording;
            }
            set {
                m_CurrentRecording = value;
            }
        }

        public virtual string SaveFolderName()
        {
            return "RVCaptures";
        }

        public virtual string SaveFileName ()
        {
            return "BaseInputFile";
        }

        public string SaveFilePath()
        {
            return SaveFolderName() + "/" + SaveFileName();
        }

        public abstract RVInputMethod InputMethod ();

        /// <summary>
        /// Saves the record file as JSON format.
        /// </summary>
        public abstract void SaveRecordFile();

        /// <summary>
        /// Loads the JSON record file and deserialize.
        /// </summary>
        public abstract void LoadRecordFile();
    }
}
