#if UNITY_EDITOR

using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace DmytroUdovychenko.AdvancedProjectBuilderTool
{
    public static class ArchiveUtility
    {
        private const string ArchiveExtension = ".zip";
        private const string ToolName         = "[ArchivePostBuild]";

        public static void CreateArchive(BuildReport buildReport)
        {
            CreateArchive(buildReport.summary.outputPath);
        }

        public static void CreateArchive(string path)
        {
            bool isFile      = File.Exists(path);
            bool isDirectory = Directory.Exists(path);

            if (!isFile && !isDirectory)
            {
                throw new Exception($"{ToolName} `path` not found: {path}");
            }

            string directoryPath  = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory;
            string targetBaseName = isFile ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
            string zipFileName    = $"{targetBaseName}{ArchiveExtension}";
            string zipPath        = Path.Combine(directoryPath, zipFileName);

            if (File.Exists(zipPath))
            {
                Debug.Log($"{ToolName}  ✔ ZIP file Exist. Removing: {zipPath}");
                File.Delete(zipPath);
            }

            if (isFile)
            {
                // *.apk for example
                CreateZipFromFile(path, zipPath);
            }
            else if (isDirectory)
            {
                // windows build for example
                CreateZipFromDirectory(path, zipPath);
            }

            Debug.Log($"{ToolName}  ✔ ZIP file Created: {zipPath}");
        }

        private static void CreateZipFromFile(string filePath, string zipPath)
        {
            using ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
            string entryName     = Path.GetFileName(filePath);
            zip.CreateEntryFromFile(filePath, entryName, CompressionLevel.Optimal);
        }

        private static void CreateZipFromDirectory(string directoryPath, string zipPath)
        {
            using ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);

            int baseLen = directoryPath.EndsWith(Path.DirectorySeparatorChar.ToString())
                        ? directoryPath.Length
                        : directoryPath.Length + 1;

            foreach (string file in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                string entryName = file.Substring(baseLen).Replace('\\', '/'); // Unix-style
                zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
            }
        }
    }
}
#endif