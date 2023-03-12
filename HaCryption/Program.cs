using System;
using System.IO;
using MapleLib.MapleCryptoLib;
using MapleLib.WzLib;
using MapleLib.WzLib.Serialization;

namespace HaCryption
{
    class Program
    {
        static WzDirectory Load_IMG_Folder(string directory_path, byte[] iv)
        {
            WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);
            WzImgSerializer imgSerializer = new WzImgSerializer();
            WzDirectory img_dir = new WzDirectory();
            img_dir.Name = Path.GetFileName(directory_path);

            foreach (var img_file in Directory.GetFiles(directory_path, "*.img"))
            {
                Console.WriteLine(img_file);
                WzImage img = imgDeserializer.WzImageFromIMGFile(img_file, iv, Path.GetFileName(img_file));
                Console.WriteLine(img.WzProperties.Count);
                Console.WriteLine(img.Parsed.ToString());
                img.Changed = true;
                img_dir.AddImage(img);
            }

            foreach (var subdir in Directory.GetDirectories(directory_path))
            {
                Console.WriteLine(subdir);
                img_dir.AddDirectory(Load_IMG_Folder(subdir, iv));
            }
            return img_dir;
        }

        static void Main(string[] args)
        {
            WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);
            WzImgSerializer imgSerializer = new WzImgSerializer();
            WzDirectory data = Load_IMG_Folder("F:\\GitHub\\MapleFire083Client\\Data", CryptoConstants.WZ_GMSIV);
            data.SetIV(CryptoConstants.WZ_MapleFireIV);
            imgSerializer.SerializeDirectory(data, "D:\\MapleFire0.01\\Data");
        }
    }
}
