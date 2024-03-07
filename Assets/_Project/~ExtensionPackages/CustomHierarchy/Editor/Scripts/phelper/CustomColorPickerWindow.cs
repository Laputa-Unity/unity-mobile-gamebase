using UnityEngine;
using UnityEditor;
using customtools.customhierarchy.pdata;

namespace customtools.customhierarchy.phelper
{
    public delegate void CustomColorSelectedHandler(GameObject[] gameObjects, Color color);
    public delegate void CustomColorRemovedHandler(GameObject[] gameObjects);

    public class CustomColorPickerWindow: PopupWindowContent 
    {
        // PRIVATE
        private GameObject[] gameObjects;
        private CustomColorSelectedHandler colorSelectedHandler;
        private CustomColorRemovedHandler colorRemovedHandler;
        private Texture2D colorPaletteTexture;
        private Rect paletteRect;

        // CONSTRUCTOR
        public CustomColorPickerWindow(GameObject[] gameObjects, CustomColorSelectedHandler colorSelectedHandler, CustomColorRemovedHandler colorRemovedHandler)
        {
            this.gameObjects = gameObjects;
            this.colorSelectedHandler = colorSelectedHandler;
            this.colorRemovedHandler = colorRemovedHandler;

            colorPaletteTexture = CustomResources.getInstance().GetTexture(CustomTexture.CustomColorPalette);
            paletteRect = new Rect(0, 0, colorPaletteTexture.width, colorPaletteTexture.height);
        }

        // DESTRUCTOR
        public override void OnClose()
        {
            gameObjects = null;
            colorSelectedHandler = null;
            colorRemovedHandler = null; 
        }

        // GUI
        public override Vector2 GetWindowSize()
        {
            return new Vector2(paletteRect.width, paletteRect.height);
        }

        public override void OnGUI(Rect rect)
        {
            GUI.DrawTexture(paletteRect, colorPaletteTexture);

            Vector2 mousePosition = Event.current.mousePosition;
            if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseUp && paletteRect.Contains(mousePosition))
            {
                Event.current.Use();
                if (mousePosition.x < 15 && mousePosition.y < 15)
                {
                    colorRemovedHandler(gameObjects);
                }
                else
                {
                    colorSelectedHandler(gameObjects, colorPaletteTexture.GetPixel((int)mousePosition.x, colorPaletteTexture.height - (int)mousePosition.y));
                }
                this.editorWindow.Close();
            }
        }
    }
}

