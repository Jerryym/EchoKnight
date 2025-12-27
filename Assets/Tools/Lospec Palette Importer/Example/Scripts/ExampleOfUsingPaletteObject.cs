using UnityEngine;

namespace YesDev.Lospec
{
    public class ExampleOfUsingPaletteObject : MonoBehaviour
    {
        public LospecPaletteObject palette;
        private void OnValidate()
        {
            if (palette == null) return;
            Renderer[] children = GetComponentsInChildren<Renderer>();
            for (int x = 0; x < palette.colors.Count && x < children.Length; x++)
            {
                children[x].material.color = palette.colors[x];
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
