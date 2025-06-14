namespace CsvEditorApplication.Services
{
    /// <summary>
    /// ダイアログ表示のインターフェース。 (U002-CLD-3.2.3)
    /// ViewModelからViewのダイアログを抽象的に呼び出すための契約を定義します。
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// ファイルを開くダイアログを表示し、選択されたファイルパスを返します。
        /// </summary>
        /// <returns>選択されたファイルのパス。キャンセルされた場合はnull。</returns>
        string? ShowOpenFileDialog();

        /// <summary>
        /// 名前を付けて保存ダイアログを表示し、指定された保存パスを返します。
        /// </summary>
        /// <returns>指定された保存パス。キャンセルされた場合はnull。</returns>
        string? ShowSaveFileDialog();

        /// <summary>
        /// 確認ダイアログ（はい/いいえ/キャンセル）を表示します。
        /// </summary>
        /// <param name="message">ダイアログに表示するメッセージ。</param>
        /// <param name="title">ダイアログのタイトル。</param>
        /// <returns>はい:true, いいえ:false, キャンセル:null。</returns>
        bool? ShowConfirmationDialog(string message, string title);

        /// <summary>
        /// エラー通知ダイアログを表示します。
        /// </summary>
        /// <param name="message">ダイアログに表示するエラーメッセージ。</param>
        /// <param name="title">ダイアログのタイトル。</param>
        void ShowErrorDialog(string message, string title);
    }
}
