using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
         LineDirectiveTriviaType
      }

      public TriviaTypes TriviaType { get; internal set; }
      public string Source { get; internal set; }

      internal Trivia() : this(TriviaTypes.UnknownTriviaSyntaxType, "")
      {
      }

      internal Trivia(TriviaTypes trivia_types, string source)
      {
         this.TriviaType = trivia_types;
         this.Source = source;
      }

      public override string ToString() => $"{this.TriviaType}: {this.Source}";
   }
}
