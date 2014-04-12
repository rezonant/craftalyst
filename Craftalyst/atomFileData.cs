using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.IO.Compression;
using System.Threading;

namespace AtomLauncher
{
    class atomFileData
    {
        internal static string userDataFile = @".\ALUData";
        internal static string gameDataFile = @".\ALGData";
        internal static string configFile = @".\ALConfig.alcfg";
        public static Dictionary<string, string> config;

        public static Dictionary<int, string[]> fileCheck(Dictionary<int, string[]> dict, string location)
        {
            Dictionary<int, string[]> tmpDict = new Dictionary<int, string[]>();
            int l = 0;
            int x = 0;
            while (l < dict.Count)
            {
                if (atomLauncher.cancelPressed) throw new System.Exception("Checking Files");
                if (File.Exists(location + @"\" + dict[l][1]))
                {
                    //string localChecksum = "ff344e7bc6007fade349565d545fd3e7"; //Development Temp.
                    //string fileChecksum = "";
                    //using (var md5 = MD5.Create())
                    //{
                    //    using (var stream = File.OpenRead(location + @"\" + urlAddress[l][2] + @"\" + urlAddress[l][1]))
                    //    {
                    //        fileChecksum = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    //    }
                    //}
                    //if (l == 1)
                    //{
                    //    if (fileChecksum != localChecksum)
                    //    {
                    //        doSkip = true;
                    //        tmpDict.Add(x, urlAddress[l]);
                    //        x++;
                    //    }
                    //}
                }
                else
                {
                    tmpDict.Add(x, dict[l]);
                    x++;
                }
                l++;
            }
            return tmpDict;
        }

        /// <summary>
        /// Return defaults for the config file or Dictonary config.
        /// </summary>
        /// <param name="game">Select the the title for the config to load.</param>
        /// <returns>Return defaults for the config file or Dictonary config.</returns>
        internal static Dictionary<string, string> loadConfDefaults()
        {
            Dictionary<string, string> dict = new Dictionary<string, string> {
                {"lastSelectedGame", ""},
                {"launcherVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString()},
                {"debug", "false"}
            };
            return dict;
        }
        /// <summary>
        /// Load the config file. If none, load defaults.
        /// </summary>
        /// <param name="location">Location of config file Example: "C:\LOCATION\file.config"</param>
        /// <returns>Returns the loaded config file.</returns>
        internal static Dictionary<string, string> loadConfFile(string pathFile)
        {
            var dict = new Dictionary<string, string>();
            dict = loadConfDefaults();
            if (File.Exists(pathFile))
            {
                string[] getArray = File.ReadAllLines(pathFile);
                for (int i = 0; i < getArray.Length; i++)
                {
                    if (getArray[i] != "" && getArray[i].Contains("=") && !getArray[i].StartsWith(";") && !getArray[i].StartsWith("["))
                    {
                        string[] splitArray = getArray[i].Split('=');
                        dict[splitArray[0]] = splitArray[1];
                    }
                }
            }
            return dict;
        }
        /// <summary>
        /// Save the the dictonary to a config file.
        /// </summary>
        /// <param name="location">Location to save to. Example: "C:\LOCATION\file.config"</param>
        /// <param name="dict">Input a dictonary of "string, string" here </param>
        internal static void saveConfFile(string pathFile, Dictionary<string, string> dict)
        {
            string[] setArray = { "" };
            int x = 0;
            foreach (KeyValuePair<string, string> entry in dict)
            {
                if (x > setArray.Length - 1)
                {
                    Array.Resize(ref setArray, setArray.Length + 1);
                }
                setArray[x] = entry.Key + "=" + entry.Value;
                x++;
            }
            File.WriteAllLines(pathFile, setArray);
        }

        /// <summary>
        /// Load and set the Game Data and Settings
        /// </summary>
        /// <param name="pathFile">Set this to the file that stores the data.</param>
        /// <param name="defaultGame">Input game you wish to reset. If this is set, it will not load from file. It requires a dictonary to be input.</param>
        /// <param name="returnDict">Input dictonary to be parsed and defaults set for a specific game.</param>
        /// <param name="gameType">Input the name to get the defaults of.</param>
        /// <returns></returns>
        internal static Dictionary<string, Dictionary<string, string[]>> getGameData(string pathFile, string defaultGame = "", Dictionary<string, Dictionary<string, string[]>> returnDict = null, string gameType = "")
        {
            Dictionary<string, Dictionary<string, string[]>> defaultDict = new Dictionary<string, Dictionary<string, string[]>>{
                {
                    "Minecraft", new Dictionary<string, string[]> { 
                        { "gameType",      new string[] { "Minecraft" } }, //Used to Reference Defaults
                        { "location",      new string[] { atomProgram.appData + @"\.minecraft" } },
                        { "saveLoc",       new string[] { atomProgram.appData + @"\.minecraft" } },
                        { "thumbnailLoc",  new string[] { "" } },
                        { "startRam",      new string[] { "512" } },
                        { "maxRam",        new string[] { "1024" } },
                        { "displayCMD",    new string[] { "False" } },
                        { "CPUPriority",   new string[] { "Normal" } },
                        { "autoLoginUser", new string[] { "" } },
                        { "onlineMode",    new string[] { "True" } },
                        { "offlineName",   new string[] { "Player" } },
                        { "selectVer",     new string[] { "Latest: Recommended" } },
                        { "autoSelect",    new string[] { "True" } },
                        { "useNightly",    new string[] { "False" } },
                        { "force64Bit",    new string[] { "False" } }
                    } 
                }
            };
            if (defaultGame != "")
            {
                if (returnDict == null)
                {
                    throw new System.Exception("Error: Need a Dictonary to set defaults for " + defaultGame);
                }
                if (gameType == "")
                {
                    throw new System.Exception("Error: Select Game Type");
                }
                returnDict[defaultGame] = defaultDict[gameType];
                return returnDict;
            }
            else
            {
                Dictionary<string, Dictionary<string, string[]>> loadedDict = new Dictionary<string, Dictionary<string, string[]>>();
                if (File.Exists(pathFile))
                {
                    loadedDict = loadDictonary(pathFile);

                    foreach (KeyValuePair<string, Dictionary<string, string[]>> game in loadedDict)
                    {
                        foreach (KeyValuePair<string, string[]> item in defaultDict[game.Value["gameType"][0]])
                        {
                            if (!loadedDict[game.Key].ContainsKey(item.Key))
                            {
                                loadedDict[game.Key][item.Key] = item.Value;
                            }
                        }
                    }
                }
                return loadedDict;
            }
        }
        internal static Dictionary<string, Dictionary<string, string[]>> getUserData(string pathFile)
        {
            Dictionary<string, Dictionary<string, string[]>> loadedDict = new Dictionary<string, Dictionary<string, string[]>>();//{
            //    {
            //        "Minecraft", new Dictionary<string, string[]> { 
            //            { "UserName", new string[] { "Propper Username", "Encrypted Password", "Last Saved Date and Time", "Access Token", "Client Token", "Universally Unique Identifier"} }
            //        } 
            //    }
            //};
            if (File.Exists(pathFile))
            {
                loadedDict = loadDictonary(pathFile, true);
            }
            return loadedDict;
        }
        
        /// <summary>
        /// Save the programs Dictonary
        /// </summary>
        /// <param name="pathFile">The save location and file name.</param>
        /// <param name="data">The input dictonary</param>
        /// <param name="arrayThree">The Format of the dictonary to save. UserData is true. GameData is false</param>
        internal static void saveDictonary(string pathFile, Dictionary<string, Dictionary<string, string[]>> inData, bool arraySix = false)
        {
            using (var file = File.Create(pathFile))
            using (var deflate = new DeflateStream(file, CompressionMode.Compress))
            using (var writer = new BinaryWriter(deflate))
            {
                writer.Write(inData.Count);
                foreach (var pair in inData)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value.Count);
                    foreach (var subpair in pair.Value)
                    {
                        writer.Write(subpair.Key);
                        writer.Write(subpair.Value[0]);
                        if (arraySix)
                        {
                            writer.Write(subpair.Value[1]);
                            writer.Write(subpair.Value[2]);
                            writer.Write(subpair.Value[3]);
                            writer.Write(subpair.Value[4]);
                            writer.Write(subpair.Value[5]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Reads the saved dictonary.
        /// </summary>
        /// <param name="pathFile">The read location and file name.</param>
        /// <param name="arrayThree">The Format of the dictonary to read. UserData is true. GameData is false</param>
        /// <returns></returns>
        internal static Dictionary<string, Dictionary<string, string[]>> loadDictonary(string pathFile, bool arraySix = false)
        {
            using (var file = File.OpenRead(pathFile))
            using (var deflate = new DeflateStream(file, CompressionMode.Decompress))
            using (var reader = new BinaryReader(deflate))
            {
                int count = reader.ReadInt32();
                var data = new Dictionary<string, Dictionary<string, string[]>>(count);
                while (count-- > 0)
                {
                    Dictionary<string, string[]> subdata = new Dictionary<string, string[]>();
                    string stringDict = reader.ReadString();
                    int subCount = reader.ReadInt32();
                    while (subCount-- > 0)
                    {
                        if (arraySix) 
                        {
                            subdata.Add(reader.ReadString(), new string[] { reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString() });
                        }
                        else 
                        { 
                            subdata.Add(reader.ReadString(), new string[] { reader.ReadString() }); 
                        };
                    }
                    data.Add(stringDict, subdata);
                }
                return data;
            }
        }
        
        public static void queueDelete(string pathFILE)
        {
            Thread delQuT = new Thread(() => deleteLoop(pathFILE, true));
            delQuT.Start();
        }
        public static string deleteLoop(string pathFILE, bool displayMessageBox = false)
        {
            string status = "";
            int x = 0;
            while (true)
            {
                bool tempBool = tryToDelete(pathFILE);
                if (tempBool)
                {
                    break;
                }
                Thread.Sleep(1000);
                if (x > 10)
                {
                    try
                    {
                        File.Delete(pathFILE);
                    }
                    catch (Exception ex)
                    {
                        status = ex.Message;
                        if (displayMessageBox) MessageBox.Show(ex.Message);
                    }
                    break;
                }
                x++;
            }
            return status;
        }
        public static bool tryToDelete(string pathFILE)
        {
            if (File.Exists(pathFILE))
            {
                try
                {
                    File.Delete(pathFILE);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
