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
        
        // Webpage file type, with preceding '.' - i.e. ".html"
        public const string WebpageFileType = ".html";
        // Link to the website your blog is hosted on
        public const string Website = "";
        // Path to the blog entry template file
        public const string BlogEntryTemplate = "";
        // Path to the blog index template file
        public const string BlogIndexTemplate = "";
        // Path to Blog directory of the website        
        public const string BlogDirectory = "";
        // Path to directory containing data files
        public const string DataDirectory = "";

        // RSS Feed Title
        public const string RssTitle = "";
        // RSS Language
        public const string RssLanguage = "en-uk";
        // RSS link to site's blog page
        public const string RssBlogLink = "";
        // Folder Containing RSS XML
        public const string RssFilePath = "";
        // RSS Description
        public const string RssDescription = "";
    }
}