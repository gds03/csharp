using CodeToUMLNotation.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation
{
    public class RicherTextboxBuilder : IRichStringbuilder
    {
        public RicherTextbox RicherTextbox { get; private set; }

        public RicherTextboxBuilder(RicherTextbox richerTextbox)
        {
            ParameterValidator.ThrowIfArgumentNull(richerTextbox, "richerTextbox");
            RicherTextbox = richerTextbox;
        }

        void Write(String text, FontStyle style)
        {
            var currentFont = RicherTextbox.Font;        // save current font

            RicherTextbox.SelectionFont = new Font(currentFont, style);
            RicherTextbox.AppendText(text);
            RicherTextbox.SelectionFont = currentFont;
        }

        public IRichStringbuilder WriteBold(string text)
        {
            Write(text, FontStyle.Bold);
            return this;
        }

        public IRichStringbuilder WriteItalic(string text)
        {
            Write(text, FontStyle.Italic);
            return this;
        }

        public IRichStringbuilder WriteUnderline(string text)
        {
            Write(text, FontStyle.Underline);
            return this;
        }

        public IRichStringbuilder WriteRegular(string text)
        {
            Write(text, FontStyle.Regular);
            return this;
        }

        public IRichStringbuilder WriteLine()
        {
            Write("\n", FontStyle.Regular);
            return this;
        }

        public IRichStringbuilder DeleteLast(int numChars)
        {
            throw new NotImplementedException();
        }

        public int Length
        {
            get { return this.RicherTextbox.Text.Length; }
        }
    }
}
