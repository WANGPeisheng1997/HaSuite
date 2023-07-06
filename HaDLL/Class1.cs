using System;
using System.IO;
using MapleLib.MapleCryptoLib;
using MapleLib.WzLib;
using MapleLib.WzLib.Serialization;

namespace HaDLL
{
    class HaDLLProgram
    {
        static long totalImgCount = 0;
        static long currentImgCount = 0;

        static long ImgCount(string source_folder)
        {
            long count = 0;
            count += Directory.GetFiles(source_folder, "*.img").Length;
            foreach (var dir in Directory.GetDirectories(source_folder))
            {
                count += ImgCount(dir);
            }
            return count;
        }

        static WzDirectory Load_IMG_Folder(string directory_path, byte[] iv, bool recursive = true)
        {
            WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);
            WzDirectory img_dir = new WzDirectory();
            img_dir.Name = Path.GetFileName(directory_path);

            foreach (var img_file in Directory.GetFiles(directory_path, "*.img"))
            {
                currentImgCount += 1;
                string progress = string.Format("({0}/{1}, {2:P2}) ", currentImgCount, totalImgCount, (double)currentImgCount / totalImgCount);
                Console.WriteLine(progress + img_file);

                WzImage img = imgDeserializer.WzImageFromIMGFile(img_file, iv, Path.GetFileName(img_file));
                img.ParseImage();
                img.Changed = true;
                img_dir.AddImage(img);
            }

            if (recursive)
            {
                foreach (var subdir in Directory.GetDirectories(directory_path))
                {
                    Console.WriteLine(subdir);
                    img_dir.AddDirectory(Load_IMG_Folder(subdir, iv));
                }
            }

            return img_dir;
        }

        static bool Save_IMG_Folder(WzDirectory data, string directory_path, byte[] newiv)
        {
            WzImgSerializer imgSerializer = new WzImgSerializer();
            data.SetIV(newiv);
            imgSerializer.SerializeDirectory(data, directory_path);
            return true;
        }

        static bool Transform_IMG_Folder_Encoding(string source_folder, string dest_folder, byte[] old_iv, byte[] new_iv)
        {
            WzDirectory data = Load_IMG_Folder(source_folder, old_iv, false);
            Save_IMG_Folder(data, dest_folder, new_iv);
            data.ClearImages();

            foreach (var subdir in Directory.GetDirectories(source_folder))
            {
                string dir_name = Path.GetFileName(subdir);
                Transform_IMG_Folder_Encoding(subdir, System.IO.Path.Combine(dest_folder, dir_name), old_iv, new_iv);
            }

            return true;
        }
    }
}
