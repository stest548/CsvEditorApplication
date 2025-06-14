using System;
using System.Windows.Input;

namespace CsvEditorApplication.Commands
{
    /// <summary>
    /// ViewModel�̃��\�b�h��View�̃R�}���h�Ƀo�C���h���邽�߂̔ėp�I��ICommand�����B
    /// ����: CommunityToolkit.Mvvm��RelayCommand�𗘗p���邱�Ƃ𐄏����܂��B
    /// ����͊w�K�ړI�܂��̓��C�u������ˑ��ł̎�����ł��B
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// �R�}���h�����s�\���ǂ����ɉe������v�����ύX���ꂽ�ꍇ�ɔ������܂��B
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// �V�����R�}���h���쐬���܂��B
        /// </summary>
        /// <param name="execute">�R�}���h�̎��s���W�b�N�B</param>
        /// <param name="canExecute">�R�}���h�̎��s�ۂ𔻒肷�郍�W�b�N�B</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// �R�}���h�����݂̏�ԂŎ��s�ł��邩�ǂ����𔻒f���܂��B
        /// </summary>
        /// <param name="parameter">�R�}���h�Ŏg�p�����f�[�^�B�R�}���h���f�[�^��K�v�Ƃ��Ȃ��ꍇ�́A���̃I�u�W�F�N�g��null�ɐݒ�ł��܂��B</param>
        /// <returns>���̃R�}���h�����s�ł���ꍇ��true�A����ȊO�̏ꍇ��false�B</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// �R�}���h���Ăяo���ꂽ�Ƃ��Ɏ��s����郁�\�b�h�B
        /// </summary>
        /// <param name="parameter">�R�}���h�Ŏg�p�����f�[�^�B�R�}���h���f�[�^��K�v�Ƃ��Ȃ��ꍇ�́A���̃I�u�W�F�N�g��null�ɐݒ�ł��܂��B</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
