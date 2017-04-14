using System.Text;
using System;

namespace StudioMobile
{
	public static class NamedFormat
	{
		struct FormatParser
		{
			int idx;
			StringBuilder result;
			StringBuilder tmp;
			Record context;
			readonly string format;
			FormatException error;

			public FormatParser (string format, Record context)
			{
				this.context = context;
				this.format = format;
				idx = 0;
				result = new StringBuilder();
				tmp = new StringBuilder();
				error = null;
			}

			public bool Parse (bool throwError)
			{
				while (!IsEOF ()) {
					
					if (LookingAt ('{')) {						
						if (LookingAt ('{', 1)) {
							//{{
							MatchOpenEscapeSeq ();
						} else {
							//{<ID>}
							MatchId ();
						}
					} else if (LookingAt ('}')) {						
						if (LookingAt ('}', 1)) {
							//}}
							MatchCloseEscapeSeq ();
						} else {
							//}
							error = new FormatException("Unexpected } at " + idx);
						}
					} else {
						result.Append (ConsumeChar ());
					}

					if (error != null) {
						if (throwError) {
							throw error;
						} else {
							return false;
						}
					} 
				}
				return true;
			}

			public string Result { get { return result.ToString (); } }

			bool IsEOF (int lookahead = 0)
			{
				return idx + lookahead >= format.Length;
			}

			bool LookingAt (char c, int lookahead = 0)
			{
				return !IsEOF(lookahead) && format [idx + lookahead] == c;
			}

			void MatchOpenEscapeSeq()
			{
				result.Append ('{');
				idx += 2;
			}

			void MatchCloseEscapeSeq()
			{
				result.Append ('}');
				idx += 2;
			}

			void MatchId()
			{
				tmp.Clear ();
				if (!Consume ('{'))
					return;
				while (!LookingAt ('}')) {
					if (IsEOF ())
						break;
					tmp.Append (ConsumeChar ());
				}
				if (!Consume ('}'))
					return;

				object value;
				if (context.TryGetValue (tmp.ToString (), out value)) {
					result.Append (value);
				}
			}

			bool Consume(char c) 
			{
				if (IsEOF ()) {
					error = new FormatException ("Unexected end of format string.");
				} else {
					if (c == format [idx]) {
						idx++;
					} else {
						error = new FormatException (string.Format ("Unexpected '{0}' at {1}. Expected '{2}'.", format [idx], idx, c));
					}
				}
				return error == null;
			}

			char ConsumeChar()
			{
				var c = format [idx];
				idx++;
				return c;
			}
		}

		public static string FormatWithObject (this string format, object obj)
		{
			var parser = new FormatParser (format, new Record (obj));
			parser.Parse (true);
			return parser.Result;				
		}

		public static bool TryFormatWithObject (this string format, object obj, out string result)
		{
			var parser = new FormatParser (format, new Record (obj));
			if (parser.Parse (false)) {
				result = parser.Result;	
				return true;
			} else {
				result = null;
				return false;
			}
		}
	}
}