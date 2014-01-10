using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Escc.TextEmailNewsletterStandard
{
    /// <summary>
    /// A plain text email newsletter which conforms to the Text Email Newsletter (TEN) standard at http://www.headstar.com/ten/
    /// </summary>
    public class TextEmailNewsletter
    {
        #region Fields

        private Collection<TextEmailNewsletterSection> sections = new Collection<TextEmailNewsletterSection>();

        #endregion

        #region Constructors

        /// <summary>
        /// A plain text email newsletter which conforms to the Text Email Newsletter (TEN) standard at http://www.headstar.com/ten/
        /// </summary>
        public TextEmailNewsletter()
        {
            this.ContentsSection = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets sections of the newsletter
        /// </summary>
        public Collection<TextEmailNewsletterSection> Sections
        {
            get
            {
                return this.sections;
            }
        }


        /// <summary>
        /// Gets or sets the title of the newsletter
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the strapline of the newsletter
        /// </summary>
        public string StrapLine { get; set; }

        /// <summary>
        /// Short introduction to appear between the strapline /contents and the first article
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// Gets or sets whether to generate a contents section
        /// </summary>
        public bool ContentsSection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the complete newsletter formatted according to the TEN standard
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Start with the newsletter title and strapline
            if (!this.ContentsSection && this.sections.Count == 1 && this.sections[0].Title.Length == 0)
            {
                // Special case if there are no sections (no contents, and the only section has no title)
                sb.Append(Properties.Resources.SectionStructurePrefix);
            }
            else
            {
                sb.Append(Properties.Resources.NewsletterStructurePrefix);
            }
            sb.Append(TextEmailNewsletter.EnsureFullStop(this.Title));
            if (!String.IsNullOrEmpty(this.StrapLine))
            {
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(TextEmailNewsletter.EnsureFullStop(this.StrapLine));
            }
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);


            // Only write body of newsletter if there are sections to write
            if (this.sections.Count > 0)
            {
                int i;
                int itemCount = 1; // itemCount is used to number the articles

                // Build contents
                if (this.ContentsSection)
                {
                    sb.Append(Properties.Resources.SectionStructurePrefix + TextEmailNewsletter.EnsureFullStop(Properties.Resources.Contents));
                    sb.Append(Environment.NewLine);

                    for (i = 0; i < this.sections.Count; i++)
                    {
                        sb.Append(this.sections[i].ToContentsString(i + 1, itemCount));
                        itemCount = itemCount + this.sections[i].Articles.Count;
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append("[" + Properties.Resources.Contents + Properties.Resources.Ends + "].");
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                }

                // Write intro
                if (!String.IsNullOrEmpty(this.Introduction))
                {
                    TextEmailNewsletterArticle intro = new TextEmailNewsletterArticle();
                    intro.Text = this.Introduction;
                    sb.Append(intro.FilteredText);
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                }

                // Write sections
                itemCount = 1;
                for (i = 0; i < this.sections.Count; i++)
                {
                    sb.Append(this.sections[i].ToString(i + 1, itemCount));
                    itemCount = itemCount + this.sections[i].Articles.Count;
                }
            }

            // Add a footer stating conformance with TEN standard
            sb.Append(Environment.NewLine);
            sb.Append(Properties.Resources.TenStandardLine1);
            sb.Append(Environment.NewLine);
            sb.Append(Properties.Resources.TenStandardLine2);
            sb.Append(Environment.NewLine);
            sb.Append(Properties.Resources.TenStandardUrl);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("[" + Properties.Resources.Newsletter + Properties.Resources.Ends + "].");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// The TEN standard says all titles/paras should end with a fullstop
        /// </summary>
        /// <param name="text">Text string</param>
        /// <returns>Text string with fullstop added where necessary</returns>
        internal static string EnsureFullStop(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                text = text.TrimEnd();
                if (text.Length > 0 &&
                    !text.EndsWith(".", StringComparison.Ordinal) &&
                    !text.EndsWith(":", StringComparison.Ordinal) &&
                    !text.EndsWith(";", StringComparison.Ordinal) &&
                    !text.EndsWith("!", StringComparison.Ordinal) &&
                    !text.EndsWith("?", StringComparison.Ordinal) &&
                    !text.EndsWith(",", StringComparison.Ordinal))
                {
                    if (text.StartsWith(Properties.Resources.ListItemPrefix, StringComparison.Ordinal))
                    {
                        text += ";";
                    }
                    else
                    {
                        text += ".";
                    }
                }
            }

            return text;
        }

        #endregion


    }
}
