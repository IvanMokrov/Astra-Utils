using System;

namespace Astra_NICNT_Utils.Model
{
    /// <summary>
    /// NKI instrument
    /// </summary>
    [Serializable]
    public class NicntInstrument : BaseModel
    {

        private string _sourceFilePath;
        private string _displayName;
        private string _bank;


        /// <summary> Relative source path for NKI instrument </summary>
        public string SourceFilePath
        {
            get { return _sourceFilePath; }
            set
            {
                if (value == _sourceFilePath) return;
                _sourceFilePath = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Display name for instrument </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Sample-bank name </summary>
        public string Bank
        {
            get { return _bank; }
            set
            {
                if (value == _bank) return;
                _bank = value;
                OnPropertyChanged();
            }
        }

    }
}
