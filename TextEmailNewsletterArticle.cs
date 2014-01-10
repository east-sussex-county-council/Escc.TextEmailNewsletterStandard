using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Escc.TextEmailNewsletterStandard
{
    /// <summary>
    /// An article in a plain text email newsletter which conforms to the Text Email Newsletter (TEN) standard at http://www.headstar.com/ten/
    /// </summary>
    public class TextEmailNewsletterArticle
    {
        #region Fields

        /// <summary>
        /// Store the title of the article
        /// </summary>
        private string title = "";

        /// <summary>
        /// Store the text of the article
        /// </summary>
        private string text = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unformatted text of the article
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Gets the body text of the article, formatted according to the TEN standard
        /// </summary>
        public string FilteredText
        {
            get
            {
                string filteredText = this.text;

                // remove tags likely to be found within a link (thereby stopping it matching link regex)
                filteredText = Regex.Replace(filteredText, @"<acronym(\s[^>]+)*>([^>]+)</acronym>", "$2", RegexOptions.IgnoreCase);
                filteredText = Regex.Replace(filteredText, @"<abbr(\s[^>]+)*>([^>]+)</abbr>", "$2", RegexOptions.IgnoreCase);
                filteredText = Regex.Replace(filteredText, @"<strong>([^>]+)</strong>", "$2", RegexOptions.IgnoreCase);
                filteredText = Regex.Replace(filteredText, @"<em>([^>]+)</em>", "$2", RegexOptions.IgnoreCase);

                // expand links
                filteredText = filteredText.Replace(Environment.NewLine, Environment.NewLine + Environment.NewLine);
                filteredText = Regex.Replace(filteredText, @"<a\shref=[^A-Z](?<URL>[A-Za-z0-9:/?&.;%~=@#-_ ]+)[^A-Z](\s[^>]+)*>(?<LinkText>[^<]+)</a>", new MatchEvaluator(this.MatchEvaluator_ConvertLinks), RegexOptions.IgnoreCase);

                // convert bullets
                filteredText = Regex.Replace(filteredText, @"</?(u|o)l>\s*", "", RegexOptions.IgnoreCase);
                filteredText = filteredText.Replace("</li><p>", Environment.NewLine + Environment.NewLine); // space between this pair removed - side-effect of previous line
                filteredText = filteredText.Replace("<li>", Properties.Resources.ListItemPrefix);

                // remove HTML tags
                filteredText = Regex.Replace(filteredText, @"</?[a-z]+(\s[^>]+)*>", "", RegexOptions.IgnoreCase);

                // remove special characters
                filteredText = TextEmailNewsletterArticle.ExpandSpecialCharacters(filteredText);

                // split into 70 character lines
                filteredText = TextEmailNewsletterArticle.WrapLines(filteredText);

                return filteredText;
            }
        }

        /// <summary>
        /// Delegate for Regex used to translate links into text
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MatchEvaluator_ConvertLinks(Match m)
        {
            string url = m.Groups["URL"].Value.Trim();
            if (url.StartsWith("/", StringComparison.Ordinal)) url = Properties.Resources.VirtualUrlBase + url; // add domain to internal URLs
            if (url.ToLower(CultureInfo.CurrentCulture).StartsWith("mailto", StringComparison.Ordinal))
            {
                return m.Groups["LinkText"].Value;
            }
            else
            {
                return m.Groups["LinkText"].Value + Environment.NewLine + "#MatchedUrl#" + url + Environment.NewLine;
            }
        }

        /// <summary>
        /// Gets or sets the title of the article
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = TextEmailNewsletterArticle.ExpandSpecialCharacters(TextEmailNewsletter.EnsureFullStop(value));
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// An article in a plain text email newsletter which conforms to the Text Email Newsletter (TEN) standard at http://www.headstar.com/ten/
        /// </summary>
        public TextEmailNewsletterArticle()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a contents listing entry for this article
        /// </summary>
        /// <param name="itemNumber">The number of this article in the newsletter</param>
        /// <returns></returns>
        public string ToContentsString(int itemNumber)
        {
            StringBuilder sb = new StringBuilder();

            if (this.text.Length > 0)
            {
                if (itemNumber < 10) sb.Append("0");
                sb.Append(itemNumber.ToString(CultureInfo.CurrentCulture));
                sb.Append(Properties.Resources.ArticleNumberSeparator);
                sb.Append(this.title);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the complete text of the article formatted according to the TEN standard
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(1);
        }

        /// <summary>
        /// Gets the complete text of the article formatted according to the TEN standard
        /// </summary>
        /// <param name="itemNumber">The number of this article in the newsletter</param>
        /// <returns></returns>
        public string ToString(int itemNumber)
        {
            StringBuilder sb = new StringBuilder();

            if (this.text.Length > 0)
            {
                // add title
                sb.Append(Properties.Resources.ArticleStructurePrefix);
                if (itemNumber < 10) sb.Append("0");
                sb.Append(itemNumber.ToString(CultureInfo.CurrentCulture));
                sb.Append(Properties.Resources.ArticleNumberSeparator);
                sb.Append(this.title);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                // add text
                sb.Append(this.FilteredText);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert special characters into text-based equivalents (part of the TEN standard)
        /// </summary>
        /// <param name="text">Text which may contain special characters</param>
        /// <returns>Text with special characters converted</returns>
        private static string ExpandSpecialCharacters(string text)
        {
            text = text.Replace("%", Properties.Resources.SubstitutePercent);
            text = Regex.Replace(text, @"(£|&\#163;)([0-9,]+)(.[0-9][0-9]?)?([a-z]?)", Properties.Resources.SubstitutePounds);
            text = text.Replace("&#8211;", "-"); // n-dash to hyphen
            text = text.Replace("&#8216;", "'"); // curly quote to normal quote
            text = text.Replace("&#8217;", "'"); // curly quote to normal quote
            text = text.Replace("&#8230;", "..."); // ellipsis to ...
            text = HttpUtility.HtmlDecode(text); // resolve any other entities.

            return text;
        }

        /// <summary>
        /// Text should be wrapped at 70 characters wide for less capable email clients
        /// </summary>
        /// <param name="text">Text to be wrapped</param>
        /// <returns>Wrapped text</returns>
        public static string WrapLines(string text)
        {
            if (text == null) return String.Empty;

            int wrapAt = 70;

            StringBuilder sb = new StringBuilder();

            // remove \r to make new lines less complicated to deal with
            text = text.Replace("\r", "");

            // split text into lines to work with a line at a time
            string[] lines = text.Split('\n');
            string[] words;
            int iLine;
            int lineLength;

            for (iLine = 0; iLine < lines.Length; iLine++)
            {
                // leave URLs on their own line no matter how long
                // Note: MatchEvaluator_ConvertLinks will have put them on their own line when called from FilteredText property
                if (lines[iLine].StartsWith("#MatchedUrl#", StringComparison.Ordinal))
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(lines[iLine].Substring(12));
                    sb.Append(Environment.NewLine);
                    lineLength = 0;
                    continue;
                }

                // ensure line ends with fullstop, according to TEN spec
                lines[iLine] = TextEmailNewsletter.EnsureFullStop(lines[iLine]);

                // split into words so that we don't split lines mid-word
                words = lines[iLine].Trim().Split(' ');

                // monitor how long the line we're building has become
                lineLength = 0;
                int i = 0;

                do
                {
                    // an "empty" word was a paragraph break
                    if (words[i].Length == 0)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(Environment.NewLine);
                        i++;
                        continue;
                    }

                    // If adding this word would make the line too long, start a new line
                    if ((lineLength + words[i].Length) >= wrapAt)
                    {
                        sb.Append(Environment.NewLine);
                        lineLength = 0;
                    }

                    // make sure line doesn't start with a full stop (which can happen if a link is at the end of a sentence, but not the end of a para)
                    if (words[i] == ".")
                    {
                    }
                    else
                    {
                        // add the word
                        sb.Append(words[i] + " ");
                        lineLength += (words[i].Length + 1);
                    }
                    i++;
                }
                while (i < words.Length);
            }

            // don't allow it to end with a fullstop on a new line
            if (sb.ToString().EndsWith(Environment.NewLine + ". ", StringComparison.Ordinal))
            {
                // sb.Remove(sb.Length-2-Environment.NewLine.Length,Environment.NewLine.Length); // remove the newline
                sb.Remove(sb.Length - 2 - Environment.NewLine.Length, Environment.NewLine.Length + 1); // remove the newline and fullstop
            }

            return sb.ToString();
        }

        #endregion
    }
}
