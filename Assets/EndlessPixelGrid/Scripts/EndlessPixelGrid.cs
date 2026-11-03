using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Schoolmakerdays
{
    public sealed class EndlessPixelGrid : MonoBehaviour
    {
        private static readonly KeyCode[] Keys = {
            KeyCode.W, KeyCode.A, KeyCode.UpArrow, KeyCode.LeftArrow,
            KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.S
        };
        private static readonly string[] KeyLabels = { "W", "A", "↑", "←", "↓", "→", "S" };
        private static readonly Color Blue = Hex("1B9ED4");
        private static readonly Color Red = Hex("E30613");
        private static readonly Color Ink = Hex("163B54");
        private static readonly Color Muted = Hex("527084");
        private static readonly Color Line = Hex("D5E5EE");
        private readonly PixelGridModel model = new PixelGridModel();
        [SerializeField] private Image[] gridPixels;
        [SerializeField] private Image[] inputPixels;
        [SerializeField] private Button commitButton;
        [SerializeField] private Button resetButton;
        private bool hasFocus = true;

        private void Awake()
        {
            if (gridPixels == null || gridPixels.Length != 70) CreateLayout();
            commitButton.onClick.AddListener(Commit);
            resetButton.onClick.AddListener(ResetGrid);
            RenderGrid();
            RenderInputLine();
        }

        private void Update()
        {
            // Sample all seven contacts before committing this frame's snapshot.
            bool changed = false;
            for (int column = 0; column < PixelGridModel.Columns; column++)
            {
                bool signal = hasFocus && Input.GetKey(Keys[column]);
                changed |= model.InputLine[column] != signal;
                model.InputLine[column] = signal;
            }
            if (changed) RenderInputLine();
            // Reset takes priority if G and D arrive in the same frame.
            if (hasFocus && Input.GetKeyDown(KeyCode.G)) ResetGrid();
            else if (hasFocus && Input.GetKeyDown(KeyCode.D)) Commit();
        }

        public void Commit()
        {
            model.Commit();
            RenderGrid();
        }

        public void ResetGrid()
        {
            model.ResetGrid();
            RenderGrid();
        }

        private void OnApplicationFocus(bool focused) => SetFocus(focused);
        public void BrowserFocus(string focused) => SetFocus(focused == "1");
        private void SetFocus(bool focused)
        {
            hasFocus = focused;
            if (!focused)
            {
                model.ClearInput();
                if (inputPixels != null && inputPixels.Length == 7) RenderInputLine();
            }
        }

        private void RenderGrid()
        {
            for (int row = 0; row < PixelGridModel.Rows; row++)
                for (int column = 0; column < PixelGridModel.Columns; column++)
                    gridPixels[row * 7 + column].color = model.Grid[row, column] ? Blue : Color.white;
        }

        private void RenderInputLine()
        {
            for (int column = 0; column < PixelGridModel.Columns; column++)
                inputPixels[column].color = model.InputLine[column] ? Blue : Color.white;
        }

        // Also called by the editor setup so every UI Image is visible/editable in the scene.
        public void CreateLayout()
        {
            var canvasObject = new GameObject("Workshop Canvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            Transform root = canvasObject.transform;
            Panel(root, "Background", 0, 0, 1920, 1080, Hex("F3F8FB"));
            Panel(root, "Red top line", 0, 0, 1920, 8, Red);
            Label(root, "Workshop", "SCHOOLMAKERDAYS  /  MITMACH-WORKSHOP", 80, 42, 1060, 28, 21, Blue, FontStyle.Bold);
            Label(root, "Title", "Endless Pixelgrid", 76, 76, 1200, 78, 62, Ink, FontStyle.Bold);
            Label(root, "Subtitle", "Aus Berührung wird ein Muster.", 80, 158, 540, 42, 27, Muted);

            Label(root, "Instruction heading", "DEIN KONTAKT.\nDEIN PIXEL.", 80, 264, 530, 124, 47, Ink, FontStyle.Bold);
            Label(root, "Step one", "01  BERÜHREN", 80, 434, 500, 36, 25, Blue, FontStyle.Bold);
            Label(root, "Contact instructions", "Halte EARTH und berühre die Kontakte.\nJeder Kontakt steuert ein Pixel.\nDu kannst mehrere gleichzeitig berühren.", 80, 482, 515, 112, 25, Ink);
            Label(root, "Step two", "02  ZEILE ÜBERNEHMEN", 80, 642, 520, 36, 25, Blue, FontStyle.Bold);
            Label(root, "Commit instructions", "Drücke D: Dein Muster wandert ins Raster.\nDie bisherigen Zeilen rücken nach oben.\nDie älteste Zeile verschwindet.", 80, 690, 530, 112, 25, Ink);
            Panel(root, "Legend active", 80, 866, 24, 24, Blue);
            Label(root, "Legend active text", "Blau = Kontakt", 118, 860, 230, 36, 23, Ink);
            Panel(root, "Legend inactive border", 80, 908, 24, 24, Line);
            Panel(root, "Legend inactive", 82, 910, 20, 20, Color.white);
            Label(root, "Legend inactive text", "Weiß = kein Kontakt", 118, 902, 300, 36, 23, Ink);

            Panel(root, "Grid card", 660, 178, 544, 818, Line);
            Panel(root, "Grid card inner", 662, 180, 540, 814, Color.white);
            Label(root, "Grid heading", "DEIN ENDLOSES MUSTER", 694, 198, 474, 36, 23, Ink, FontStyle.Bold, TextAnchor.MiddleCenter);
            gridPixels = new Image[70];
            for (int row = 0; row < 10; row++)
                for (int column = 0; column < 7; column++)
                    gridPixels[row * 7 + column] = Pixel(root, "Grid " + row + " Q" + (6 - column),
                        714 + column * 62, 254 + row * 55, 50);
            Panel(root, "Live divider", 692, 817, 480, 2, Line);
            Label(root, "Live heading", "LIVE · DEINE KONTAKTE", 694, 827, 474, 30, 21, Blue, FontStyle.Bold, TextAnchor.MiddleCenter);
            inputPixels = new Image[7];
            for (int column = 0; column < 7; column++)
            {
                float x = 714 + column * 62;
                inputPixels[column] = Pixel(root, "Live Q" + (6 - column), x, 868, 50);
                Label(root, "Bit Q" + (6 - column), "Q" + (6 - column), x - 2, 923, 54, 24, 17, Muted, FontStyle.Normal, TextAnchor.MiddleCenter);
                Label(root, "Key " + column, KeyLabels[column], x - 2, 946, 54, 30, 23, Ink, FontStyle.Bold, TextAnchor.MiddleCenter);
            }

            Logo(root, "MakerFaire", 1366, 182, 350, 374);
            Logo(root, "HTL", 1356, 594, 104, 90);
            Label(root, "HTL label", "MAKING\nTRIFFT SCHULE.", 1490, 596, 340, 94, 29, Ink, FontStyle.Bold);
            Logo(root, "Smeerws", 1390, 754, 300, 190);
            Logo(root, "Makey", 1754, 54, 82, 78);
            Panel(root, "Footer", 0, 1020, 1920, 60, Ink);
            Label(root, "Footer hint", "7 Kontakte. Unendlich viele Muster.", 80, 1030, 800, 38, 23, Color.white);
            commitButton = ActionButton(root, "Commit", "D  ZEILE ÜBERNEHMEN", 1270, 1027, 330, 44, Blue);
            resetButton = ActionButton(root, "Reset", "G  RASTER LEEREN", 1618, 1027, 242, 44, Red);

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                events.GetComponent<EventSystem>().sendNavigationEvents = false;
            }
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            return color;
        }

        private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static Image Panel(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var image = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Image Pixel(Transform parent, string name, float x, float y, float size)
        {
            Panel(parent, name + " border", x - 2, y - 2, size + 4, size + 4, Line);
            return Panel(parent, name, x, y, size, size, Color.white);
        }

        private static void Label(Transform parent, string name, string value, float x, float y,
            float w, float h, int size, Color color, FontStyle style = FontStyle.Normal,
            TextAnchor alignment = TextAnchor.UpperLeft)
        {
            var label = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = value;
            label.fontSize = size;
            label.fontStyle = style;
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;
        }

        private static void Logo(Transform parent, string name, float x, float y, float w, float h)
        {
            Panel(parent, name + " backing", x, y, w, h, Color.white);
            var image = Rect(parent, name + " logo", x, y, w, h).gameObject.AddComponent<RawImage>();
            var texture = Resources.Load<Texture2D>("Branding/" + name);
            image.texture = texture;
            image.raycastTarget = false;
            if (texture == null) { Debug.LogError("Missing logo: " + name); return; }
            float scale = Mathf.Min(w / texture.width, h / texture.height);
            image.rectTransform.sizeDelta = new Vector2(texture.width * scale, texture.height * scale);
            image.rectTransform.anchoredPosition += new Vector2((w - texture.width * scale) / 2, -(h - texture.height * scale) / 2);
        }

        private static Button ActionButton(Transform parent, string name, string caption,
            float x, float y, float w, float h, Color color)
        {
            var background = Panel(parent, name, x, y, w, h, color);
            background.raycastTarget = true;
            var button = background.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            Label(background.transform, "Caption", caption, 0, 0, w, h, 20, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
            return button;
        }
    }
}
