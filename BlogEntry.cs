﻿using System;
using System.IO;
using System.Xml;

namespace BlogBuilder
{
    class BlogEntry
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string BodyContent { get; set; }
        public string DataPath { get; set; } // Path to the data file
        public string DatePath { get; set; } // Date path to the blog item, i.e. BlogDirectory/year/month/date/
        public string WebpageFileName { get; set; } // File name of the webpage file

        public BlogEntry(string path)
        {
            DataPath = path;
            StoreContent();
        }
        
        private void StoreContent()
        {
            // Store values from the XML data file
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(DataPath);
                Title = doc.DocumentElement.SelectSingleNode("/BlogEntry/Title").InnerText;
                Date = DateTime.Parse(doc.DocumentElement.SelectSingleNode("/BlogEntry/Date").InnerText);
                BodyContent = doc.DocumentElement.SelectSingleNode("/BlogEntry/BodyContent").InnerText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an issue reading the XML data file at: " + DataPath + ". Make sure your data file is formatted correctly.");
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }

        public void PopulateTemplate()
        {
            // Populate the merge fields of the template file with the data values
            string template = File.ReadAllText(Config.BlogEntryTemplate);
            string webpageFile = template.Replace(Config.PLACEHOLDER_TITLE, Title)
                                      .Replace(Config.PLACEHOLDER_DATE, Date.ToString("dd-MM-yyyy"))
                                      .Replace(Config.PLACEHOLDER_BODY, BodyContent);

            // Check if date-formated folder structure exists and create if needed
            DatePath = Date.Year + "/" + Date.Month + "/" + Date.Day;  
            Directory.CreateDirectory(Config.BlogDirectory + "/" + DatePath);

            // Write the webpage file, create if doesn't exist and overwrite if it does
            WebpageFileName = Title.ToLower().Trim().Replace(" ", "-").Replace("/", "-").Replace("\\", "-").Replace(":", "").Replace("&", "-").Replace("$", "") + Config.WebpageFileType;
            File.WriteAllText(Config.BlogDirectory + "/" + DatePath + "/" + WebpageFileName, webpageFile);
        }
    }
}
