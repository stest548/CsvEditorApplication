using System;
using System.Windows.Input;

namespace CsvEditorApplication.Commands
{
    /// <summary>
    /// ViewModelのメソッドをViewのコマンドにバインドするための汎用的なICommand実装。
    /// 注意: CommunityToolkit.MvvmのRelayCommandを利用することを推奨します。
    /// これは学習目的またはライブラリ非依存での実装例です。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// コマンドが実行可能かどうかに影響する要因が変更された場合に発生します。
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// 新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">コマンドの実行ロジック。</param>
        /// <param name="canExecute">コマンドの実行可否を判定するロジック。</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが現在の状態で実行できるかどうかを判断します。
        /// </summary>
        /// <param name="parameter">コマンドで使用されるデータ。コマンドがデータを必要としない場合は、このオブジェクトをnullに設定できます。</param>
        /// <returns>このコマンドを実行できる場合はtrue、それ以外の場合はfalse。</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// コマンドが呼び出されたときに実行されるメソッド。
        /// </summary>
        /// <param name="parameter">コマンドで使用されるデータ。コマンドがデータを必要としない場合は、このオブジェクトをnullに設定できます。</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
