using ImGuiNET;
using System;

namespace WestonClient {
    public class Window {
        public string Name = "Window";
        public bool Open = true;
        public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;

        private bool initialized = false;
        
        public Window() {
            Name = string.Format("Window-{0}", Guid.NewGuid().ToString());
        }

        public Window(string name) {
            Name = name;
        }

        public Window(string name, ImGuiWindowFlags flags) : this(name) {
            Flags = flags;
        }

        public virtual void Initialize() {}

        public virtual void Draw() {
            if (Open) {
                if (!initialized) {
                    Initialize();
                    initialized = true;
                }

                ImGui.Begin(Name, ref Open, Flags);
                Widgets();
                ImGui.End();
            }
        }

        public virtual void Widgets() {}
    }
}