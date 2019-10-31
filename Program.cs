using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlogBuilder
{
    class Program
    {        
        static List<BlogEntry> BlogEntries = new List<BlogEntry>();
        static bool RefreshRan = false; // Flag to check if the Refresh method has ran yet

        static void Main()
        {
            bool exit = false;
            while (!exit)
            {
                ShowOptions();
                Console.WriteLine("\nWhat do you want to do:");
                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1":
                        Refresh();
                        break;
                    case "2":
                        GenerateIndex();
                        break;
                    case "3":
                        GenerateRSS();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid entry. Select a valid option.");
                        break;
                }                
            }
        }

        static void ShowOptions()
        {
            Console.WriteLine("\n1) Refresh blog article list");
            Console.WriteLine("2) Generate blog index page");
            Console.WriteLine("3) Generate RSS XML");
            Console.WriteLine("4) Exit");
        }

        static int Refresh()
        {
            // Foreach file in data directory, load the content and generate the html from template
            DirectoryInfo di = new DirectoryInfo(Config.DataDirectory);
            int articleCount = 0;

            foreach (var file in di.GetFiles("*.xml"))
            {                
                BlogEntry b = new BlogEntry(file.ToString());
                BlogEntries.Add(b);
                b.PopulateTemplate();
                articleCount++;                           
            }
            Console.WriteLine("\tCONTENT LOADED AND WEBPAGE GENERATED FOR {0} BLOG ENTRIES", articleCount);            

            // Sort list of all blog entries with Linq
            BlogEntries = BlogEntries.OrderByDescending(x => x.Date).ToList();
            RefreshRan = true;

            return 1;
        }

        static int GenerateIndex()
        {
            // Exit if refresh method hasn't ran yet. This is because the BlogEntries list will be empty
            if (!RefreshRan)
            {
                Console.WriteLine("\tError: Blog entry refresh has not yet been ran. Unable to generate an index file without first refreshing the blog entry list...");
                return 0;
            }

            // Create lists of all blog entries grouped and sorted by year 
            int currentYear = 0;
            string indexBody = "";
            foreach (BlogEntry item in BlogEntries)
            {
                if (item.Date.Year != currentYear)
                {
                    currentYear = item.Date.Year;

                    // Only add closing </ul> if an opening tag exists, prevents adding redundant closing tag on loop entry
                    if (indexBody.Contains("<ul>"))
                        indexBody += "</ul>";

                    indexBody += "<h3>" + currentYear.ToString() + "</h3>";
                    indexBody += "<ul>";
                }

                indexBody += "<li>" +
                             "<a href=\"" + item.DatePath.Replace("\\", "/") + "/" + item.WebpageFileName + "\">" + item.Title + "</a>" +
                             "</li>\n";
            }
            indexBody += "</ul>";

            // Populate the template
            string template = File.ReadAllText(Config.BlogIndexTemplate);
            string htmlFile = template.Replace(Config.PLACEHOLDER_BODY, indexBody);

            // Create the index file 
            File.WriteAllText(Config.BlogDirectory + "/" + "index" + Config.WebpageFileType, htmlFile);

            Console.WriteLine("\tBlog index generated.");

            return 1;
        }

        static int GenerateRSS()
        {            
            // Exit if refresh method hasn't ran yet. This is because the BlogEntries list will be empty
            if (!RefreshRan)
            {
                Console.WriteLine("Error: Blog entry refresh has not yet been ran. Unable to generate an RSS XML without first refreshing the blog entry list...");
                return 0;
            }

            // Define RSS XML structure
            string rssBody = "<?xml version='1.0' encoding='UTF-8'?>\n" +
                             "<rss version='2.0'\n" +
                                "xmlns:atom=\"http://www.w3.org/2005/Atom\"\n" +
                                "xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\n" +
                             ">\n" +
                                 "<channel>\n" +
                                     "<title>" + Config.RssTitle + "</title>\n" +
                                     "<language>" + Config.RssLanguage + "</language>\n" +
                                     "<link>" + Config.RssBlogLink + "</link>\n" +
                                     "<atom:link href=\"" + Config.Website + "/" + "rss.xml" + "\" rel=\"self\" type=\"application/rss+xml\" />\n" +
                                     "<description>" + Config.RssDescription + "</description>\n" +
                                     "\n##RSS_ITEMS##\n" +
                                 "</channel>\n" +
                             "</rss>\n";

            // Generate the XML RSS item from the blog entries 
            string rssItems = "";
            foreach (BlogEntry item in BlogEntries)
            {
                rssItems += "<item>" +
                                "<title><![CDATA[" + item.Title + "]]></title>" +
                                "<link>" + Config.Website + "/" + item.DatePath.Replace("\\", "/") + "/" + item.WebpageFileName + "</link>" +
                                "<description><![CDATA[" + item.BodyContent + "]]></description>" +
                                "<guid isPermaLink=\"false\">" + Guid.NewGuid() + "</guid>" +
                                "<pubDate>" + item.Date.ToString("dd MMM yyyy HH:mm:ss") + " +0000" + "</pubDate>" +
                            "</item>";
            }

            // Add the RSS items to the RSS body 
            rssBody = rssBody.Replace("##RSS_ITEMS##", rssItems);

            // Create the RSS file in the root of the blog directory 
            File.WriteAllText(Config.BlogDirectory + "/" + "rss.xml", rssBody);

            Console.WriteLine("\tRSS XML Generated.");

            return 1;
        }
    }
}
