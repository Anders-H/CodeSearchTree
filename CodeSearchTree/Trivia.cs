namespace CodeSearchTree
{
    public class Trivia
    {
        public enum TriviaTypes
        {
            UnknownTriviaSyntaxType,
            RegionDirectiveTriviaSyntaxType,
            SingleLineCommentTriviaType,
            EndRegionDirectiveTriviaType,
            MultiLineCommentTriviaType,
            SingleLineDocumentationCommentTriviaType,
            IfDirectiveTriviaType,
            DisabledTextTriviaType,
            ElseDirectiveTriviaType,
            PragmaChecksumDirectiveTriviaType,
            LineDirectiveTriviaType,
            EndIfDirectiveTriviaType
        }

        internal Trivia() : this(TriviaTypes.UnknownTriviaSyntaxType, "")
        {
        }

        internal Trivia(TriviaTypes triviaTypes, string source)
        {
            TriviaType = triviaTypes;
            Source = source;
        }

        public TriviaTypes TriviaType { get; internal set; }
        public string Source { get; internal set; }

        public override string ToString() => $"{TriviaType}: {Source}";
    }
}