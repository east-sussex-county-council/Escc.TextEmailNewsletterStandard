using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Escc.TextEmailNewsletterStandard
{
    /// <summary>
    /// A section of a plain text email newsletter which conforms to the Text Email Newsletter (TEN) standard at http://www.headstar.com/ten/
    /// </summary>
    public class TextEmailNewsletterSection
    {
        #region Fields

        /// <summary>
        /// Store the title of the section
        /// </summary>
        private string title = "";

        /// <summary>
        /// Store the articles in the section
        /// </summary>
        private Collection<TextEmailNewsletterArticle> articles;

        #endregion

        #region Properties

        /// <summary>
        /// Gets articles in the section
        /// </summary>
        public Collection<TextEmailNewsletterArticle> Articles
        {
            get
            {
                return this.articles;
            }

        }

        /// <summary>
        /// Gets or sets the title of the section
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// A section of a plain text email newsletter which conforms to the Text Email Newsletter (TEN) standard at http://www.headstar.com/ten/
        /// </summary>
        public TextEmailNewsletterSection()
        {
            this.articles = new Collection<TextEmailNewsletterArticle>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the text of the section formatted according to the TEN standard
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(1, 1);
        }

        /// <summary>
        /// Gets a contents listing for the section formatted according to the TEN standard
        /// </summary>
        /// <param name="sectionNumber">The number of the current section</param>
        /// <param name="startItemNumber">Start numbering of articles at this number</param>
        /// <returns></returns>
        public string ToContentsString(int sectionNumber, int startItemNumber)
        {
            StringBuilder sb = new StringBuilder();

            // build the title of the section
            sb.Append(Environment.NewLine);
            sb.Append(Properties.Resources.Section + " ");
            sb.Append(sectionNumber.ToString(CultureInfo.CurrentCulture));
            sb.Append(Properties.Resources.SectionNumberSeparator);
            sb.Append(TextEmailNewsletter.EnsureFullStop(this.Title));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            // get a listing of each article in the section
            for (int i = 0; i < this.articles.Count; i++)
            {
                sb.Append(this.articles[i].ToContentsString(startItemNumber));
                startItemNumber++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the text of the section formatted according to the TEN standard
        /// </summary>
        /// <param name="sectionNumber">The number of this section in the newsletter (eg Section 2)</param>
        /// <param name="startItemNumber">The number of the first article in this section (article numbering continues across sections)</param>
        /// <returns></returns>
        public string ToString(int sectionNumber, int startItemNumber)
        {
            StringBuilder sb = new StringBuilder();

            // build the title of the section
            if (this.title.Length > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append(Properties.Resources.SectionStructurePrefix + Properties.Resources.Section + " ");
                sb.Append(sectionNumber.ToString(CultureInfo.CurrentCulture));
                sb.Append(Properties.Resources.SectionNumberSeparator);
                sb.Append(TextEmailNewsletter.EnsureFullStop(this.title));
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            // build each article in the section
            for (int i = 0; i < this.articles.Count; i++)
            {
                sb.Append(this.articles[i].ToString(startItemNumber));
                startItemNumber++;
            }

            // build a section footer (specified by the TEN standard)
            if (this.title.Length > 0)
            {
                sb.Append("[");
                sb.Append(this.title);
                sb.Append(" " + Properties.Resources.Section.ToLower(CultureInfo.CurrentCulture) + Properties.Resources.Ends + "].");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion
    }
}
