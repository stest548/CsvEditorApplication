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
    /// �A�v���P�[�V�����̃��C�����W�b�N��S������ViewModel�B (U001-CLD-3.1.1)
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
                    ? "����"
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

        public string StatusText => $"�p�X: {CurrentFilePath ?? "N/A"} | �s��: {DataTable.Rows.Count} | ��: {DataTable.Columns.Count}";

        private IList? _selectedItems;
        public IList? SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (SetProperty(ref _selectedItems, value))
                {
                    // �v���p�e�B�̕ύX��A�R�}���h�̎��s�ۂ��ĕ]������悤�ɋ�������
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
        /// F001: �\�f�[�^���N���A���A�V�K�쐬��Ԃɂ���B(F001-PGD-3.1.1)
        /// </summary>
        private void NewFile()
        {
            if (!ConfirmSaveChanges()) return;

            DataTable = new DataTable();
            CurrentFilePath = null;
            IsDirty = false;
        }

        /// <summary>
        /// F002: CSV�t�@�C����I�����A���e��\�ɓǂݍ��ށB(F002-PGD-3.1.2)
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
                // N003: �G���[�n���h�����O
                HandleException(ex);
            }
        }

        /// <summary>
        /// F003: ���݂̕\�f�[�^���㏑���ۑ�����B(F003-PGD-3.1.3)
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
        /// F004: �\�f�[�^��V�����t�@�C���Ƃ��ĕۑ�����B(F004-PGD-3.1.4)
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
        /// F006: �\�̖����ɐV������s��ǉ�����B(F006-PGD-3.1.5)
        /// </summary>
        private void AddRow()
        {
            DataTable.Rows.Add(DataTable.NewRow());
            IsDirty = true;
        }

        /// <summary>
        /// F007: �I������Ă���s���폜����B(F007-PGD-3.1.6)
        /// </summary>
        private void DeleteRow()
        {
            if (SelectedItems == null || SelectedItems.Count == 0) return;

            // F007-MSG-1: �����s�폜�̊m�F
            if (SelectedItems.Count > 1)
            {
                var message = $"�I������ {SelectedItems.Count} �s���폜���܂��B��낵���ł����H";
                var result = _dialogService.ShowConfirmationDialog(message, "�s�̍폜");
                if (result != true) return;
            }

            // ���[�v�ŃR���N�V������ύX���Ȃ��悤�ɁA�폜�Ώۂ��R�s�[����
            var rowsToDelete = SelectedItems.Cast<DataRowView>().ToList();
            foreach (var rowView in rowsToDelete)
            {
                rowView.Row.Delete();
            }

            DataTable.AcceptChanges(); // Delete()�̓}�[�N���邾���Ȃ̂ŁA����ŕ����I�ɍ폜
            IsDirty = true;
        }

        /// <summary>
        /// N002: ���ۑ��̕ύX�����邩�m�F���A�K�v�ɉ����ĕۑ��𑣂��B(N002-PGD-3.1.7)
        /// </summary>
        /// <returns>�����𑱍s���Ă悢���B true: ���s, false: ���f</returns>
        public bool ConfirmSaveChanges()
        {
            if (!IsDirty) return true;

            // N002-MSG-1
            var result = _dialogService.ShowConfirmationDialog("�ύX���ۑ�����Ă��܂���B�ۑ����܂����H", "���ۑ��̕ύX");

            switch (result)
            {
                case true: // �͂�
                    SaveFile();
                    return !IsDirty; // �ۑ��������������i�L�����Z������Ȃ��������j
                case false: // ������
                    return true; // �ύX��j�����đ��s
                case null: // �L�����Z��
                default:
                    return false; // �����𒆒f
            }
        }

        /// <summary>
        /// N003: ��O���n���h�����O���A�K�؂ȃG���[���b�Z�[�W��\������B
        /// </summary>
        private void HandleException(Exception ex)
        {
            // ���b�Z�[�W��`���Ɋ�Â��G���[���b�Z�[�W�̑I��
            string message = ex switch
            {
                // N003-MSG-2
                IOException _ => "�t�@�C���ւ̃A�N�Z�X�Ɏ��s���܂����B���̃A�v���P�[�V�����Ŏg�p���łȂ����m�F���Ă��������B",
                // N003-MSG-3
                UnauthorizedAccessException _ => "�t�@�C���ւ̃A�N�Z�X��������܂���B�t�@�C���̃v���p�e�B���m�F���Ă��������B",
                // N003-MSG-4
                HeaderValidationException or ReaderException => "CSV�t�@�C���̌`��������������܂���B�t�@�C���̓��e���m�F���Ă��������B",
                // N003-MSG-5
                _ => $"�\�����ʃG���[���������܂����B\n�G���[���e: {ex.Message}"
            };

            _dialogService.ShowErrorDialog(message, "�G���[");
        }
    }
}
