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
            Debug.Log("[MADLAD]: Function executed in " + elapsedMs + " ms");
        }

        private void CheckLogForErrors()
        {
            string path = KSPUtil.ApplicationRootPath + "KSP.log";
            string newPath = path + "_backup";
            File.Copy(path, newPath);
            using (StreamReader reader = new StreamReader(newPath))
            {
                string line = "None";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Exception loading"))
                    {
                        _errors.Add(line);
                        Debug.Log("[MADLAD]: Found an error at " + line);
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