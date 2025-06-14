using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace CsvEditorApplication.Services
{
    /// <summary>
    /// ICsvService�C���^�[�t�F�[�X�̎����N���X�B (F002-CLD-3.2.2)
    /// CsvHelper���C�u�����𗘗p���āA��̓I�ȃt�@�C��I/O�������s���܂��B
    /// </summary>
    public class CsvService : ICsvService
    {
        /// <summary>
        /// CSV�t�@�C����ǂݍ��݁ADataTable�Ƃ��ĕԂ��܂��B(F002-PGD-3.2.1)
        /// </summary>
        public DataTable Read(string filePath)
        {
            // CsvHelper���C�u�����̐ݒ�
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // �w�b�_�[�s��ǂݎ��
            };

            using var reader = new StreamReader(filePath, Encoding.UTF8); // BOM�t��UTF-8����������
            using var csv = new CsvReader(reader, config);

            // CsvDataReader���g�p����DataTable�Ƀf�[�^��ǂݍ���
            using var dr = new CsvDataReader(csv);
            var dt = new DataTable();
            dt.Load(dr);

            return dt;
        }

        /// <summary>
        /// DataTable�̓��e��CSV�t�@�C���ɏ������݂܂��B(F003-PGD-3.2.2)
        /// </summary>
        public void Write(string filePath, DataTable dataTable)
        {
            // CsvHelper���C�u�����̐ݒ� (BOM�t��UTF-8�ŏo��)
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // HasHeaderRecord�̓f�t�H���g��true
            };

            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(true));
            using var csv = new CsvWriter(writer, config);

            // �w�b�_�[�̏�������
            foreach (DataColumn column in dataTable.Columns)
            {
                csv.WriteField(column.ColumnName);
            }
            csv.NextRecord();

            // �f�[�^�s�̏�������
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
