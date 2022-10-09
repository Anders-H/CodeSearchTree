using System;

namespace CodeSearchTreeTest
{
    public class TextEnteredEventArgs : EventArgs
    {
        public string Entered { get; internal set; }

        internal TextEnteredEventArgs(string entered)
        {
            Entered = entered;
        }

        internal TextEnteredEventArgs() : this("")
        {
        }
    }
}