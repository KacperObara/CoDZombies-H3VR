using UnityEngine;

namespace CODZombies.Scripts.Objects.Window
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