using System;
using System.Collections.Generic;
using UnityEngine;

namespace MADLAD
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ModLoadExceptionCatcherOtherScenes : MonoBehaviour
    {
        PopupDialog _uiDialog;
        private readonly Rect _geometry = new Rect(0.5f,0.5f,600,10);
        private bool _displayWarning = true;

        private void OnEnable()
        {
            DontDestroyOnLoad(this);
            Application.logMessageReceivedThreaded += ParseLog;
            GameEvents.onGameSceneSwitchRequested.Add(ResetWarning);
        }

        private void ResetWarning(GameEvents.FromToAction<GameScenes, GameScenes> data)
        {
            _displayWarning = true;
        }

        private void ParseLog(string logString, string stacktrace, LogType type)
        {
            if (!logString.Contains("Could not load file or assembly")) return;
            LogWriter.Instance.WriteLog( DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]:")+" "+logString);
            if (_displayWarning) _uiDialog = GenerateDialog();
        }
        
        private PopupDialog GenerateDialog()
        {
            List<DialogGUIBase> dialog = new List<DialogGUIBase>();
            _displayWarning = false;
            dialog.Add(new DialogGUILabel("Detected Loading Exception. See "+LogWriter.Instance.pathToWrite+" for more details"));
            dialog.Add(new DialogGUIButton("Close", CloseDialog, false));
            return PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("ModLoaderDialog", "", "MADLAD - Assembly Exceptions", UISkinManager.defaultSkin, _geometry, dialog.ToArray()), true, UISkinManager.defaultSkin);
        }
        
        void CloseDialog()
        {
            if (_uiDialog != null)
            {
                _uiDialog.Dismiss();
                _uiDialog = null;
            }
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= ParseLog;
        }
    }
}