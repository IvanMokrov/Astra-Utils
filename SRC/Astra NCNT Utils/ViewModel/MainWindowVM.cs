using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Astra_NICNT_Utils;
using Astra_NICNT_Utils.Annotations;
using Astra_NICNT_Utils.Model;
using Astra_NICNT_Utils.Utils;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Astra_NICNT_Utils.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged, IDataErrorInfo
    {
        // Generate samples for XAML Designer and Demo mode
        public bool IsInDemoMode;


        public event EventHandler<string> OnAddLog; 


        public ICommand BtnDeleteReg_Click => new RCommand( 
                                                        (param) => DeleteRegRecord(param), 
                                                        (param) => CanDeleteRegRecord(param));

        public ICommand BtnDeleteXml_Click => new RCommand(
                                                        (param) => DeleteXmlRecord(param),
                                                        (param) => CanDeleteXmlRecord(param));

        public ICommand BtnSourceFolder_Click => new RCommand(
                                                        (param) => SelectSourceLibFolder(param),
                                                        (param) => CanSelectSourceLibFolder(param));

        public ICommand BtnBuild_Click => new RCommand(
                                                        (param) => BuildProject(param),
                                                        (param) => CanBuildProject(param));


        private bool                _niOpenedState;
        private Geometry            _niButtonArrow;
        private readonly Geometry   _niButtonArrowLeft = Geometry.Parse("M41,11C42.6570014953613,11 44,12.3430004119873 44,14 44,14.9099998474121 43.5940017700195,15.7250003814697 42.9539985656738,16.2759990692139L27.2360000610352,31.9939994812012 43.1599998474121,47.9169998168945C43.6790008544922,48.4580001831055 44,49.1910018920898 44,50 44,51.6570014953613 42.6570014953613,53 41,53 40.1910018920898,53 39.4580001831055,52.6790008544922 38.9179992675781,52.1599998474121L38.9169998168945,52.1609992980957 20.9169998168945,34.1609992980957 20.9179992675781,34.1599998474121C20.3519992828369,33.6139984130859 20,32.8489990234375 20,32.0009994506836 20,31.9979991912842 20,31.996000289917 20,31.9939994812012 20,31.9909992218018 20,31.9890003204346 20,31.9869995117188 20,31.1389999389648 20.3519992828369,30.3740005493164 20.9179992675781,29.8279991149902L20.9169998168945,29.8269996643066 38.9169998168945,11.8269996643066 38.9249992370605,11.835000038147C39.4640007019043,11.3179998397827,40.1940002441406,11,41,11z");
        private readonly Geometry   _niButtonArrowRight = Geometry.Parse("M23,11C23.8059997558594,11,24.5359992980957,11.3179998397827,25.0750007629395,11.835000038147L25.0830001831055,11.8269996643066 43.0830001831055,29.8269996643066 43.0820007324219,29.8279991149902C43.6479988098145,30.3740005493164 44,31.1389999389648 44,31.9869995117188 44,31.9890003204346 44,31.992000579834 44,31.9939994812012 44,31.996000289917 44,31.9979991912842 44,32 44,32.8479995727539 43.6479988098145,33.6129989624023 43.0820007324219,34.1590003967285L43.0830001831055,34.1599998474121 25.0830001831055,52.1599998474121 25.0820007324219,52.1590003967285C24.5419998168945,52.6790008544922 23.8090000152588,53 23,53 21.3430004119873,53 20,51.6570014953613 20,50 20,49.1910018920898 20.3209991455078,48.4580001831055 20.8409996032715,47.9179992675781L20.8400001525879,47.9169998168945 36.7639999389648,31.9930000305176 21.0459995269775,16.2749996185303C20.4060001373291,15.7250003814697 20,14.9099998474121 20,14 20,12.3430004119873 21.3430004119873,11 23,11z");

        private CollectionViewSource                _xmlViewSource;
        private CollectionViewSource                _registryViewSource;

        private ObservableCollection<CRegistryData> _registryData;
        private ObservableCollection<CXmlData>      _xmlData;
        private NicntRootData                       _nicntData = new NicntRootData();

        private BitmapImage                         _wallpaper;


        #region Bindings
        //--------------------------------------------------------------------------------------------------------------------

        /// <summary> Selected item in RegistryData </summary>
        private CRegistryData SelectedRegistryData => (CRegistryData)RegistryViewSource.View.CurrentItem;

        /// <summary> Selected item in XmlData </summary>
        private CXmlData SelectedXmlData => (CXmlData)XmlViewSource.View.CurrentItem;

        /// <summary> Selected index in RegistryData </summary>
        private int SelectedRegistryIndex => RegistryViewSource.View.CurrentPosition;

        /// <summary> Selected index in XmlData </summary>
        private int SelectedXmlIndex => XmlViewSource.View.CurrentPosition;


        /// <summary> ViewSource for Windows </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public CollectionViewSource RegistryViewSource
        {
            get
            {
                if (_registryViewSource == null)
                {
                    _registryViewSource = new CollectionViewSource();
                    _registryViewSource.Source = RegistryData;
                    _registryViewSource.View.MoveCurrentToFirst();
                }
                return _registryViewSource;
            }
        }

        /// <summary> ViewSource for XML </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public CollectionViewSource XmlViewSource
        {
            get
            {
                if (_xmlViewSource == null)
                {
                    _xmlViewSource = new CollectionViewSource();
                    _xmlViewSource.Source = XmlData;
                    _xmlViewSource.View.MoveCurrentToFirst();
                }
                return _xmlViewSource;
            }
        }

        /// <summary> Dataset for windows registry </summary>
        /// <remarks> To refresh table just <c>RegistryData = null</c> </remarks>
        //--------------------------------------------------------------------------------------------------------------------
        public ObservableCollection<CRegistryData> RegistryData
        {
            get
            {
                if (_registryData == null)
                    _registryData = GetRegistryData();

                return _registryData;
            }
            set
            {
                if (value == null) // to refresh table just "RegistryData = null"
                    _registryData = GetRegistryData();
                else
                    _registryData = value;

                OnPropertyChanged();

                var cache = RegistryViewSource.GetStates();
                RegistryViewSource.Source = RegistryData;
                RegistryViewSource.SetStates(cache);
            }
        }

        /// <summary> Dataset for available KONTAKT xml </summary>
        /// <remarks> To refresh table just <c>XmlData = null</c> </remarks>
        //--------------------------------------------------------------------------------------------------------------------
        public ObservableCollection<CXmlData> XmlData
        {
            get
            {
                if (_xmlData == null)
                    _xmlData = GetXmlData();

                return _xmlData;
            }
            set
            {
                if (value == null) // to refresh table just "XmlData = null"
                    _xmlData = GetXmlData();
                else
                    _xmlData = value;

                OnPropertyChanged();

                var cache = XmlViewSource.GetStates();
                XmlViewSource.Source = XmlData;
                XmlViewSource.SetStates(cache);
            }
        }

        /// <summary> NI button arrow state binding </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public bool NIOpenedState
        {
            get { return _niOpenedState; }
            set
            {
                if (value == _niOpenedState) return;
                _niOpenedState = value;

                if (_niOpenedState)
                    NIButtonArrow = _niButtonArrowRight;
                else
                    NIButtonArrow = _niButtonArrowLeft;

                OnPropertyChanged();
            }
        }

        /// <summary> Arrow geomatry (left/right) bindings for NI button </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public Geometry NIButtonArrow
        {
            get
            {
                return _niButtonArrow;
            }
            set
            {
                if (Equals(value, _niButtonArrow)) return;

                _niButtonArrow = value;

                OnPropertyChanged();
            }
        }

        /// <summary> Main data about library </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public NicntRootData NicntData
        {
            get { return _nicntData; }
            set
            {
                if (Equals(value, _nicntData)) return;
                _nicntData = value;
                OnPropertyChanged();
            }
        }


        /// <summary> Cover image of samples library </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public BitmapImage Wallpaper
        {
            get { return _wallpaper; }
            set
            {
                if (Equals(value, _wallpaper)) return;
                _wallpaper = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region .ctor

        /// <summary> .ctor </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public MainWindowVM()
        {
            NIButtonArrow = _niButtonArrowLeft;

            // Validation errors
            NicntData.PropertyChanged += (sender, args) =>
            {
                if (NicntData[args.PropertyName] != null)
                    AddLog(NicntData[args.PropertyName]);
            };

            NicntData.AddLog += (sender, args) =>
            {
                AddLog(args);
            };


            // Generate samples for XAML Designer in Design mode
            if (DesignerProperties.GetIsInDesignMode(NIButtonArrow))
            {
                CreateDesignTimeData1(); // main window
                CreateDesignTimeData2(); // other windows
            }
            else
            {
                CreateDesignTimeData1();
                Uri uri = new Uri("pack://application:,,,/Resources/wallpapersample.png", UriKind.RelativeOrAbsolute);
                Wallpaper = new BitmapImage(uri);
                IsInDemoMode = true;
            }
        }


        /// <summary> Generate datasets for Demo and DesignTime XAML (main window) </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public void CreateDesignTimeData1()
        {
            NicntData.SourcePath = @"C:\My Samples\NI\Ensemble\SAM";
            NicntData.LibraryName = "Orchestral Essentials 2";
            NicntData.SNPID = "801";
            NicntData.VendorCompany = "Sonic Culture";

            NicntData.Instruments.Add(new NicntInstrument()
            {
                SourceFilePath = @"\Intstr\Piano1.nki",
                Bank = "Piano",
                DisplayName = "Piano1"
            });

            NicntData.Instruments.Add(new NicntInstrument()
            {
                SourceFilePath = @"\Intstr\Piano2.nki",
                Bank = "Piano",
                DisplayName = "Piano2"
            });

            NicntData.Instruments.Add(new NicntInstrument()
            {
                SourceFilePath = @"\Intstr\Piano333.nki",
                Bank = "Piano",
                DisplayName = "Piano333"
            });

            NicntData.Monoliths.Add(new NicntMonolith()
            {
                SourceFilePath = @"\Monolith\Piano444.nki",
                Bank = "Piano",
            });

            NicntData.Monoliths.Add(new NicntMonolith()
            {
                SourceFilePath = @"\Monolith\Piano555.nki",
                Bank = "Piano",
            });

            NicntData.Monoliths.Add(new NicntMonolith()
            {
                SourceFilePath = @"\Monolith\Piano777.nki",
                Bank = "Piano",
            });

        }


        /// <summary> Generate datasets for Demo and DesignTime XAML (other windows) </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public void CreateDesignTimeData2()
        {
            RegistryData = new ObservableCollection<CRegistryData>();
            XmlData = new ObservableCollection<CXmlData>();

            RegistryData.Add(new CRegistryData()
            {
                ContentPath = @"C:\NativeInstruments\Samples1",
                LibID = "k00143",
                LibraryName = "Drum n Bass 1"
            });

            RegistryData.Add(new CRegistryData()
            {
                ContentPath = @"C:\NativeInstruments\Samples2",
                LibID = "k00725",
                LibraryName = "Drum n Bass 2"
            });

            RegistryData.Add(new CRegistryData()
            {
                ContentPath = @"C:\NativeInstruments\Samples2",
                LibID = "k00725",
                LibraryName = "Drum n Bass 2"
            });

            XmlData.Add(new CXmlData()
            {
                LibraryName = "Classic piano 1",
                SnpID = "d5001",
                SourcePath = @"Frozen.xml",
                VendorCompany = "Frozen Plain Phoenix"
            });

            XmlData.Add(new CXmlData()
            {
                LibraryName = "Classic piano 2",
                SnpID = "d5001",
                SourcePath = @"Plain.xml",
                VendorCompany = "Frozen Plain Phoenix"
            });

            XmlData.Add(new CXmlData()
            {
                LibraryName = "Classic piano 2",
                SnpID = "d5002",
                SourcePath = @"Phoenix.xml",
                VendorCompany = "Frozen Plain Phoenix"
            });

            RegistryViewSource.Source = RegistryData;
            XmlViewSource.Source = XmlData;
        }

        #endregion


        #region Private

        /// <summary> Enable delete from registry button </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private bool CanDeleteRegRecord(object param)
        {
            if (SelectedRegistryData != null)
            {
                return true;
            }

            return false;
        }

        /// <summary> Delete record from Windows registry </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void DeleteRegRecord(object param)
        {
            if (IsInDemoMode)
            {
                AddLog("Is in Design mode");
                return;
            }

            if (UAC.IsUserAdministrator() && SelectedRegistryIndex >= 0 && SelectedRegistryData != null)
            {

                try
                {
                    using (RegistryKey keyLM = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Native Instruments", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        using (RegistryKey keyCU = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Native Instruments", RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {

                            if (keyLM == null)
                            {
                                AddLog("Scanning registry (HKLM): Native Instruments is not installed");
                                return;
                            }
                            if (keyCU == null)
                            {
                                AddLog("Scanning registry (HKCU): Native Instruments is not installed");
                                return;
                            }

                            AddLog("Deleting record from registry...");

                            if (!string.IsNullOrEmpty(SelectedRegistryData.LibraryName))
                            {
                                if (keyLM.GetSubKeyNames().Any( x => x.ToUpper() == SelectedRegistryData.LibraryName.ToUpper()))
                                   keyLM.DeleteSubKey(SelectedRegistryData.LibraryName);
                                if (keyCU.GetSubKeyNames().Any(x => x.ToUpper() == SelectedRegistryData.LibraryName.ToUpper()))
                                    keyCU.DeleteSubKey(SelectedRegistryData.LibraryName);
                            }

                            if (!string.IsNullOrEmpty(SelectedRegistryData.LibID))
                            {
                                using (RegistryKey keyLMContent = keyLM.OpenSubKey(@"Content", RegistryKeyPermissionCheck.ReadWriteSubTree))
                                {
                                    if (keyLMContent == null)
                                    {
                                        AddLog("Scanning registry (HKLM): WARINING. No KONTAKT Content key found");
                                    }
                                    else
                                    {
                                        keyLMContent.DeleteValue(SelectedRegistryData.LibID);
                                        keyLMContent.Close();
                                    }
                                }
                            }

                            AddLog("Deleting registry data complete");

                            keyCU.Close();
                            keyLM.Close();
                        }
                    }

                    // refresh datasource and repositioning
                    RegistryData.Remove(SelectedRegistryData); // = null;

                }
                catch (Exception ex)
                {
                    AddLog(ex.ToString());
                }
            }
        }

        /// <summary> Enable delete xml button </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private bool CanDeleteXmlRecord(object param)
        {
            if (SelectedXmlData != null)
            {
                return true;
            }

            return false;
        }

        /// <summary> Delete XML data </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void DeleteXmlRecord(object param)
        {
            if (!UAC.IsUserAdministrator())
            {
                MessageBox.Show("Required administrator mode");
            }


            if (UAC.IsUserAdministrator() && SelectedXmlIndex >= 0 && SelectedXmlData != null)
            {
                try
                {
                    AddLog("Deleting Xml data....");

                    ObservableCollection<CXmlData> result = new ObservableCollection<CXmlData>();

                    string common = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

                    if (!Directory.Exists(common + @"\Native Instruments"))
                    {
                        AddLog("Xml's: WARINING. Native Instruments is not installed");
                        return;
                    }

                    if (!Directory.Exists(common + @"\Native Instruments\Service Center"))
                    {
                        AddLog("Xml's: WARINING. NI Service Center is not installed");
                        return;
                    }

                    string SCPath = common + @"\Native Instruments\Service Center\";

                    List<string> files = Directory.GetFiles(SCPath, "*.xml")
                        .Where(
                            f =>
                                !f.ToUpper().Contains("Kontakt 5.xml".ToUpper()) &&
                                !f.ToUpper().Contains("Kontakt 5 App.xml".ToUpper()))
                        .Select(f => Path.GetFileName(f))
                        .ToList();

                    if (!files.Contains(SelectedXmlData.SourcePath))
                    {
                        AddLog("Xml's: WARINING. Source file not found");
                        return;
                    }

                    XmlDocument doc = new XmlDocument();
                    doc.Load(SCPath + SelectedXmlData.SourcePath);

                    XmlNodeList nodeListT = doc.ChildNodes;

                    if (nodeListT == null)
                        return;

                    bool deletionComplete = false;

                    foreach (XmlNode nodeT in nodeListT)
                    {
                        if (nodeT.Name.ToUpper() == "ProductHints".ToUpper())
                        {

                            XmlNode nodeToDelete = null;

                            XmlNodeList nodeB = nodeT.ChildNodes;

                            foreach (XmlNode node in nodeB)
                            {
                                if (!node.HasChildNodes || node.Name != "Product")
                                    continue;

                                CXmlData REC = new CXmlData();

                                foreach (XmlNode element in node.ChildNodes)
                                {
                                    switch (element.Name)
                                    {
                                        case "Name":
                                            REC.LibraryName = element.InnerText;
                                            break;
                                        case "Company":
                                            REC.VendorCompany = element.InnerText;
                                            break;
                                        case "SNPID":
                                            REC.SnpID = element.InnerText;
                                            break;
                                    }
                                }

                                if (REC.LibraryName == SelectedXmlData.LibraryName && REC.SnpID == SelectedXmlData.SnpID)
                                {
                                    nodeToDelete = node;
                                    break;
                                }
                            }

                            if (nodeToDelete != null)
                            {
                                nodeT.RemoveChild(nodeToDelete);
                                deletionComplete = true;
                            }

                        }
                    }

                    if (deletionComplete)
                    {
                        doc.Save(SCPath + SelectedXmlData.SourcePath);
                        AddLog("Deleting Xml data complete");

                        DeleteXmlRecordCheckFile();

                        // refresh datasource and repositioning
                        XmlData.Remove(SelectedXmlData); // = null;
                    }
                }
                catch (Exception ex)
                {
                    AddLog(ex.ToString());
                }
            }
        }

        /// <summary> Delete file -- they have no more instruments </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void DeleteXmlRecordCheckFile()
        {
            if (!UAC.IsUserAdministrator())
            {
                MessageBox.Show("Required administrator mode");
            }

            try
            {
                XmlDocument doc = new XmlDocument();

                string common = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                string SCPath = common + @"\Native Instruments\Service Center\";
                doc.Load(SCPath + SelectedXmlData.SourcePath);

                XmlNodeList nodeListT = doc.ChildNodes;

                if (nodeListT == null)
                    return;

                bool instrumentsIsAvailable = false;

                foreach (XmlNode nodeT in nodeListT)
                {
                    if (nodeT.Name.ToUpper() == "ProductHints".ToUpper())
                    {
                        XmlNodeList nodeB = nodeT.ChildNodes;

                        foreach (XmlNode node in nodeB)
                        {
                            if (node.Name == "Product")
                            {
                                instrumentsIsAvailable = true;
                                break;
                            }
                        }
                    }
                }

                if (!instrumentsIsAvailable)
                {
                    File.Delete(SCPath + SelectedXmlData.SourcePath);
                }
            }
            catch (Exception ex)
            {
                AddLog(ex.ToString());
            }
        }

        /// <summary> Enable select source library button </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private bool CanSelectSourceLibFolder(object param)
        {
            if (!CommonFileDialog.IsPlatformSupported)
            {
                AddLog("In this realization your Windows must be older than Vista");
            }

            return true;
        }

        /// <summary> Select source library path </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void SelectSourceLibFolder(object param)
        {
            if (!CommonFileDialog.IsPlatformSupported)
            {
                AddLog("In this realization your Windows must be older than Vista");
                return;
            }

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                dialog.AddToMostRecentlyUsedList = true;
                dialog.EnsurePathExists = true;
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    NicntData.SourcePath = dialog.FileName;
                }
            }
        }

        /// <summary> Enable build button </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private bool CanBuildProject(object param)
        {
            return true;
        }

        /// <summary> Start build </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void BuildProject(object param)
        {
            AddLog("Mabuta1 asdf asdf as.ld,fj a;sdf asdhf laksdjfh aslkdjfh laksdjfh aslkdf hlaskdfh laksdhfj alskdfh laskdfh jlaskdhf laskdf hlaskdjfh alskdfh alskdfh alskdhf j");
            AddLog("Not yet implemened");
            AddLog("Not yet implemened");
            AddLog("Not yet implemened");
        }

        /// <summary> Returns a Dataset from Windows registry </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private ObservableCollection<CRegistryData> GetRegistryData()
        {

            AddLog("Scanning registry....");

            ObservableCollection<CRegistryData> result = new ObservableCollection<CRegistryData>();

            RegistryKey keyLM = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Native Instruments1");
            if (keyLM == null)
            {
                AddLog("Scanning registry (HKLM): Native Instruments is not installed");
                return new ObservableCollection<CRegistryData>();
            }

            AddLog(keyLM.SubKeyCount.ToString());
            
            string[] names = keyLM.GetSubKeyNames();
            foreach (string name in names)
            {
                RegistryKey key = keyLM.OpenSubKey(name);

                object path = key.GetValue("ContentDir");

                if (path != null)
                {
                    result.Add(new CRegistryData()
                                        {
                                            LibraryName = name,
                                            ContentPath = (string)path,
                                        });
                }
            }

            if (result.Count == 0)
            {
                AddLog("Scanning registry (HKLM): No KONTAKT instruments detected");
            }

            // check consistency

            RegistryKey keyLMContent = keyLM.OpenSubKey(@"Content");

            if (keyLMContent == null)
            {
                AddLog("Scanning registry (HKLM): WARINING. No KONTAKT Content key found");
                return result;
            }

            foreach (string id in keyLMContent.GetValueNames())
            {
                object name = keyLMContent.GetValue(id);

                if (name != null)
                {
                    if (name != null && (string)name != "UserPatches")
                    {
                        if (result.Any(v => v.LibraryName == (string)name))
                        {
                            var rec = result.First(v => v.LibraryName == (string)name);
                            rec.LibID = id;
                        }
                        else
                        {
                            AddLog($"Scanning registry (HKLM): WARINING. For k2lib-key {id} registered library {name}, but this library name not exist in registry path");
                            result.Add(new CRegistryData()
                            {
                                LibraryName = (string)name,
                                LibID = id,
                            });
                        }
                    }
                }
                else
                {
                    AddLog($"Scanning registry (HKLM): WARINING. For key {id} record is empty and have no value");
                    result.Add(new CRegistryData()
                    {
                        LibID = id,
                    });
                }
            }

            AddLog("Scanning registry complete");
            return result;
        }

        /// <summary> Returns a Dataset from XML files </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private ObservableCollection<CXmlData> GetXmlData()
        {

            AddLog("Scanning Xml's....");

            ObservableCollection<CXmlData> result = new ObservableCollection<CXmlData>();

            string common = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

            if (!Directory.Exists(common + @"\Native Instruments1"))
            {
                AddLog("Scanning Xml's: WARINING. Native Instruments is not installed");
                return result;
            }

            if (!Directory.Exists(common + @"\Native Instruments\Service Center"))
            {
                AddLog("Scanning Xml's: WARINING. NI Service Center is not installed");
                return result;
            }

            string SCPath = common + @"\Native Instruments\Service Center\";

            List<string> files = Directory.GetFiles(SCPath, "*.xml")
                                                .Where( f => !f.ToUpper().Contains("Kontakt 5.xml".ToUpper()) && !f.ToUpper().Contains("Kontakt 5 App.xml".ToUpper()))
                                                .Select( f => Path.GetFileName(f))
                                                .ToList();

            foreach (string file in files)
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(SCPath + file);

                XmlNodeList nodeListT = doc.ChildNodes;

                foreach (XmlNode nodeT in nodeListT)
                {
                    if (nodeT.Name.ToUpper() == "ProductHints".ToUpper())
                    {
                        XmlNodeList nodeB = nodeT.ChildNodes;

                        foreach (XmlNode node in nodeB)
                        {
                            if (!node.HasChildNodes)
                                continue;

                            CXmlData REC = new CXmlData();
                            REC.SourcePath = file;

                            foreach (XmlNode element in node.ChildNodes)
                            {
                                switch (element.Name)
                                {
                                    case "Name":
                                        REC.LibraryName = element.InnerText;
                                        break;
                                    case "Company":
                                        REC.VendorCompany = element.InnerText;
                                        break;
                                    case "SNPID":
                                        REC.SnpID = element.InnerText;
                                        break;
                                }
                            }

                            if (!string.IsNullOrEmpty(REC.SnpID))
                                result.Add(REC);
                        }
                    }
                }
            }

            AddLog("Scanning Xml's complete");
            return result;
        }

        #endregion


        #region PropertyChanged & Validation


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string Error => string.Empty;


        public string this[string checkPropName] => Validate(checkPropName);


        private string Validate(string checkPropName)
        {
            //switch (checkPropName)
            //{
            //    //TODO Property validation here
            //}

            return null;
        }


        #endregion

        protected virtual void AddLog(string e)
        {
            Console.WriteLine(e);
            OnAddLog?.Invoke(this, e);
        }

    }

}

