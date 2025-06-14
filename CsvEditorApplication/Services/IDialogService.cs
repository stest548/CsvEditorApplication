namespace CsvEditorApplication.Services
{
    /// <summary>
    /// �_�C�A���O�\���̃C���^�[�t�F�[�X�B (U002-CLD-3.2.3)
    /// ViewModel����View�̃_�C�A���O�𒊏ۓI�ɌĂяo�����߂̌_����`���܂��B
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// �t�@�C�����J���_�C�A���O��\�����A�I�����ꂽ�t�@�C���p�X��Ԃ��܂��B
        /// </summary>
        /// <returns>�I�����ꂽ�t�@�C���̃p�X�B�L�����Z�����ꂽ�ꍇ��null�B</returns>
        string? ShowOpenFileDialog();

        /// <summary>
        /// ���O��t���ĕۑ��_�C�A���O��\�����A�w�肳�ꂽ�ۑ��p�X��Ԃ��܂��B
        /// </summary>
        /// <returns>�w�肳�ꂽ�ۑ��p�X�B�L�����Z�����ꂽ�ꍇ��null�B</returns>
        string? ShowSaveFileDialog();

        /// <summary>
        /// �m�F�_�C�A���O�i�͂�/������/�L�����Z���j��\�����܂��B
        /// </summary>
        /// <param name="message">�_�C�A���O�ɕ\�����郁�b�Z�[�W�B</param>
        /// <param name="title">�_�C�A���O�̃^�C�g���B</param>
        /// <returns>�͂�:true, ������:false, �L�����Z��:null�B</returns>
        bool? ShowConfirmationDialog(string message, string title);

        /// <summary>
        /// �G���[�ʒm�_�C�A���O��\�����܂��B
        /// </summary>
        /// <param name="message">�_�C�A���O�ɕ\������G���[���b�Z�[�W�B</param>
        /// <param name="title">�_�C�A���O�̃^�C�g���B</param>
        void ShowErrorDialog(string message, string title);
    }
}
