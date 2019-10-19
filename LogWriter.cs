using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MADLAD
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]

    public class LogWriter : MonoBehaviour
    {
        private bool _appendLog = false;
        public static LogWriter Instance;
        public string pathToWrite;
        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
            pathToWrite = KSPUtil.ApplicationRootPath + "/GameData/MADLAD/Logs/log.txt";
            Debug.Log("[MADLAD]: LogWriter.Awake");
        }


        public void WriteLog(string error)
        {
            using (StreamWriter writer = new StreamWriter(pathToWrite, _appendLog))
            {
                _appendLog = true;
                writer.WriteLine(error); 
            }
        }
    }
}