namespace CodeSearchTree
{
    public class Trivia
    {
        public TriviaType TriviaType { get; internal set; }

        public string Source { get; internal set; }

        internal Trivia() : this(TriviaType.UnknownTriviaSyntaxType, "")
        {
        }
        
        internal Trivia(TriviaType triviaType, string source)
        {
            TriviaType = triviaType;
            Source = source;
        }

        public override string ToString() =>
            $"{TriviaType}: {Source}";
    }
}