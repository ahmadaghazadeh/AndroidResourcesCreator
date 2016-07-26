using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AndroidResourcesCreator.Properties;

namespace AndroidResourcesCreator
{
    class Project
    {
        static string url = "https://api.tinify.com/shrink";
        static int index = 0;
        public static string getImagePath()
        {
            var theDialog = new OpenFileDialog
            {
                Title = Resources.Open_Images_File,
                Filter = Resources.Click_All_files
            };
            var lastPath = (string)Settings.Default["lastPath"];
            theDialog.InitialDirectory = lastPath;
            if (theDialog.ShowDialog() != DialogResult.OK) return null;
            var directoryPath = Path.GetDirectoryName(theDialog.FileName);
            Settings.Default["lastPath"] = directoryPath;
            Settings.Default.Save();
            try
            {

                return theDialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.
                    Click_Error + ex.Message);

            }
            return null;
        }

        public static DataTable ReadCSV(string filePath)
        {
            var dt = new DataTable();
            // Creating the columns
            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            // Adding the rows
            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split(','))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));
            return dt;
        }

        public static void CompressTinyPng(string file, DataTable dbApi)
        {
            DataRow dr = dbApi.Rows[index];
            string key = dr[1].ToString();
            string input = file;
            string output = file;

            WebClient client = new WebClient();
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
            client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);
            try
            {
                client.UploadData(url, File.ReadAllBytes(input));
                /* Compression was successful, retrieve output from Location header. */
                client.DownloadFile(client.ResponseHeaders["Location"], output);
                dr[2] = client.ResponseHeaders["Compression-Count"];
                if ((Convert.ToInt32(dr[2].ToString())) == 500)
                {
                    index++;
                    if (dbApi.Rows.Count < index)
                    {
                        index = 0;
                    }
                }
            }
            catch (WebException)
            {
                /* Something went wrong! You can parse the JSON body for details. */
                Console.WriteLine("Compression failed.");
            }
        }
    }
}
