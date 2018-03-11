//! \file       ImageYGA.cs
//! \date       2018 Mar 07
//! \brief      Yaneurao image format.
//
// Copyright (C) 2018 by morkt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//

using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Media;
using GameRes.Compression;

namespace GameRes.Formats.Yaneurao
{
    internal class YgaMetaData : ImageMetaData
    {
        public int  UnpackedSize;
    }

    [Export(typeof(ImageFormat))]
    public class YgaFormat : ImageFormat
    {
        public override string         Tag { get { return "YGA"; } }
        public override string Description { get { return "Yaneurao image format"; } }
        public override uint     Signature { get { return 0x616779; } } // 'yga'

        public override ImageMetaData ReadMetaData (IBinaryStream file)
        {
            var header = file.ReadHeader (0x18);
            if (header.ToInt32 (0xC) != 1)
                return null;
            return new YgaMetaData {
                Width  = header.ToUInt32 (4),
                Height = header.ToUInt32 (8),
                BPP    = 32,
                UnpackedSize = header.ToInt32 (0x10),
            };
        }

        public override ImageData Read (IBinaryStream file, ImageMetaData info)
        {
            var meta = (YgaMetaData)info;
            file.Position = 0x18;
            using (var input = new LzssStream (file.AsStream, LzssMode.Decompress, true))
            {
                var pixels = new byte[meta.UnpackedSize];
                input.Read (pixels, 0, meta.UnpackedSize);
                return ImageData.Create (info, PixelFormats.Bgra32, null, pixels);
            }
        }

        public override void Write (Stream file, ImageData image)
        {
            throw new System.NotImplementedException ("YgaFormat.Write not implemented");
        }
    }
}
