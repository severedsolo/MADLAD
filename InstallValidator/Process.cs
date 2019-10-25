
#region Using Directives

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace InstallValidator
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Process: MonoBehaviour
    {
        internal static List<string> parseErrorMsgs = new List<string>();
        
        public static bool ParseError { get; set; }
        public static string AddParseErrorMsg { set { parseErrorMsgs.Add(value); } }

        string ModName = "";    // This comes from the main NAME in the .version file

        const string DefaultMsg = "<MODNAME> has been installed incorrectly and will not function properly. All files should be located in: <KSP install directory>/GameData/<PATH>/<DIRECTORY>. Do not move any files from inside that folder.";

        /// <summary>
        /// Process a single .version file
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        bool DoProcess(string fname)
        {
            string json = File.ReadAllText(fname);
            var data = Json.Deserialize(json) as Dictionary<string, object>;
            if (data == null)
            {
                ParseError = true;
                AddParseErrorMsg = "[InstallValidator] Bad .version file!, " + fname+ " is invalid";
                Log.Error("[InstallValidator] Error in Json.Deserialize, file: " + fname);
                return false;
            }
            
            foreach (var key in data.Keys)
            {
                if (key == "NAME")
                {
                    ModName = (string)data[key];
                }
                if (key == "INSTALL_LOC" || key.StartsWith("INSTALL_LOC_"))
                {
                    var installLocInfo = new InstallLocInfo(data[key]);
                    ValidateInstallLoc(key, installLocInfo);
                }
            }
            return true;
        }

        /// <summary>
        /// Validate that the specified path, directory & file exist
        /// </summary>
        /// <param name="stanza"></param>
        /// <param name="ili"></param>
        void ValidateInstallLoc(string stanza, InstallLocInfo ili)
        {
            string fullPath = "GameData" + ili.Path;
             if (ili.Path != null && !Directory.Exists(fullPath))
            {
                ParseError = true;
                AddParseErrorMsg = ProcessMessage("Path", ili, stanza, DefaultMsg);

                Log.Error("Missing path: " + ili.Path);
                return;
            }
            fullPath += "/" + ili.Directory;
            fullPath.Replace("//", "/");

            if (ili.Directory != null && !Directory.Exists(fullPath))
            {
                ParseError = true;
                AddParseErrorMsg = ProcessMessage("Directory", ili, stanza, DefaultMsg);

                Log.Error("Missing directory: " + ili.Directory);
                return;
            }
            if (String.IsNullOrEmpty(ili.FileName)) ili.FileName = ili.Name + ".version";
            fullPath += "/" + ili.FileName;           
            fullPath.Replace("//", "/");
            if (!File.Exists(fullPath))
            {
                ParseError = true;
                AddParseErrorMsg = ProcessMessage("File", ili, stanza, DefaultMsg);
                Log.Error("Missing file: " + ili.FileName);
                return;
            }
        }

        /// <summary>
        /// This processes the message provided in the .version file, doing substitutions as needed
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="ili"></param>
        /// <param name="stanza"></param>
        /// <param name="defaultMsg"></param>
        /// <returns></returns>
        string ProcessMessage(string fieldName, InstallLocInfo ili, string stanza, string defaultMsg)
        {
            string msg;
            if (string.IsNullOrEmpty(ili.Message)) msg = defaultMsg;
            else msg = ili.Message;
            if (ili.Name == null) ili.Name = ModName;
            msg = msg.Replace("<MODNAME>", ili.Name);
            msg = msg.Replace("<FILE>", ili.FileName);
            msg = msg.Replace("<DIRECTORY>", ili.Directory);
            msg = msg.Replace("<PATH>", ili.Path);
            msg = msg.Replace("<STANZA>", stanza);
            msg = msg.Replace("<FIELD>", fieldName);
            msg.Replace("//", "/");
            return msg;
        }

        /// <summary>
        /// Scan all version files, process each one
        /// </summary>
        void ScanVersionFiles()
        {
            var files = System.IO.Directory.GetFiles(KSPUtil.ApplicationRootPath + "GameData/", "*.version", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                f.Replace('\\', '/');
                Log.Info("file: " + f);
                DoProcess(f);
            }
        }

        public void Start()
        {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        Log.Info("Start, searching: " + KSPUtil.ApplicationRootPath + "GameData/");
            ScanVersionFiles();
            if (ParseError)
            {
                gameObject.AddComponent<IssueGui>();
            }
            watch.Stop();
            Debug.Log("[MADLAD]: Parsed .version files in "+watch.ElapsedMilliseconds+"ms");
        }
    }
}
