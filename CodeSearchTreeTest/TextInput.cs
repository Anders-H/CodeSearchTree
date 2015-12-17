using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeSearchTreeTest
{
    public class TextInput : TextBox
    {
        private new bool Multiline => true;
        private new bool AcceptsReturn => true;
        public event Action<object, TextEnteredEventArgs> Entered;
        private int KeyDownEnd_End { get; set; } = 0;
        private int KeyDownEnd_Line { get; set; } = 0;
        private int KeyDownEnd_Start { get; set; } = 0;

        public TextInput()
        {
            base.Multiline = true;
            base.AcceptsReturn = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && !(e.Shift))
            {
                var end = SelectionStart;
                var line = GetLineFromCharIndex(end);
                var start = GetFirstCharIndexFromLine(line);
                var text = Text.Substring(start, end - start);
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\s", " ").Trim();
                Application.DoEvents();
                e.Handled = true;
                if (!(text == ""))
                    if (!(Entered == null))
                        Entered(this, new TextEnteredEventArgs(text));
                SelectionStart = TextLength;
                ScrollToCaret();
            }
            else if (e.KeyCode == Keys.End)
            {
                //Ibland (på sista raden oftast) fungerar inte End. Den tar oss till nästa rad istället.
                KeyDownEnd_End = SelectionStart;
                KeyDownEnd_Line = GetLineFromCharIndex(KeyDownEnd_End);
                KeyDownEnd_Start = GetFirstCharIndexFromLine(KeyDownEnd_Start);
                Application.DoEvents();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.End)
            {
                var end = SelectionStart;
                var line = GetLineFromCharIndex(end);
                var start = GetFirstCharIndexFromLine(line);
                if (line > KeyDownEnd_Line)
                    SendKeys.Send("{LEFT}");
            }
        }

        public void WriteLine(string text)
        {
            SelectionLength = 0;
            SelectionStart = TextLength;
            var line = GetLineFromCharIndex(SelectionStart);
            var lineStart = GetFirstCharIndexFromLine(line);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s", " ").Trim();
            if (!(lineStart == SelectionStart))
                AppendText("\n");
            AppendText(text);
            AppendText("\n");
            SelectionStart = TextLength;
            ScrollToCaret();
            Application.DoEvents();
        }
    }

    public class TextEnteredEventArgs : EventArgs
    {
        public string Entered { get; internal set; }

        internal TextEnteredEventArgs(string entered)
        {
            Entered = entered;
        }

        internal TextEnteredEventArgs() : this("")
        { }
    }
}
