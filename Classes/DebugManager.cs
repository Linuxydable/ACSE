﻿using System;
using System.IO;

namespace ACSE
{
    public enum DebugLevel
    {
        None,
        Error,
        Info,
        Debug
    }

    public class DebugManager
    {
        private const string LogFileName = "ACSE_Log";
        private FileStream _logFile;
        private StreamWriter _logWriter;
        private const int MaxLogSize = 5000000; // 5MB Max Size
        public bool Enabled;

        public DebugManager()
        {
            CheckAndDeleteLogFile();

            if (Properties.Settings.Default.DebugLevel > 0)
            {
                InitiateDebugLogWriter();
                Enabled = true;
                WriteLine("========== Debug Log Initiated ==========");
            }
            else
            {
                Enabled = false;
            }
        }

        public void CloseDebugLogWriter()
        {
            if (_logWriter != null)
            {
                _logWriter.Close();
                _logWriter.Dispose();
            }

            if (_logFile != null)
            {
                _logFile.Close();
                _logFile.Dispose();
            }

            Enabled = false;
        }

        public void InitiateDebugLogWriter()
        {
            CloseDebugLogWriter();

            try
            {
                _logFile = new FileStream(GetLogFilePath(), FileMode.OpenOrCreate);
                _logWriter = new StreamWriter(_logFile);
                _logWriter.BaseStream.Seek(0, SeekOrigin.End);
            }
            catch
            {
                Enabled = false;
                Console.WriteLine("Unable to open or create the debug log file!");
            }
        }

        private bool CheckLogSizeOk()
        {
            var info = new FileInfo(GetLogFilePath());
            return info.Length <= MaxLogSize;
        }

        public void DeleteLogFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                Console.WriteLine("Log file exceeded maximum file length and was deleted.");
            }
            catch { Console.WriteLine("Unable to delete log file!"); }
        }

        private void CheckAndDeleteLogFile()
        {
            var filePath = GetLogFilePath();
            if (File.Exists(filePath) && !CheckLogSizeOk())
            {
                DeleteLogFile(filePath);
            }
        }

        public string GetLogFilePath()
        {
            return MainForm.AssemblyLocation + $"\\{LogFileName}.txt";
        }

        public void WriteLine(string contents, DebugLevel level = DebugLevel.Info)
        {
            if (_logWriter != null && level <= Properties.Settings.Default.DebugLevel)
            {
                if (!CheckLogSizeOk())
                {
                    CloseDebugLogWriter();
                    DeleteLogFile(GetLogFilePath());
                    InitiateDebugLogWriter();
                }
                _logWriter.WriteLine(
                    $"[{level}] - ({(MainForm.SaveFile != null ? MainForm.SaveFile.SaveType.ToString().Replace("_", " ") : "No Save")}) - {DateTime.Now} => {contents}");
                _logWriter.Flush();
            }
        }
    }
}
