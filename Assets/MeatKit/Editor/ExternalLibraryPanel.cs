using UnityEngine;
using UnityEditor;
using Valve.Newtonsoft.Json;

namespace MeatKit
{
    public class ExternalLibraryPanel : EditorWindow
    {
        private class ExternalLibrary
        {
            public string   Name            { get; set; }
            public string   Description     { get; set; }
            public string[] Authors         { get; set; }
            public string   DownloadURL     { get; set; }
            public string   ProjectURL      { get; set; }
            public float    Size            { get; set; }
        }

        public static ExternalLibraryPanel Create()
        {
            var window = EditorWindow.GetWindow<ExternalLibraryPanel>();
            window.Show();
            return window;
        }

        public void OnGUI()
        {

        }
    }
}