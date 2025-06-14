using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CsvEditorApplication.ViewModels
{
    /// <summary>
    /// INotifyPropertyChanged�����������A�SViewModel�̊��N���X�B(SYS-CLD-3.1.2)
    /// ����: CommunityToolkit.Mvvm��ObservableObject�𗘗p���邱�Ƃ𐄏����܂��B
    /// ����͊w�K�ړI�܂��̓��C�u������ˑ��ł̎�����ł��B
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// �v���p�e�B�l���ύX���ꂽ�Ƃ��ɔ������܂��B
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// PropertyChanged�C�x���g�𔭐������܂��B
        /// </summary>
        /// <param name="propertyName">�ύX���ꂽ�v���p�e�B�̖��O�B</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// �v���p�e�B�̃o�b�L���O�t�B�[���h��ݒ肵�A�ύX������Βʒm���܂��B
        /// </summary>
        /// <typeparam name="T">�v���p�e�B�̌^�B</typeparam>
        /// <param name="field">�v���p�e�B�̃o�b�L���O�t�B�[���h�ւ̎Q�ƁB</param>
        /// <param name="value">�V�����v���p�e�B�l�B</param>
        /// <param name="propertyName">�v���p�e�B�̖��O�B�ʏ�͎����I�Ɏ擾����܂��B</param>
        /// <returns>�l���ύX���ꂽ�ꍇ��true�A����ȊO��false�B</returns>
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
