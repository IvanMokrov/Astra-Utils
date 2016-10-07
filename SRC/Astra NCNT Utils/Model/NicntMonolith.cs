using System;

namespace Astra_NICNT_Utils.Model
{
    /// <summary>
    /// Data class for Monolith (Multi's) type instruments
    /// </summary>
    [Serializable]
    public class NicntMonolith : BaseModel
    {

        private string _sourceFilePath;
        private string _bank;

        /// <summary> Relative source path for NKX monolith </summary>
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


        /// <summary> Multi-bank name </summary>
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
