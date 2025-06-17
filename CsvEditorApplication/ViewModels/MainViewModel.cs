using CsvEditorApplication.Commands;
using CsvEditorApplication.Services;
using CsvHelper;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CsvEditorApplication.ViewModels
{
    /// <summary>
    /// アプリケーションのメインロジックを担当するViewModel。 (U001-CLD-3.1.1)
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ICsvService _csvService;
        private readonly IDialogService _dialogService;

        private DataTable _dataTable = new DataTable();
        public DataTable DataTable
        {
            get => _dataTable;
            set => SetProperty(ref _dataTable, value);
        }

        private string? _currentFilePath;
        public string? CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                if (SetProperty(ref _currentFilePath, value))
                {
                    OnPropertyChanged(nameof(WindowTitle));
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        public string WindowTitle
        {
            get
            {
                var fileName = string.IsNullOrEmpty(CurrentFilePath)
                    ? "無題"
                    : Path.GetFileName(CurrentFilePath);
                var dirtyMarker = IsDirty ? " *" : "";
                return $"{fileName}{dirtyMarker} - CsvEditorApplication";
            }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }

        public string StatusText => $"パス: {CurrentFilePath ?? "N/A"} | 行数: {DataTable.Rows.Count} | 列数: {DataTable.Columns.Count}";

        private IList? _selectedItems;
        public IList? SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (SetProperty(ref _selectedItems, value))
                {
                    // プロパティの変更後、コマンドの実行可否を再評価するように強制する
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand NewFileCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand SaveFileAsCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand DeleteRowCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel(ICsvService csvService, IDialogService dialogService)
        {
            _csvService = csvService;
            _dialogService = dialogService;

            NewFileCommand = new RelayCommand(_ => NewFile());
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile());
            SaveFileAsCommand = new RelayCommand(_ => SaveFileAs());
            AddRowCommand = new RelayCommand(_ => AddRow(), _ => DataTable.Columns.Count > 0);
            DeleteRowCommand = new RelayCommand(_ => DeleteRow(), _ => SelectedItems != null && SelectedItems.Count > 0);
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());

            DataTable.RowChanged += (_, _) => IsDirty = true;
            DataTable.ColumnChanged += (_, _) => IsDirty = true;
        }

        /// <summary>
        /// F001: 表データをクリアし、新規作成状態にする。(F001-PGD-3.1.1)
        /// </summary>
        private void NewFile()
        {
            if (!ConfirmSaveChanges()) return;

            DataTable = new DataTable();
            CurrentFilePath = null;
            IsDirty = false;
        }

        /// <summary>
        /// F002: CSVファイルを選択し、内容を表に読み込む。(F002-PGD-3.1.2)
        /// </summary>
        private void OpenFile()
        {
            if (!ConfirmSaveChanges()) return;

            var filePath = _dialogService.ShowOpenFileDialog();
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                DataTable = _csvService.Read(filePath);
                CurrentFilePath = filePath;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                // N003: エラーハンドリング
                HandleException(ex);
            }
        }

        /// <summary>
        /// F003: 現在の表データを上書き保存する。(F003-PGD-3.1.3)
        /// </summary>
        private void SaveFile()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveFileAs();
                return;
            }

            try
            {
                _csvService.Write(CurrentFilePath, DataTable);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// F004: 表データを新しいファイルとして保存する。(F004-PGD-3.1.4)
        /// </summary>
        private void SaveFileAs()
        {
            var filePath = _dialogService.ShowSaveFileDialog();
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                _csvService.Write(filePath, DataTable);
                CurrentFilePath = filePath;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// F006: 表の末尾に新しい空行を追加する。(F006-PGD-3.1.5)
        /// </summary>
        private void AddRow()
        {
            DataTable.Rows.Add(DataTable.NewRow());
            IsDirty = true;
        }

        /// <summary>
        /// F007: 選択されている行を削除する。(F007-PGD-3.1.6)
        /// </summary>
        private void DeleteRow()
        {
            if (SelectedItems == null || SelectedItems.Count == 0) return;

            // F007-MSG-1: 複数行削除の確認
            if (SelectedItems.Count > 1)
            {
                var message = $"選択した {SelectedItems.Count} 行を削除します。よろしいですか？";
                var result = _dialogService.ShowConfirmationDialog(message, "行の削除");
                if (result != true) return;
            }

            // ループでコレクションを変更しないように、削除対象をコピーする
            var rowsToDelete = SelectedItems.Cast<DataRowView>().ToList();
            foreach (var rowView in rowsToDelete)
            {
                rowView.Row.Delete();
            }

            DataTable.AcceptChanges(); // Delete()はマークするだけなので、これで物理的に削除
            IsDirty = true;
        }

        /// <summary>
        /// N002: 未保存の変更があるか確認し、必要に応じて保存を促す。(N002-PGD-3.1.7)
        /// </summary>
        /// <returns>処理を続行してよいか。 true: 続行, false: 中断</returns>
        public bool ConfirmSaveChanges()
        {
            if (!IsDirty) return true;

            // N002-MSG-1
            var result = _dialogService.ShowConfirmationDialog("変更が保存されていません。保存しますか？", "未保存の変更");

            switch (result)
            {
                case true: // はい
                    SaveFile();
                    return !IsDirty; // 保存が成功したか（キャンセルされなかったか）
                case false: // いいえ
                    return true; // 変更を破棄して続行
                case null: // キャンセル
                default:
                    return false; // 処理を中断
            }
        }

        /// <summary>
        /// N003: 例外をハンドリングし、適切なエラーメッセージを表示する。
        /// </summary>
        private void HandleException(Exception ex)
        {
            // メッセージ定義書に基づくエラーメッセージの選択
            string message = ex switch
            {
                // N003-MSG-2
                IOException _ => "ファイルへのアクセスに失敗しました。他のアプリケーションで使用中でないか確認してください。",
                // N003-MSG-3
                UnauthorizedAccessException _ => "ファイルへのアクセス許可がありません。ファイルのプロパティを確認してください。",
                // N003-MSG-4
                HeaderValidationException or ReaderException => "CSVファイルの形式が正しくありません。ファイルの内容を確認してください。",
                // N003-MSG-5
                _ => $"予期せぬエラーが発生しました。\nエラー内容: {ex.Message}"
            };

            _dialogService.ShowErrorDialog(message, "エラー");
        }
    }
}
