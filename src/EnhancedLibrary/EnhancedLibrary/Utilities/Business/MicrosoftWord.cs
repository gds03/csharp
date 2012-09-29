using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;

namespace EnhancedLibrary.Utilities.Business
{
    /// <summary>
    ///     Provide static methods to manipulate word files
    /// </summary>
    public static class MicrosoftWord
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

            Application wordApp = null;
            Document wordDoc = null;
            object missing = System.Type.Missing;

            try
            {
                wordApp = new Application();
                wordDoc = wordApp.Documents.Open(destFilename);

                foreach ( string tag in tags.Keys )
                {
                    foreach ( Range range in wordDoc.StoryRanges )
                    {
                        //
                        // Set the text to find and replace

                        range.Find.Text = tag;
                        range.Find.Replacement.Text = tags[tag].ToString();
                        range.Find.Wrap = WdFindWrap.wdFindContinue;            // don't ask to user anything.

                        object replaceAll = WdReplace.wdReplaceAll;

                        range.Find.Execute(ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref replaceAll,
                            ref missing, ref missing, ref missing, ref missing);
                    }
                }

                // Store to disk information
                wordDoc.Save();
            }

            finally
            {
                ForceCleanup(ref wordApp, ref wordDoc, ref missing);
            }
        }






        #region Internal Methods

        
        static void ForceCleanup(ref Application wordApp, ref Document wordDoc, ref object missing)
        {
            if ( wordDoc != null )
            {
                wordDoc.Close(ref missing, ref missing, ref missing);
            }

            if ( wordApp != null )
            {
                wordApp.Quit(ref missing, ref missing, ref missing);
            }

            Marshal.FinalReleaseComObject(wordDoc);
            Marshal.FinalReleaseComObject(wordApp);

            wordDoc = null;
            wordApp = null;
        }


        #endregion
    }
}
