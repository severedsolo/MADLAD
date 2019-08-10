using System.Linq;
using Enumerable = UniLinq.Enumerable;

namespace ModLoaderExceptionCatcher
{
    using UnityEngine;
    using System.IO;
    using System.Collections.Generic;
    using System;
    
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ModLoaderExceptionCatcher : MonoBehaviour
    {
        List<string> errors = new List<string>();
        PopupDialog uiDialog;
        Rect geometry = new Rect(0.5f,0.5f,600,10);
        
        private void Start()
        {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        string line = "None";
            string path = KSPUtil.ApplicationRootPath + "KSP.log";
            string newPath = path + "_backup";
            File.Copy(path, newPath);
            using (StreamReader reader = new StreamReader(newPath))
            {;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Exception loading"))
                    {
                        errors.Add(line);
                        Debug.Log("[MADLAD]: Found an error at " + line);
                    }
                }
            }
            Debug.Log("[MADLAD]: Found "+errors.Count()+" exceptions");
            File.Delete(newPath);
            if (errors.Count == 0) return;
            string pathToWrite = KSPUtil.ApplicationRootPath + "/GameData/MADLAD/Logs/log.txt";
            
            using (StreamWriter writer = new StreamWriter(pathToWrite))
            {
                foreach (string s in errors)
                {
                    writer.WriteLine(s);
                }
            }
            uiDialog = GenerateDialog();
            // the code that you want to measure comes here
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log("[MADLAD]: Function executed in "+elapsedMs+" ms");
        }

        private PopupDialog GenerateDialog()
        {
             List<DialogGUIBase> dialog = new List<DialogGUIBase>();
             for (int i = 0; i < errors.Count(); i++)
             {
                 string s = errors.ElementAt(i);
                 dialog.Add(new DialogGUILabel(s, false, false));
             }
             dialog.Add(new DialogGUIButton("Close", CloseDialog, false));
             return PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("ModLoaderDialog", "", "MADLAD - Assembly Exceptions", UISkinManager.defaultSkin, geometry, dialog.ToArray()), true, UISkinManager.defaultSkin);
        }

        void CloseDialog()
        {
            if(uiDialog != null) uiDialog.Dismiss();
        }
    }
}