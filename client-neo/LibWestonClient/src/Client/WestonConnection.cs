using System.Net.Sockets;

namespace LibWestonClient {
    public enum WestonPixelFormat : UInt32 {
        XRGB8888 = 0x34325258,
        XBGR8888 = 0x34324258,
        RGBX8888 = 0x34325852,
        BGRX8888 = 0x34325842
    }

    public class WestonConnection {
        public WestonConnection(string serverAddr, Int32 port) {
            this.client = new TcpClient(serverAddr, port);
            this.stream = client.GetStream();
            stream.ReadTimeout = 5000;

            // read header (4 32 bit unsigned ints)
            stream.Read(buffer, 0, 16);
            UInt32 magicNumber = BitConverter.ToUInt32(buffer, 0);
            PixelFormat = (WestonPixelFormat) BitConverter.ToUInt32(buffer, 4);
            FrameWidth  = BitConverter.ToUInt32(buffer, 8);
            FrameHeight = BitConverter.ToUInt32(buffer, 12);

            if (magicNumber == 0x57434150) { // "WCAP"
                Console.WriteLine("magic number correct!");
            } else {
                Console.WriteLine("unexpected magic number 0x{0:x}", magicNumber);
                throw new InvalidDataException();
            }

            Console.WriteLine("pixel format {0}", PixelFormat.ToString());
            Console.WriteLine("frame width  0x{0:x} // decimal: {0}", FrameWidth);
            Console.WriteLine("frame height 0x{0:x} // decimal: {0}", FrameHeight);

            if (PixelFormat != WestonPixelFormat.XRGB8888) // FIXME!
                throw new NotImplementedException("FIXME! Only XRGB8888 is implemented right now :(");
        }

        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer = new byte[2048];

        // header (missing magic number because we wont need it)
        public WestonPixelFormat PixelFormat { get; }
        public UInt32 FrameWidth  { get; }
        public UInt32 FrameHeight { get; }

        public WCAPFrame ReadFrame() {
            WCAPFrame frame = new WCAPFrame(stream); // number of rects expected
            frame.ReadAllRects();

            return frame;
        }
    }
}