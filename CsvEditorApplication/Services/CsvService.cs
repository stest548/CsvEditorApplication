using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace CsvEditorApplication.Services
{
    /// <summary>
    /// ICsvServiceインターフェースの実装クラス。 (F002-CLD-3.2.2)
    /// CsvHelperライブラリを利用して、具体的なファイルI/O処理を行います。
    /// </summary>
    public class CsvService : ICsvService
    {
        /// <summary>
        /// CSVファイルを読み込み、DataTableとして返します。(F002-PGD-3.2.1)
        /// </summary>
        public DataTable Read(string filePath)
        {
            // CsvHelperライブラリの設定
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // ヘッダー行を読み取る
            };

            using var reader = new StreamReader(filePath, Encoding.UTF8); // BOM付きUTF-8を自動判別
            using var csv = new CsvReader(reader, config);

            // CsvDataReaderを使用してDataTableにデータを読み込む
            using var dr = new CsvDataReader(csv);
            var dt = new DataTable();
            dt.Load(dr);

            return dt;
        }

        /// <summary>
        /// DataTableの内容をCSVファイルに書き込みます。(F003-PGD-3.2.2)
        /// </summary>
        public void Write(string filePath, DataTable dataTable)
        {
            // CsvHelperライブラリの設定 (BOM付きUTF-8で出力)
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // HasHeaderRecordはデフォルトでtrue
            };

            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));
            using var csv = new CsvWriter(writer, config);

            // ヘッダーの書き込み
            foreach (DataColumn column in dataTable.Columns)
            {
                csv.WriteField(column.ColumnName);
            }
            csv.NextRecord();

            // データ行の書き込み
            foreach (DataRow row in dataTable.Rows)
            {
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    csv.WriteField(row[i]);
                }
                csv.NextRecord();
            }
        }
    }
}
