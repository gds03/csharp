using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace ExtensionMethods.Utilities
{
    /// <summary>
    ///     Provide static methods to manipulate word files
    /// </summary>
    public static class MicrosoftWordUtils
    {
        /// <summary>
        ///     Copy a new file from srcFilename to destFilename and replaces all occurrences in the keys of the dictionary by their values.
        /// </summary>
        public static void PerformReplaces(string srcFilename, string destFilename, Dictionary<string, object> tags)
        {
            if ( string.IsNullOrEmpty(srcFilename) )
                throw new ArgumentNullException("srcFilename cannot be null");

            if ( string.IsNullOrEmpty(destFilename) )
                throw new ArgumentNullException("destFilename cannot be null");

            if ( !File.Exists(srcFilename) )
                throw new InvalidOperationException(string.Format("File [From] with path {0} doens't exists.", srcFilename));

            if ( File.Exists(destFilename) )
            {
                File.Delete(destFilename);
            }

            //
            // Generate dest file

            File.Copy(srcFilename, destFilename);       // Copy identical file

            ApplicationClass app = null;
            Document doc = null;
            object missing = System.Type.Missing;

            try
            {
                app = new ApplicationClass();
                doc = app.Documents.Open(destFilename);

                foreach ( string tag in tags.Keys )
                {
                    foreach ( Range range in doc.StoryRanges )
                    {
                        //
                        // Set the text to find and replace

                        range.Find.Text = tag;
                        range.Find.Replacement.Text = tags[tag].ToString();
                        range.Find.Wrap = WdFindWrap.wdFindContinue;            // don't ask to user anything.

                        object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;

                        range.Find.Execute(ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref replaceAll,
                            ref missing, ref missing, ref missing, ref missing);
                    }
                }

                // Store to disk information
                doc.Save();
            }

            finally
            {
                if ( doc != null )
                {
                    doc.Close(ref missing, ref missing, ref missing);
                    doc = null;
                }

                if ( app != null )
                {
                    app.Quit(ref missing, ref missing, ref missing);
                    app = null;
                }
            }
        }
    }
}
