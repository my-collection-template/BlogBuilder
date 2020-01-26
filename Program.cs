using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace BlogBuilder
{
    class Program
    {
        static List<BlogEntry> BlogEntries = new List<BlogEntry>();
        // Flag to check if the Refresh method has ran yet
        static bool RefreshRan = false;

        static void Main()
        {
            bool exit = false;
            while (!exit)
            {
                ShowOptions();
                Console.WriteLine("\nSelect an option:");
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

            Console.WriteLine("Exiting BlogBuilder.");
        }

        static void ShowOptions()
        {
            Console.WriteLine("\n1) Refresh blog article list");
            Console.WriteLine("2) Generate blog index page");
            Console.WriteLine("3) Generate RSS XML");
            Console.WriteLine("4) Exit");
        }

        static void Refresh()
        {
            // If refresh has already ran then empty the BlogEntries list to prevent duplications
            if (RefreshRan)
            {
                BlogEntries.Clear();
            }

            // Foreach xml file in data directory, load the content and generate the webpage from template
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
        }

        static void GenerateIndex()
        {
            // Exit if refresh method hasn't ran yet. This is because the BlogEntries list will be empty
            if (!RefreshRan)
            {
                Console.WriteLine("\tError: Blog entry refresh has not yet been ran. Unable to generate an index file without first refreshing the blog entry list...");
            }
            else
            {
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
                string webpageFile = template.Replace(Config.PLACEHOLDER_BODY, indexBody);

                // Create the index file 
                File.WriteAllText(Config.BlogDirectory + "/" + "index" + Config.WebpageFileType, webpageFile);

                Console.WriteLine("\tBLOG INDEX GENERATED.");
            }
        }

        static void GenerateRSS()
        {
            // Exit if refresh method hasn't ran yet. This is because the BlogEntries list will be empty
            if (!RefreshRan)
            {
                Console.WriteLine("Error: Blog entry refresh has not yet been ran. Unable to generate an RSS XML without first refreshing the blog entry list...");
            }
            else
            {
                string rssItems = "";

                // If an RSS XML exists then append new blog entries to the channel node
                if (File.Exists(Config.RssFilePath + "/rss.xml"))
                {
                    Console.WriteLine("\t" + "RSS XML ALREADY EXISTS.");
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Config.RssFilePath + "/rss.xml");
                    XmlNodeList rssTitles = doc.GetElementsByTagName("title");

                    // Foreach blog entry, check if its title already exists in the rss feed and add a new item if it doesn't exist
                    foreach (BlogEntry item in BlogEntries)
                    {
                        bool entryExists = false;

                        // int i intentionally set to 1 to skip the documents title tag at index 0
                        for (int i = 1; i < rssTitles.Count; i++)
                        {
                            if (rssTitles[i].InnerText == item.Title)
                            {
                                entryExists = true;                                
                            }
                        }

                        if (!entryExists)
                        {
                            // Change relative img src's to absolute URLs
                            item.BodyContent.Replace("<img src=\"", "<img src=\"" + Config.Website);

                            rssItems = "<title><![CDATA[" + item.Title + "]]></title>" +
                                            "<link>" + Config.Website + "/blog/" + item.DatePath.Replace("\\", "/") + "/" + item.WebpageFileName + "</link>" +
                                            "<description><![CDATA[" + item.BodyContent + "]]></description>" +
                                            "<guid isPermaLink=\"false\">" + Guid.NewGuid() + "</guid>" +
                                            "<pubDate>" + item.Date.ToString("dd MMM yyyy HH:mm:ss") + " +0000" + "</pubDate>";

                            // Add the new item to the existing rss feed
                            XmlElement newItem = doc.CreateElement("item");
                            newItem.InnerXml = rssItems;
                            doc.DocumentElement.SelectSingleNode("/rss/channel").AppendChild(newItem);
                            Console.WriteLine("\t'" + item.Title + "' doesn't exist so is being created.");
                        }
                    }

                    doc.Save(Config.RssFilePath + "/rss.xml");
                }

                // If an RSS XML doesn't exist then create one
                else
                {
                    Console.WriteLine("\t" + "RSS XML DOESN'T EXIST. CREATING FILE.");
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
                    foreach (BlogEntry item in BlogEntries)
                    {
                        // Change relative img src's to absolute URLs
                        item.BodyContent.Replace("<img src=\"", "<img src=\"" + Config.Website);

                        rssItems += "<item>" +
                                        "<title><![CDATA[" + item.Title + "]]></title>" +
                                        "<link>" + Config.Website + "/blog/" + item.DatePath.Replace("\\", "/") + "/" + item.WebpageFileName + "</link>" +
                                        "<description><![CDATA[" + item.BodyContent + "]]></description>" +
                                        "<guid isPermaLink=\"false\">" + Guid.NewGuid() + "</guid>" +
                                        "<pubDate>" + item.Date.ToString("dd MMM yyyy HH:mm:ss") + " +0000" + "</pubDate>" +
                                    "</item>";
                        Console.WriteLine("\t'" + item.Title + "' doesn't exist so is being created.");
                    }

                    // Add the RSS items to the RSS body 
                    rssBody = rssBody.Replace("##RSS_ITEMS##", rssItems);

                    // Create the RSS file in the root of the blog directory 
                    File.WriteAllText(Config.BlogDirectory + "/" + "rss.xml", rssBody);

                    Console.WriteLine("\tRSS XML GENERATED.");
                }
            }
        }
    }
}
