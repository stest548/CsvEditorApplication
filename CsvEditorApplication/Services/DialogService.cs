using Microsoft.Win32;
using System.Windows;

namespace CsvEditorApplication.Services
{
    /// <summary>
    /// IDialogServiceインターフェースの実装クラス。 (U002-CLD-3.2.4)
    /// WPFの標準ダイアログを実際に表示する処理を実装します。
    /// </summary>
    public class DialogService : IDialogService
    {
        private const string CsvFilter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";

        public string? ShowOpenFileDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = CsvFilter
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? ShowSaveFileDialog()
        {
            var dialog = new SaveFileDialog
            {
                Filter = CsvFilter
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public bool? ShowConfirmationDialog(string message, string title)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }

        public void ShowErrorDialog(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
