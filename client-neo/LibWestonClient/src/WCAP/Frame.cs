using System.Net.Sockets;

namespace LibWestonClient {
    public class WCAPFrame {
        public WCAPFrame(UInt32 ms, UInt32 numRects) {
            this.Timestamp = ms;
            this.NumRects = numRects;
            this.Rects = new WCAPRect[numRects];
        }

        public WCAPFrame(NetworkStream netStream) {
            this.stream = netStream;

            // read timestamp and number of rects (2 32 bit unsigned ints)
            netStream.Read(buffer, 0, 8);

            this.Timestamp = BitConverter.ToUInt32(buffer, 0);  // timestamp
            this.NumRects = BitConverter.ToUInt32(buffer, 4);   // number of rects
            this.Rects = new WCAPRect[NumRects];
        }

        public UInt32 Timestamp { get; }
        public UInt32 NumRects { get; }
        public WCAPRect[] Rects { get; }

        private NetworkStream stream;
        private byte[] buffer = new byte[2048];
        private int nextRectLoc = -1;

        public bool AddRect(WCAPRect rect) {
            nextRectLoc++;

            if (nextRectLoc > this.Rects.Length) {
                // FIXME: log the error
                return false;
            }

            Rects[nextRectLoc] = rect;
            return true;
        }

        public bool ReadAllRects() {
            for (int i = 0; i < NumRects; i++) {
                WCAPRect rect = new WCAPRect(stream);
                rect.DecodeFromStream();

                bool res = AddRect(rect);
                if (!res) return res;
            }

            return true;
        }
    }
}