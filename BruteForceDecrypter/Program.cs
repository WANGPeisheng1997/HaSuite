using System;
using System.IO;
using MapleLib.MapleCryptoLib;
using MapleLib.WzLib;
using MapleLib.WzLib.Serialization;

namespace WZTool
{
    class Program
    {
        static WzDirectory Load_IMG_Folder(string directory_path, byte[] iv) {
            WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);
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
                /*
                foreach (var prop in img.WzProperties)
                {
                    Console.WriteLine(prop.ToString());
                }
                */
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
            /*
            WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);
            byte[] WZ_MapleWorldIV = new byte[4] { 0x00, 0x55, 0x15, 0x30 }; //标准答案 { 0xA2, 0x55, 0x15, 0x30 }

            for (byte i = 0; i< 255; i++)
            {
                WZ_MapleWorldIV[0] = i;
                WzImage img = imgDeserializer.WzImageFromIMGFile("D:\\MapleFire0.01\\UI\\BuffIcon.img", WZ_MapleWorldIV, "MapleWorld");
                //Console.WriteLine(img.WzProperties.Count);
                if (img.WzProperties.Count > 0)
                {
                    Console.WriteLine("解码成功！IV密钥是: {0}", BitConverter.ToString(WZ_MapleWorldIV));
                }
            }   
            */
            WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);
            WzImgSerializer imgSerializer = new WzImgSerializer();
            //byte[] WZ_MapleFireIV = new byte[4] { 0x76, 0x8D, 0x03, 0x2F };

            byte[] WZ_GMSIV = new byte[4] { 0x4D, 0x23, 0xC7, 0x2B };

            /*
            WzImage img = imgDeserializer.WzImageFromIMGFile("D:\\MapleFire0.01\\UI\\BuffIconN.img", WZ_GMSIV, "Maple");
            if (img.WzProperties.Count > 0)
            {
                Console.WriteLine("解码成功！IV密钥是: {0}", BitConverter.ToString(WZ_GMSIV));
            }
            else
            {
                Console.WriteLine("解码失败");
            }

            WzImage img2 = imgDeserializer.WzImageFromIMGFile("D:\\MapleFire0.01\\UI\\BuffIcon.img", CryptoConstants.WZ_MapleFireIV, "MapleFire");
            if (img2.WzProperties.Count > 0)
            {
                Console.WriteLine("解码成功！IV密钥是: {0}", BitConverter.ToString(CryptoConstants.WZ_MapleFireIV));
            }
            else
            {
                Console.WriteLine("解码失败");
            }
            */
            WzDirectory data = Load_IMG_Folder("F:\\GitHub\\MapleFire083Client\\Data", WZ_GMSIV);
            data.SetIV(CryptoConstants.WZ_MapleFireIV);
            imgSerializer.SerializeDirectory(data, "D:\\MapleFire0.01\\Data");
        }
    }
}
