using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeSearchTreeTest
{
    public class TextInput : TextBox
    {
        private new bool Multiline;
        private new bool AcceptsReturn;
        private int KeyDownEnd_End { get; set; }
        private int KeyDownEnd_Line { get; set; }
        private int KeyDownEnd_Start { get; set; }
        public event Action<object, TextEnteredEventArgs> Entered;

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
                text = Regex.Replace(text, @"\s", " ").Trim();
                Application.DoEvents();
                e.Handled = true;

                if (!string.IsNullOrWhiteSpace(text) && Entered != null)
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

            if (e.KeyCode != Keys.End)
                return;
            
            var end = SelectionStart;
            var line = GetLineFromCharIndex(end);

            if (line > KeyDownEnd_Line)
                SendKeys.Send("{LEFT}");
        }

        public void WriteLine(string text)
        {
            SelectionLength = 0;
            SelectionStart = TextLength;
            var line = GetLineFromCharIndex(SelectionStart);
            var lineStart = GetFirstCharIndexFromLine(line);
            text = Regex.Replace(text, @"\s", " ").Trim();
 
            if (lineStart != SelectionStart)
                AppendText("\n");
            
            AppendText(text);
            AppendText("\n");
            SelectionStart = TextLength;
            ScrollToCaret();
            Application.DoEvents();
        }
    }
}