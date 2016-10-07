using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Astra_NICNT_Utils.Model
{

    /// <summary>
    /// Root data for .nicnt file
    /// </summary>
    [Serializable]
    public class NicntRootData : BaseModel, IDataErrorInfo
    {

        private string _sourcePath;
        private string _libraryName;
        private string _vendorCompany;
        private string _upid;
        private string _hu;
        private string _jdx;
        private BitmapImage _coverImage;
        private ObservableCollection<NicntInstrument> _instruments = new ObservableCollection<NicntInstrument>();
        private ObservableCollection<NicntMonolith> _monoliths = new ObservableCollection<NicntMonolith>();
        private string _snpid;


        /// <summary> Source folder, same as root of src .nicnt </summary>
        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (value == _sourcePath) return;
                _sourcePath = value;

                RebuildSourcePathData();

                OnPropertyChanged();
            }
        }


        /// <summary> Name of the library </summary>
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


        /// <summary> Vendor company name for this library </summary>
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


        /// <summary> ID in library database (required, unique) HEX </summary>
        public string SNPID
        {
            get { return _snpid; }
            set
            {
                if (value == _snpid) return;
                _snpid = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Special production code (not required) bytes mask: 8b-4b-4b-12b </summary> 
        public string UPID
        {
            get { return _upid; }
            set
            {
                if (value == _upid) return;
                _upid = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Special heading id (required, unique) bytes mask: 32b </summary>
        public string HU
        {
            get { return _hu; }
            set
            {
                if (value == _hu) return;
                _hu = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Special unknown id (required, unique) bytes mask: 64b </summary>
        public string JDX
        {
            get { return _jdx; }
            set
            {
                if (value == _jdx) return;
                _jdx = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Wallpaper for library </summary>
        public BitmapImage CoverImage
        {
            get { return _coverImage; }
            set
            {
                if (Equals(value, _coverImage)) return;
                _coverImage = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Instruments in library </summary>
        public ObservableCollection<NicntInstrument> Instruments
        {
            get { return _instruments; }
            set
            {
                if (Equals(value, _instruments)) return;
                _instruments = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Multi's in library </summary>
        public ObservableCollection<NicntMonolith> Monoliths
        {
            get { return _monoliths; }
            set
            {
                if (Equals(value, _monoliths)) return;
                _monoliths = value;
                OnPropertyChanged();
            }
        }


        public string Error { get; }

        public string this[string checkPropName] => Validate(checkPropName);

        private string Validate(string checkPropName)
        {
            //switch (checkPropName)
            //{
            //    // TODO Property validation here
            //}

            return null;
        }


        [Obsolete("(Check it) Code required refreshing to get all file types")]
        private void RebuildSourcePathData()
        {
            DirectoryInfo root = new DirectoryInfo(SourcePath);

            List<FileInfo> files = new List<FileInfo>();

            if (root.Exists)
            {

                try
                {
                    files.AddRange(root.EnumerateFiles().ToList());

                    foreach (DirectoryInfo directory in root.EnumerateDirectories())
                    {
                        files.AddRange(RebuildSourcePathDataRecurse(directory));
                    }
                }
                catch (Exception)
                {
                    OnAddLog("Fail: do NOT use for Native Instruments non-english folders, hardlinks and file names");
                }
            }
        }


        private List<FileInfo> RebuildSourcePathDataRecurse(DirectoryInfo directory)
        {
            List<FileInfo> result = directory.EnumerateFiles().ToList();

            foreach (DirectoryInfo subdirectory in directory.EnumerateDirectories())
            {
                result.AddRange(RebuildSourcePathDataRecurse(subdirectory));
            }

            return result;
        }


    }
}
