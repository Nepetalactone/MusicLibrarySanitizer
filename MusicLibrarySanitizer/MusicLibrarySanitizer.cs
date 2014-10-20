using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MusicLibrarySanitizer
{
    class MusicLibrarySanitizer
    {
        private static readonly string MusicLocation = @"E:\Musik\Musik";
        private static readonly string Desktop = @"C:\Users\Tobias\Desktop";
        private static List<String> ErronousList;

        static void Main(string[] args)
        {
            ErronousList = new List<String>();
            ErronousList.Add("These albums do not conform to the albumname specification");
            foreach (String albumString in ValidateAlbums())
            {
                ErronousList.Add("\t" + albumString);
            }

            ErronousList.Add("\r\nThese folders do not conform to the folder content specification");
            foreach (String albumString in ValidateContentFolders())
            {
                ErronousList.Add("\t" + albumString);
            }

            ErronousList.Add("\r\nThese folders contain unknown files");
            foreach (String albumString in ContainsUnknownNonMusicNonPictureFiles())
            {
                ErronousList.Add("\t" + albumString);
            }

            ErronousList.Add("\r\nThese folders contain unwanted files");
            foreach (String albumString in ContainsMD5CueLogFiles())
            {
                ErronousList.Add("\t" + albumString);
            }

            SerializeErronousAlbums();
        }

        private static List<String> ValidateContentFolders()
        {
            List<String> folderList = new List<string>();
            DirectoryInfo musicDirectory = new DirectoryInfo(MusicLocation);

            foreach (DirectoryInfo albumDir in musicDirectory.GetDirectories())
            {
                foreach (DirectoryInfo contentInfo in albumDir.GetDirectories())
                {
                    if (contentInfo.Name != "Lyrics")
                    {
                        folderList.Add(albumDir + " -> " + contentInfo.Name);
                    }
                }
            }
            return folderList;
        }

        private static List<String> ValidateAlbums()
        {
            List<String> albumList = new List<string>();
            DirectoryInfo musicDirectory = new DirectoryInfo(MusicLocation);

            foreach (DirectoryInfo albumDir in musicDirectory.GetDirectories())
            {
                if (!AlbumFormatIsValid(albumDir.Name))
                {
                    albumList.Add(albumDir.Name);
                }
            }
            return albumList;
        }

        private static bool AlbumFormatIsValid(String name)
        {
            Char[] nameArray = name.ToCharArray();
            int i = 0;

            try
            {
                while (nameArray[i] != '-')
                {
                    i++;
                }
            }
            catch (IndexOutOfRangeException ind)
            {
                return false;
            }
            i = i + 2;

            for (int j = i; j < i + 4; j++)
            {
                if (!Char.IsNumber(nameArray[j]))
                {
                    return false;
                }
            }
            i = i + 4;
            if ((nameArray[i] == ' ') && (nameArray[i + 1] == '-') && (nameArray[i + 2] == ' ') && (nameArray.Length != i + 3))
            {
                return true;
            }
            return false;
        }

        private static List<String> ContainsUnknownNonMusicNonPictureFiles()
        {
            List<String> albumList = new List<string>();
            DirectoryInfo musicDirectory = new DirectoryInfo(MusicLocation);

            foreach (DirectoryInfo albumDir in musicDirectory.GetDirectories())
            {
                foreach (FileInfo file in albumDir.GetFiles())
                {
                    if ((!file.Name.EndsWith(".mp3")) 
                        && (!file.Name.EndsWith(".flac")) 
                        && (!file.Name.EndsWith(".ogg")) 
                        && (!file.Name.EndsWith(".jpg")) 
                        && (!file.Name.EndsWith(".png")) 
                        && (!file.Name.EndsWith(".bmp")) 
                        && (!file.Name.EndsWith(".gif")) 
                        && (!file.Name.EndsWith(".m4a")) 
                        && (!file.Name.EndsWith(".MP3")) 
                        && (!file.Name.EndsWith(".txt")) 
                        && (!file.Name.EndsWith(".TXT")) 
                        && (!file.Name.EndsWith(".JPG")) 
                        && (!file.Name.EndsWith(".wma")) 
                        && (!file.Name.EndsWith(".pdf"))
                        && (!file.Name.EndsWith(".Mp3"))
                        && (!file.Name.EndsWith(".ape"))
                        && (!file.Name.EndsWith(".jpeg")))
                    {
                        albumList.Add(albumDir.Name + " -> " + file.Name);
                    }
                }
            }
            return albumList;
        }

        private static List<String> ContainsMD5CueLogFiles()
        {
            List<String> albumList = new List<string>();
            DirectoryInfo musicDirectory = new DirectoryInfo(MusicLocation);

            foreach (DirectoryInfo albumDir in musicDirectory.GetDirectories())
            {
                foreach (FileInfo file in albumDir.GetFiles())
                {
                    if ((file.Name.EndsWith(".cue"))
                        || (file.Name.EndsWith(".md5"))
                        || (file.Name.EndsWith(".Log"))
                        || (file.Name.EndsWith(".Cue"))
                        || (file.Name.EndsWith(".MD5"))
                        || (file.Name.EndsWith(".LOG"))
                        || (file.Name.EndsWith(".cue"))
                        || (file.Name.EndsWith(".CUE"))
                        || (file.Name.EndsWith(".log")))
                    {
                        albumList.Add(albumDir.Name + " -> " + file.Name);
                    }
                }
            }
            return albumList;
        }

        private static void SerializeErronousAlbums()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(Desktop, "Musikfehler.txt"), false))
            {
                foreach (String error in ErronousList)
                {
                    file.WriteLine(error);
                }
            }
        }
    }
}
