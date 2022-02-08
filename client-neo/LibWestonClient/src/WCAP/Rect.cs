using System.Drawing;
using System.Net.Sockets;

namespace LibWestonClient {
    public class WCAPRect {
        
        public WCAPRect(Rectangle rect) {
            this.Rect = rect;
        }

        public WCAPRect(NetworkStream netStream) {
            this.stream = netStream;

            // read rectangle (4 int32_t)
            netStream.Read(buffer, 0, 16);

            Int32 x      = BitConverter.ToInt32(buffer, 0);
            Int32 y      = BitConverter.ToInt32(buffer, 4);
            Int32 width  = BitConverter.ToInt32(buffer, 8) - x;
            Int32 height = BitConverter.ToInt32(buffer, 12) - y;

            Console.WriteLine("{0}x{1} {2}", width, height, width*height);

            this.Rect = new Rectangle(x, y, width, height);
            this.NumberOfPixels = width * height;
            this.PixelData = new byte[NumberOfPixels * 3];
        }

        public Rectangle Rect { get; }
        public int NumberOfPixels { get; }
        public byte[] PixelData { get; }
        public byte[] PixelDataFlipped;

        private byte[] buffer = new byte[2048];
        private NetworkStream stream;
        private uint bytesDecoded;

        public void DecodeFromStream() {
            uint pixelsDecoded = 0;
            
            // FIXME: handle other pixel formats
            // decode all runs
            //Console.WriteLine("starting to decode runs");
            while (pixelsDecoded < NumberOfPixels) {
                // read the color of the run (4 bytes: b g r x)
                //Console.WriteLine("reading the color/length of the run");
                try {
                    stream.Read(buffer, 0, 4);
                } catch (IOException e) {
                    Console.WriteLine("no more runs? pixels decoded: {0} expected {1}", pixelsDecoded, NumberOfPixels);
                    break; // no more runs
                }

                byte b = buffer[0];
                byte g = buffer[1];
                byte r = buffer[2];
                byte x = buffer[3];

                // determine run length (see wcap docs)
                UInt32 runLength = 0;
                if (x < 0xDF)
                    runLength = (UInt32)(x + 1);
                else if (x >= 0xE0)
                    runLength = (UInt32)(1 << (x - 0xE0 + 7));
                
                // put our run in the pixeldata array
                int pos = 0;
                //Console.WriteLine("decoding run into pixeldata array");
                while (pos < (runLength * 3)) {
                    long desiredPos = pos + bytesDecoded;
                    
                    if (desiredPos < PixelData.Length) {
                        PixelData[desiredPos    ] = r;
                        PixelData[desiredPos + 1] = g;
                        PixelData[desiredPos + 2] = b;
                        pos += 3;
                    } else {
                        Console.WriteLine("oopsie! it won't fit!");
                        break;
                    }
                }

                pixelsDecoded += runLength;
                bytesDecoded += (runLength * 3);
            }

            Console.WriteLine("successfully decoded frame of {0} bytes long. expected {1} bytes", bytesDecoded, PixelData.Length);

            // FIXME: the pixel data is upside-down, for... some reason?
            //        (for now we just flip it. that's really slow. plz fix)
            List<byte> pixelDataFlipped = new List<byte>();
            byte[] pixelDataCopy = new byte[PixelData.Length];
            Array.Copy(PixelData, pixelDataCopy, PixelData.Length);

            for (int i = 0; i < Rect.Height; i++) {
                IEnumerable<byte> chunk = pixelDataCopy.TakeLast(Rect.Width * 3);
                pixelDataCopy = pixelDataCopy.SkipLast(chunk.ToArray().Length).ToArray();

                pixelDataFlipped = pixelDataFlipped.Concat(chunk).ToList();
            }

            this.PixelDataFlipped = pixelDataFlipped.ToArray();
        }
    }
}