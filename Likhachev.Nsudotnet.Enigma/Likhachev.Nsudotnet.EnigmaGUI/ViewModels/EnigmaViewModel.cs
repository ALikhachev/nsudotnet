﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Likhachev.Nsudotnet.Enigma;
using Microsoft.Win32;
using static Likhachev.Nsudotnet.Enigma.Enigma;

namespace Likhachev.Nsudotnet.EnigmaGUI.ViewModels
{
    class EnigmaViewModel : PropertyChangedBase
    {
        private string _sourceFilename;
        private string _outputFilename;
        private string _keyFilename;
        private Algorithm _algorithm = Enigma.Algorithm.AES;
        private bool _encrypt = true;
        private int _currentProgress;
        private bool _isActive = false;

        public List<Algorithm> Algorithm => Enum.GetValues(typeof(Algorithm)).OfType<Algorithm>().ToList();

        public EnigmaViewModel()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                _encrypt = args[1] == "e";
            }
            if (args.Length > 2)
            {
                SourceFilename = args[2];
            }
        }

        public Algorithm SelectedAlgorithm
        {
            get => _algorithm;
            set
            {
                _algorithm = value;
                NotifyOfPropertyChange(() => SelectedAlgorithm);
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                NotifyOfPropertyChange(() => IsActive);
                NotifyOfPropertyChange(() => CanGo);
            }
        }

        public int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                if (_currentProgress == value)
                {
                    return;
                }
                _currentProgress = value;
                NotifyOfPropertyChange(() => CurrentProgress);
            }
        }

        public string SourceFilename
        {
            get => _sourceFilename;
            set
            {
                _sourceFilename = value;
                OutputFilename = IsEncryptMode ? GetDefaultCryptedFilename(value) : null;
                KeyFilename = GetDefaultKeyFilename(value);
                CurrentProgress = 0;
                NotifyOfPropertyChange(() => SourceFilename);
                NotifyOfPropertyChange(() => CanGo);
            }
        }

        public void SelectSourceFile()
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                SourceFilename = fileDialog.FileName;
            }
        }

        public string OutputFilename
        {
            get => _outputFilename;
            set
            {
                _outputFilename = value;
                NotifyOfPropertyChange(() => OutputFilename);
                NotifyOfPropertyChange(() => CanGo);
            }
        }

        public void SelectOutputFile()
        {
            var fileDialog = new SaveFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                OutputFilename = fileDialog.FileName;
            }
        }

        public string KeyFilename
        {
            get => _keyFilename;
            set
            {
                _keyFilename = value;
                NotifyOfPropertyChange(() => KeyFilename);
                NotifyOfPropertyChange(() => CanGo);
            }
        }

        public void SelectKeyFile()
        {
            var fileDialog = _encrypt ? (FileDialog) new SaveFileDialog() : (FileDialog) new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                KeyFilename = fileDialog.FileName;
            }
        }

        public bool IsEncryptMode
        {
            get => _encrypt;
            set
            {
                _encrypt = value;
                OutputFilename = _encrypt ? GetDefaultCryptedFilename(SourceFilename) : null;
                NotifyOfPropertyChange(() => IsEncryptMode);
                NotifyOfPropertyChange(() => CanGo);
            }
        }

        public bool IsDecryptMode
        {
            get => !_encrypt;
            set
            {
                _encrypt = !value;
                OutputFilename = _encrypt ? GetDefaultCryptedFilename(SourceFilename) : null;
                NotifyOfPropertyChange(() => IsDecryptMode);
                NotifyOfPropertyChange(() => CanGo);
            }
        }

        public bool CanGo => File.Exists(SourceFilename) && !string.IsNullOrEmpty(OutputFilename)
                             && (_encrypt
                                 ? !string.IsNullOrEmpty(KeyFilename)
                                 : File.Exists(KeyFilename))
                             && !IsActive;

        public async void Go()
        {
            try
            {
                IsActive = true;
                if (_encrypt)
                {
                    await Encrypt(SourceFilename, OutputFilename, _algorithm, KeyFilename,
                        (progress) => { CurrentProgress = progress; });
                }
                else
                {
                    await Decrypt(SourceFilename, OutputFilename, _algorithm, KeyFilename,
                        (progress) => { CurrentProgress = progress; });
                }
                MessageBox.Show("File encoding done.", "Success!", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                IsActive = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during encoding: {ex.Message}", "Fail", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                IsActive = false;
            }
        }
    }
}