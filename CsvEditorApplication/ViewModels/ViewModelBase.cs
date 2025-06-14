using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CsvEditorApplication.ViewModels
{
    /// <summary>
    /// INotifyPropertyChangedを実装した、全ViewModelの基底クラス。(SYS-CLD-3.1.2)
    /// 注意: CommunityToolkit.MvvmのObservableObjectを利用することを推奨します。
    /// これは学習目的またはライブラリ非依存での実装例です。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ値が変更されたときに発生します。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// PropertyChangedイベントを発生させます。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティの名前。</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティのバッキングフィールドを設定し、変更があれば通知します。
        /// </summary>
        /// <typeparam name="T">プロパティの型。</typeparam>
        /// <param name="field">プロパティのバッキングフィールドへの参照。</param>
        /// <param name="value">新しいプロパティ値。</param>
        /// <param name="propertyName">プロパティの名前。通常は自動的に取得されます。</param>
        /// <returns>値が変更された場合はtrue、それ以外はfalse。</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
