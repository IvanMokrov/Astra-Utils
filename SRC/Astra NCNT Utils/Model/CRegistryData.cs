using System;

namespace Astra_NICNT_Utils.Model
{

    [Serializable]
    public class CRegistryData : BaseModel
    {

        private string _libraryName;
        private string _libId;
        private string _contentPath;


        public string LibraryName
        {
            get { return _libraryName; }
            set
            {
                if (value == _libraryName) return;
                _libraryName = value;
                OnPropertyChanged();
            }
        }


        public string LibID
        {
            get { return _libId; }
            set
            {
                if (value == _libId) return;
                _libId = value;
                OnPropertyChanged();
            }
        }


        public string ContentPath
        {
            get { return _contentPath; }
            set
            {
                if (value == _contentPath) return;
                _contentPath = value;
                OnPropertyChanged();
            }
        }

    }
}
