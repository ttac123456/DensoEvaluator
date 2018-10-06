using System.ComponentModel;            // for INotifyPropertyChanged
using System.Runtime.CompilerServices;  // for CallerMemberName

namespace DensoEvaluator
{
    /// <summary>
    /// 画面バインド用のベースクラス
    /// </summary>
    /// <remarks>双方向にバインドするため、INotifyPropertyChangedインタフェースを実装。</remarks>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected void SetProperty<T>(ref T storage, T value,
                                    [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return;
            storage = value;
            OnPropertyChanged(propertyName);
        }
    }
}
