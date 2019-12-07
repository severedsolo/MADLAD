using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MADLAD
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ModLoaderExceptionCatcherMainMenu : MonoBehaviour
    {
        readonly List<string> _errors = new List<string>();
        PopupDialog _uiDialog;
        private readonly Rect _geometry = new Rect(0.5f,0.5f,600,10);

        private void Start()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CheckLogForErrors();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log("[MADLAD]: Parsed Log in " + elapsedMs + " ms");
        }

        private void CheckLogForErrors()
        {
            string path = KSPUtil.ApplicationRootPath + "KSP.log";
            string newPath = path + "_backup";
            if(File.Exists(newPath)) File.Delete(newPath);
            File.Copy(path, newPath);
            ConfigNode[] whitelist = GameDatabase.Instance.GetConfigNode("MADLAD/MADLAD_WHITELIST").GetNodes("MOD");
            using (StreamReader reader = new StreamReader(newPath))
            {
                string line = "None";
                while ((line = reader.ReadLine()) != null)
                {
                    if (ErrorToBeLogged(line) && !Whitelisted(line, whitelist))
                    {
                        _errors.Add(line);
                    }
                }
            }

            Debug.Log("[MADLAD]: Found " + _errors.Count + " exceptions");
            File.Delete(newPath);
            if (_errors.Count == 0) return;
            _uiDialog = GenerateDialog();
            foreach (string s in _errors)
            {
                LogWriter.Instance.WriteLog(s);
            }
        }

        private bool Whitelisted(string line, ConfigNode[] whitelist)
        {
            Debug.Log("[MADLAD]: Found an error at "+line+" - checking whitelist");
            if (whitelist == null || whitelist.Length == 0)
            {
                Debug.Log("[MADLAD]: Whitelist is empty");
                return false;
            }

            for (int i = 0; i < whitelist.Length; i++)
            {
                ConfigNode cn = whitelist[i];
                string modName = cn.GetValue("ModName");
                string path = cn.GetValue("DependencyPath");
                if (!line.Contains(modName)) continue;
                Debug.Log("[MADLAD]: Found potential whitelist entry for "+modName);
                if(File.Exists(Path.Combine(KSPUtil.ApplicationRootPath, "GameData/", path))) continue;
                Debug.Log("[MADLAD]: Matched "+modName+" to the whitelist. Suppressing error");
                return true;
            }
            Debug.Log("[MADLAD]: Could not verify that error is expected. Logging");
            return false;
        }

        private bool ErrorToBeLogged(string line)
        {
            if (line.Contains("Exception loading")) return true;
            if (line.Contains("AssemblyLoader: Assembly"))
            {
                if (line.Contains("is missing")) return false;
                return true;
            }
            return false;
        }

        private PopupDialog GenerateDialog()
        {
             List<DialogGUIBase> dialog = new List<DialogGUIBase>();
             for (int i = 0; i < _errors.Count(); i++)
             {
                 string s = _errors.ElementAt(i);
                 dialog.Add(new DialogGUILabel(s, false, false));
             }
             dialog.Add(new DialogGUIButton("Close", CloseDialog, false));
             return PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("ModLoaderDialog", "", "MADLAD - Assembly Exceptions", UISkinManager.defaultSkin, _geometry, dialog.ToArray()), true, UISkinManager.defaultSkin);
        }

        void CloseDialog()
        {
            if(_uiDialog != null) _uiDialog.Dismiss();
        }
    }
}