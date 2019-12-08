// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

// This file is based on the IssueGui.cs file from the KSP-AVC mod

#region Using Directives

using System;
using UnityEngine;

#endregion

namespace InstallValidator
{
    /// <summary>
    /// This class displays a small dialog showing all issues found
    /// </summary>
    public class IssueGui : MonoBehaviour
    {
        #region Fields        

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle messageStyle;
        private GUIStyle nameTitleStyle;
        private Rect position = new Rect(Screen.width, Screen.height,0, 0);


        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            Log.Info("IssueGui was created.");
        }

        /// <summary>
        /// Standard OnGUI method to control the window
        /// </summary>
        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Installation Validation Monitor", HighLogic.Skin.window);
                this.CentreWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        protected void Start()
        {
            try
            {
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        #endregion

        #region Methods: private

        private void CentreWindow()
        {
            if (this.hasCentred || !(this.position.width > 0) || !(this.position.height > 0))
            {
                return;
            }
            this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            this.hasCentred = true;
        }

        /// <summary>
        /// Draw the window shoing the issues
        /// </summary>
        private void DrawInstallationIssues()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.Label("INSTALLATION ISSUES", this.nameTitleStyle);
            foreach (var error in Process.parseErrorMsgs)
            {
                GUILayout.Label(error, messageStyle, GUILayout.MinWidth(575.0f));
            }
            GUILayout.EndVertical();
        }

        private void InitialiseStyles()
        {
            this.boxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            this.nameTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        /// <summary>
        /// Display the window, destroys itself when done
        /// </summary>
        /// <param name="id"></param>
        private void Window(int id)
        {
            try
            {
                if (Process.parseErrorMsgs != null && Process.parseErrorMsgs.Count > 0)
                {
                    this.DrawInstallationIssues();
                }
                if (GUILayout.Button("CLOSE", this.buttonStyle))
                {
                    Destroy(this);
                }
                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

#endregion
    }
}
