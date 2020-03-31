using System.Drawing;
using System.IO;
namespace Pk2ReaderAPI.Formats
{
    public class JMXVDDJ
    {
        #region Public Properties
        public string Header { get; private set; }
        public uint TextureSize { get; private set; }
        public uint TextureLength { get; private set; }
        public Bitmap Texture { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a DDJ from stream
        /// </summary>
        /// <param name="ddjStream">The stream which contains the DDJ file</param>
        public JMXVDDJ(Stream ddjStream)
        {
            using (BinaryReader reader = new BinaryReader(ddjStream))
            {
                ReadFormat(reader);
            }
        }
        #endregion

        #region Private Helpers
        private void ReadFormat(BinaryReader reader){ 
            Header = new string(reader.ReadChars(12));
            TextureSize = reader.ReadUInt32();
            TextureLength = reader.ReadUInt32();
            Texture = DDSToBitmapConverter(reader.ReadBytes((int)reader.BaseStream.Length - 20));
        }
        private Bitmap DDSToBitmapConverter(byte[] DDSBuffer)
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, DDSBuffer);
            Bitmap bmp = DevIL.DevIL.LoadBitmap(tempFile);
            File.Delete(tempFile);
            return bmp;
        }
        #endregion
    }
}
