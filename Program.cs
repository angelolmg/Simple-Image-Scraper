using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace SimpleImageScrapper
{
    class Scrapper
    {
        public string GetHtmlCode(string topicSubject)
        {
            string url = "https://www.google.com/search?q=" + topicSubject + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }

            return data;
        }

        public string GetLuckyUrl(string html)
        {
            int maxImgsToGet = 1; // Máximo = ~20
            var urls = new List<string>();
            int ndx = html.IndexOf("<font size=\"-1\"></font>", StringComparison.Ordinal);
            ndx = html.IndexOf("<img", ndx, StringComparison.Ordinal);

            while (urls.Count < maxImgsToGet && ndx >= 0)
            {
                ndx = html.IndexOf("src=\"", ndx, StringComparison.Ordinal);
                ndx = ndx + 5;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("<img", ndx, StringComparison.Ordinal);
            }

            var rnd = new Random();
            int randomUrl = rnd.Next(0, urls.Count - 1);
            string luckyUrl = urls[randomUrl];

            return luckyUrl;
        }

        public byte[] GetImage(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return null;
                using (var sr = new BinaryReader(dataStream))
                {
                    byte[] bytes = sr.ReadBytes(100000);

                    return bytes;
                }
            }
        }
    }

    class Program
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static void Main(string[] args)
        {
            string sujeitosTxt = "C:\\Users\\angel\\source\\repos\\SimpleImageScraper\\subjects.txt";
            string pastaImgs = "C:\\Users\\angel\\source\\repos\\SimpleImageScraper\\images\\";
            List<string> topicList = new List<string>();

            Scrapper scp = new Scrapper();
            string[] lines = File.ReadAllLines(sujeitosTxt);

            // Alimenta lista de topicos com o arquivo .txt
            foreach (string line in lines) topicList.Add(line);

            foreach (string topic in topicList)
            {
                string html = scp.GetHtmlCode(topic);
                string luckyUrl = scp.GetLuckyUrl(html);
                byte[] image = scp.GetImage(luckyUrl);

                using (var ms = new MemoryStream(image))
                {
                    string num = RandomString(3);
                    Image pictureBox = Image.FromStream(ms);
                    pictureBox.Save(pastaImgs + topic + "_" + num + ".jpg", ImageFormat.Jpeg);
                }
            }

        }
    }
}
