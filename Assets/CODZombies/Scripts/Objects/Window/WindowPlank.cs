using UnityEngine;

namespace CustomScripts.Objects.Windows
{
    public class WindowPlank : MonoBehaviour
    {
        public Plank Plank;

        private void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}