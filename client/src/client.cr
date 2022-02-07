require "raylib-cr"
require "socket"

module Client
  VERSION = "0.1.0"

  pixel_formats = {
    0x34325258 => "XRGB8888",
    0x34324258 => "XBGR8888",
    0x34325852 => "RGBX8888",
    0x34325842 => "BGRX8888"
  }

  client = TCPSocket.new("localhost", 5000)
  
  magic_number = client.read_bytes(UInt32)
  pixel_format = client.read_bytes(UInt32)
  frame_width  = client.read_bytes(UInt32)
  frame_height = client.read_bytes(UInt32)

  unless magic_number == 0x57434150
    puts "Invalid magic number 0x%s! (Expected 'WCAP') Aborting" % [magic_number.to_s(16)]
    exit 1
  end

  unless pixel_formats.has_key? pixel_format
    puts "Invalid pixel format 0x%s! Aborting" % [pixel_format.to_s(16)]
    exit 1
  end

  puts "Magic number: 0x%s     " % [magic_number.to_s(16)]
  puts "Pixel format: 0x%s (%s)" % [pixel_format.to_s(16), pixel_formats[pixel_format]]
  puts "Frame width : 0x%s (%s)" % [frame_width.to_s(16), frame_width]
  puts "Frame height: 0x%s (%s)" % [frame_height.to_s(16), frame_height]

  while true
    frame_timestamp = client.read_bytes(UInt32)#, IO::ByteFormat::NetworkEndian)
    frame_numrects  = client.read_bytes(UInt32)#, IO::ByteFormat::NetworkEndian)

    puts "FRAME: ms: %d / numrects: %d" % [frame_timestamp, frame_numrects]
    (1..frame_numrects).each do |i|
      x1 = client.read_bytes(Int32)#, IO::ByteFormat::NetworkEndian)
      y1 = client.read_bytes(Int32)#, IO::ByteFormat::NetworkEndian)
      x2 = client.read_bytes(Int32)#, IO::ByteFormat::NetworkEndian)
      y2 = client.read_bytes(Int32)#, IO::ByteFormat::NetworkEndian)

      puts "\tRECT (%d): %d %d %d %d" % [i, x1, y1, x2, y2]

      pixels_expected = (x2 - x1) * (y2 - y1)

      new_pixel_buf = Bytes.new(pixels_expected * 3)

      pixels_decoded = 0
      bytes_decoded = 0
      while pixels_decoded < pixels_expected
        # have to read the components backwards for endianness reasons ;)
        b = client.read_bytes(UInt8)
        g = client.read_bytes(UInt8)
        r = client.read_bytes(UInt8)
        x = client.read_bytes(UInt8)
        
        run_length : UInt32 = 0
        if x < 0xDF
          run_length = (x + 1).to_u32!
        elsif x >= 0xE0
          run_length = (1 << (x - 0xE0 + 7)).to_u32!
        end

        #puts "\t\tRUN: %d pixels of (%d, %d, %d)" % [run_length, r, g, b]

        pos = 0
        while pos < (run_length * 3)
          #puts "\t%d" % (pos + bytes_decoded)
          #puts "\t\tr: %d" % r
          new_pixel_buf[pos + bytes_decoded]     = r
          #puts "\t\tg: %d" % g
          new_pixel_buf[pos + bytes_decoded + 1] = g
          #puts "\t\tb: %d" % b
          new_pixel_buf[pos + bytes_decoded + 2] = b
          pos += 3
        end

        pixels_decoded += run_length
        bytes_decoded += (run_length * 3)
      end
    end

    #client.read(pixel_buf)
    #puts "RECT: ms: %d / number: %d" % [frame_timestamp, frame_numrects]
  end
  
  client.close
end
