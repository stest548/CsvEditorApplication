using System.Data;

namespace CsvEditorApplication.Services
{
    /// <summary>
    /// CSVファイルの読み書き処理のインターフェース。 (F002-CLD-3.2.1)
    /// </summary>
    public interface ICsvService
    {
        /// <summary>
        /// 指定されたパスからCSVファイルを読み込み、DataTableとして返します。
        /// </summary>
        /// <param name="filePath">読み込むCSVファイルのパス。</param>
        /// <returns>読み込んだデータを含むDataTable。</returns>
        DataTable Read(string filePath);

        /// <summary>
        /// 指定されたDataTableの内容を、指定されたパスにCSV形式で書き込みます。
        /// </summary>
        /// <param name="filePath">書き込むCSVファイルのパス。</param>
        /// <param name="dataTable">書き込むデータを含むDataTable。</param>
        void Write(string filePath, DataTable dataTable);
    }
}
