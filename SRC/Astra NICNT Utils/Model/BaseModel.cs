using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Astra_NICNT_Utils.Annotations;

namespace Astra_NICNT_Utils.Model
{
    [Serializable]
    public abstract class BaseModel : INotifyPropertyChanged
    {
        public event EventHandler<string> AddLog;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnAddLog(string e)
        {
            AddLog?.Invoke(this, e);
        }
    }
}
