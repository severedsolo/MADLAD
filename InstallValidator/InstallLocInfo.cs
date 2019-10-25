#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace InstallValidator
{
    public class InstallLocInfo
    {
        public string Name = null;
        public string FileName = null;
        public string Directory = null;
        public string Path = null;
        public string Message = null;

        /// <summary>
        /// Instantiator
        /// </summary>
        /// <param name="obj"></param>
        public InstallLocInfo(object obj)
        {
            this.ParseJson(obj);
        }

        /// <summary>
        /// Parse the JSON stanza 
        /// </summary>
        /// <param name="obj"></param>
        private void ParseJson(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null)
            {
                Process.ParseError = true;
                Process.AddParseErrorMsg = "[InstallValidator] No data in dictionary (ParseJson)";
                Log.Error("No data in dictionary (ParseJson)");
                throw new ArgumentException("[InstallValidator] No data in dictionary (ParseJson)");
            }

            foreach (var key in data.Keys)
            {
                switch (key)
                {
                    case "NAME":
                        this.Name = (string)data[key];
                        Log.Info("NAME: " + Name);
                        break;

                    case "FILE":
                        this.FileName = (string)data[key];
                        Log.Info("FILE: " + FileName);
                        break;

                    case "DIRECTORY":
                        this.Directory = (string)data[key];
                        Log.Info("DIRECTORY: " + Directory);
                        break;
                    case "PATH":
                        this.Path = (string)data[key];
                        Log.Info("PATH: " + Path);
                        break;

                    case "MESSAGE":
                        this.Message = (string)data[key];
                        Log.Info("MESSAGE: " + Message);
                        break;
                }
            }
        }
    }
}
