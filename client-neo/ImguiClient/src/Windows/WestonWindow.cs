using System.Runtime.Versioning;
using ImGuiNET;
using LibWestonClient;
using System.IO;

using static WestonClient.Config.Settings;

namespace WestonClient {
    public class WestonWindow : Window { 
        private WestonConnection connection;

        public override void Widgets() {
            bool shouldMakeClient = ImGui.Button("make weston client");
            bool shouldReadFrame = ImGui.Button("read and save frame");

            if (shouldMakeClient) {
                connection = new WestonConnection(ConfigSettings.Client.ServerAddr, ConfigSettings.Client.Port);
            }

            if (shouldReadFrame) {
                if (connection != null) {
                    WCAPFrame frame = connection.ReadFrame();

                    foreach (WCAPRect r in frame.Rects) {
                        //WCAPRect r = frame.Rects[0];
                        File.WriteAllBytes(string.Format("rect-{0}-{1}x{2}.data", Guid.NewGuid().ToString(),
                                            r.Rect.Width, r.Rect.Height), r.PixelDataFlipped);
                    }
                }
            }
        }
    }
}