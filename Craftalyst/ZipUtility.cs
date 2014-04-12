using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Craftalyst
{
    class ZipUtility
    {
        /// <summary>
        /// Decompress a zip file to a specified directory. Exclude by matching string.
        /// </summary>
        /// <param name="zipFileName">Location and file name of the zip file. Example: "C:\LOCATION\file.zip"</param>
        /// <param name="outputDirectory">Output of the files extracted. Example: "C:\LOCATION\"</param>
        /// <param name="excludeCard">Anything that matches any part of this string will not get extracted. Example: "blah"</param>
		public static void Extract(string zipFileName, string outputDirectory, string fileFilter = "")
        {
			FastZip zip = new FastZip();
			zip.ExtractZip(zipFileName, outputDirectory, fileFilter);
        }
    }
}
