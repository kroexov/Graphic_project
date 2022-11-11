using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Lab1.Models
{
    public class BitmapCreate
    {
        private PixelFormat _pixelFormat;
        private int _columns;
        private int _rows;
        private int _stride;
        private string _path;

        public Bitmap CreateP5Bit8(FileHeaderInfo header, byte[] bytes)
        {
            _pixelFormat = PixelFormat.Format8bppIndexed;
            _columns = header.Width;
            _rows = header.Height;
            _stride = _columns;

            Bitmap im = new Bitmap(_columns, _rows, _stride, 
                _pixelFormat, 
                Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0));
            
            ColorPalette _palette = im.Palette;
            Color[] _entries = _palette.Entries;
            for (int i = 0; i < 256; i++)
            {
                Color b = new Color();
                b = Color.FromArgb((byte)i, (byte)i, (byte)i);
                _entries[i] = b;
            }
            im.Palette = _palette;

            return im;
        }

        public Bitmap CreateP6Bit8(FileHeaderInfo header, byte[] bytes)
        {
            _pixelFormat = PixelFormat.Format24bppRgb;
            
            _columns = header.Width;
            _rows = header.Height;
            _stride = _columns * 3;

            var bytePtr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, bytePtr, bytes.Length);
            var scan0 = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);

            Bitmap im = new Bitmap(_columns, _rows, _stride, 
                _pixelFormat, 
                scan0);

            return im;
        }
    }
}