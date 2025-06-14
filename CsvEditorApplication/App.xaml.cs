using CsvEditorApplication.Services;
using CsvEditorApplication.ViewModels;
using CsvEditorApplication.Views; // ← この行を追加
using System.Configuration;
using System.Data;
using System.Windows;

namespace CsvEditorApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 依存性の注入（DI）
            // Service層のインスタンスを生成
            ICsvService csvService = new CsvService();
            IDialogService dialogService = new DialogService();

            // ViewModelにServiceを注入してインスタンスを生成
            var viewModel = new MainViewModel(csvService, dialogService);

            // Viewを生成し、ViewModel（DataContext）を設定
            var mainWindow = new MainWindow // この行で MainWindow が正しく認識されるようになります
            {
                DataContext = viewModel
            };

            // アプリケーション終了前に未保存確認を行う
            mainWindow.Closing += (s, args) =>
            {
                if (!viewModel.ConfirmSaveChanges())
                {
                    args.Cancel = true; // 終了をキャンセル
                }
            };

            mainWindow.Show();
        }
    }

}