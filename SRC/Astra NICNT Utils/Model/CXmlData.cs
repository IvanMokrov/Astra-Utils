using System;

namespace Astra_NICNT_Utils.Model
{

    [Serializable]
    public class CXmlData : BaseModel
    {

        private string _libraryName;
        private string _snpId;
        private string _sourcePath;
        private string _vendorCompany;


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


        public string VendorCompany
        {
            get { return _vendorCompany; }
            set
            {
                if (value == _vendorCompany) return;
                _vendorCompany = value;
                OnPropertyChanged();
            }
        }


        public string SnpID
        {
            get { return _snpId; }
            set
            {
                if (value == _snpId) return;
                _snpId = value;
                OnPropertyChanged();
            }
        }


        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (value == _sourcePath) return;
                _sourcePath = value;
                OnPropertyChanged();
            }
        }


    }
}
