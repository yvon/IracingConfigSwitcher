using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace IracingConfigSwitcher
{
    class CurrentConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _alt;
        private string _iRacingFolder;

        public CurrentConfig()
        {
            _alt = Properties.Settings.Default.AltConfig;
            _iRacingFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\iRacing";
        }
        public string Name(bool other = false)
        {
            bool alt = other ? !_alt : _alt;
            return alt ? "alternative" : "default";
        }
        public void Switch()
        {
            Properties.Settings.Default.AltConfig = _alt = !_alt;
            Properties.Settings.Default.Save();

            NotifyPropertyChanged("LabelContent");
            NotifyPropertyChanged("ButtonContent");

            backupCurrentConfig("app");
            backupCurrentConfig("rendererDX11");
            applyOtherConfig("app");
            applyOtherConfig("rendererDX11");
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public string LabelContent => $"Current config: {Name()}".ToUpper();
        public string ButtonContent => $"Load {Name(true)}".ToUpper();

        private void backupCurrentConfig(string baseName)
        {
            File.Copy(currentConfigPath(baseName), backupConfigPath(baseName), true);
        }

        private void applyOtherConfig(string baseName)
        {
            string source = backupConfigPath(baseName, true);

            if (File.Exists(source))
            {
                File.Copy(source, currentConfigPath(baseName), true);
            }
        }

        private string currentConfigPath(string baseName)
        {
            return $"{_iRacingFolder}\\{baseName}.ini";
        }

        private string backupConfigPath(string baseName, bool other = false)
        {
            return $"{baseName}.{Name(other)}.ini";
        }
    }
}
