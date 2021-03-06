﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    public static class ImageEffectPackager
    {
        private const string ConfigFileName = "Config.json";

        public static string Package(string outputFilePath, string configJson, string assetsFolderPath)
        {
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                string outputFolder = Path.GetDirectoryName(outputFilePath);
                Helpers.CreateDirectory(outputFolder);

                string configFilePath = Path.Combine(outputFolder, ConfigFileName);
                File.WriteAllText(configFilePath, configJson, Encoding.UTF8);

                Dictionary<string, string> files = new Dictionary<string, string>();
                files.Add(configFilePath, ConfigFileName);

                if (!string.IsNullOrEmpty(assetsFolderPath) && Directory.Exists(assetsFolderPath))
                {
                    string parentFolderPath = Directory.GetParent(assetsFolderPath).FullName;
                    int entryNamePosition = parentFolderPath.Length + 1;

                    foreach (string assetPath in Directory.EnumerateFiles(assetsFolderPath, "*.*", SearchOption.AllDirectories).Where(x => Helpers.IsImageFile(x)))
                    {
                        string entryName = assetPath.Substring(entryNamePosition);
                        files.Add(assetPath, entryName);
                    }
                }

                try
                {
                    ZipManager.Compress(outputFilePath, files);
                }
                finally
                {
                    File.Delete(configFilePath);
                }

                return outputFilePath;
            }

            return null;
        }

        public static string ExtractPackage(string packageFilePath, string destination)
        {
            if (!string.IsNullOrEmpty(packageFilePath) && File.Exists(packageFilePath) && !string.IsNullOrEmpty(destination))
            {
                string configFilePath = Path.Combine(destination, ConfigFileName);

                if (File.Exists(configFilePath))
                {
                    File.Delete(configFilePath);
                }

                ZipManager.Extract(packageFilePath, destination);

                return configFilePath;
            }

            return null;
        }
    }
}