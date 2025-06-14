using System.Data;

namespace CsvEditorApplication.Services
{
    /// <summary>
    /// CSV�t�@�C���̓ǂݏ��������̃C���^�[�t�F�[�X�B (F002-CLD-3.2.1)
    /// </summary>
    public interface ICsvService
    {
        /// <summary>
        /// �w�肳�ꂽ�p�X����CSV�t�@�C����ǂݍ��݁ADataTable�Ƃ��ĕԂ��܂��B
        /// </summary>
        /// <param name="filePath">�ǂݍ���CSV�t�@�C���̃p�X�B</param>
        /// <returns>�ǂݍ��񂾃f�[�^���܂�DataTable�B</returns>
        DataTable Read(string filePath);

        /// <summary>
        /// �w�肳�ꂽDataTable�̓��e���A�w�肳�ꂽ�p�X��CSV�`���ŏ������݂܂��B
        /// </summary>
        /// <param name="filePath">��������CSV�t�@�C���̃p�X�B</param>
        /// <param name="dataTable">�������ރf�[�^���܂�DataTable�B</param>
        void Write(string filePath, DataTable dataTable);
    }
}
