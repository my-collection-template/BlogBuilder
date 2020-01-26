namespace BlogBuilder
{
    class Config
    {
        // The title placeholder tag to look for in the blog template
        public const string PLACEHOLDER_TITLE = "##TITLE##";
        // The date placeholder tag to look for in the blog template
        public const string PLACEHOLDER_DATE = "##DATE##";
        // The body placeholder tag to look for in the blog template
        public const string PLACEHOLDER_BODY = "##BODY##";
        
        // File type for created webpage files, with preceding '.' - i.e. ".html"
        public const string WebpageFileType = ".html";
        // Link to the website your blog is hosted on
        public const string Website = "www.test-blog.com";
        // Path to the blog entry template file
        public const string BlogEntryTemplate = "../../../example/templates/blog-entry-template.html";
        // Path to the blog index template file
        public const string BlogIndexTemplate = "../../../example/templates/blog-index-template.html";
        // Path to Blog directory of the website        
        public const string BlogDirectory = "../../../example/blog";
        // Path to directory containing data files
        public const string DataDirectory = "../../../example/data-files";

        // RSS Feed Title
        public const string RssTitle = "My RSS Feed";
        // RSS Language
        public const string RssLanguage = "en-uk";
        // RSS link to site's blog page
        public const string RssBlogLink = "www.test-blog.com/blog";
        // Folder Containing RSS XML
        public const string RssFilePath = "../../../example/blog";
        // RSS Description
        public const string RssDescription = "An example RSS feed";
    }
}