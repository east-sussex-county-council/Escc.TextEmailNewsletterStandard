Escc.TextEmailNewsletterStandard
================================

Format a plain text email newsletter according to the Text Email Newsletter Standard 1.2 defined by Headstar at http://www.headstar.com/ten/. 

To create a newsletter conforming to the standard, follow 4 steps:

1. Create an instance of ```TextEmailNewsletter```
2. Create instances of ```TextEmailNewsletterSection``` and add them to the ```TextEmailNewsletter```
3. Create instances of ```TextEmailNewsletterArticle``` and add them to the ```TextEmailNewsletterSection```s
4. Call ```.ToString()``` on the ```TextEmailNewsletter```