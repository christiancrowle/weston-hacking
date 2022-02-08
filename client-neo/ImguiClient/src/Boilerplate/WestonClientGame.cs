using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Num = System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using static WestonClient.Config.Settings;

namespace WestonClient {
    public class WestonClientGame : Game {
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;

        private Texture2D _xnaTexture;
        private IntPtr _imGuiTexture;

        private ImFontPtr font;

        public WestonClientGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferMultiSampling = true;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();

            _graphics.PreferredBackBufferWidth = ConfigSettings.Display.WindowWidth;
            _graphics.PreferredBackBufferHeight = ConfigSettings.Display.WindowHeight;
            _graphics.IsFullScreen = ConfigSettings.Display.FullScreen;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override unsafe void LoadContent()
        {
            // Texture loading example

			// First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
			_xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
			{
				var red = (pixel % 300) / 2;
				return new Color(red, 1, 1);
			});

			// Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
			_imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

            ImGuiIOPtr io = ImGui.GetIO();
            var nativeConfig = ImGuiNative.ImFontConfig_ImFontConfig();
            var fontConfig = new ImFontConfigPtr(nativeConfig);
            fontConfig.SizePixels = ConfigSettings.Display.FontSize;
            fontConfig.OversampleH = fontConfig.OversampleV = 1;
            fontConfig.PixelSnapH = true;

            font = io.Fonts.AddFontFromFileTTF(ConfigSettings.Display.FontPath, ConfigSettings.Display.FontSize, fontConfig);
            fontConfig.Destroy();

            _imGuiRenderer.RebuildFontAtlas();

            base.LoadContent();
        }

        private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        List<Window> windows = new List<Window>();

        #region context menu stuff
        public void DisplayContextMenu() {
            if (ImGui.BeginPopupContextWindow("Commands", ImGuiPopupFlags.MouseButtonRight)) {
                if (ImGui.Button("Spawn Weston Window")) spawnWestonWindow();
                ImGui.EndPopup();
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                ImGui.OpenPopup("Commands");
        }

        private void spawnWestonWindow() {
            windows.Add(new WestonWindow());
        }
        #endregion

        protected virtual void ImGuiLayout()
        {
            ImGui.PushFont(font);
            DisplayContextMenu();

            foreach (var window in windows) {
                window.Draw();
            }
            ImGui.PopFont();
        }

		public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
		{
			//initialize a texture
			var texture = new Texture2D(device, width, height);

			//the array holds the color for each pixel in the texture
			Color[] data = new Color[width * height];
			for(var pixel = 0; pixel < data.Length; pixel++)
			{
				//the function applies the color according to the specified pixel
				data[pixel] = paint( pixel );
			}

			//set the color
			texture.SetData( data );

			return texture;
		}
    }
}