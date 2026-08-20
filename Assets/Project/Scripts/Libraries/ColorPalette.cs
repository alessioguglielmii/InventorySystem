using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Libraries/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [System.Serializable]
    public class TextColors
    {
        public Color Normal = Color.white;
        public Color Selected = Color.green;
        public Color Hovered = Color.yellow;
        public Color Disabled = Color.gray;
    }

    public TextColors Text;
}
