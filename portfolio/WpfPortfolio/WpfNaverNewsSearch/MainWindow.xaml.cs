﻿using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace WpfNaverNewsSearch
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Commons.ShowMessageAsync("실행", "뉴스검색 실행!");
                SearchNaverNews();
            }
        }

        private void SearchNaverNews()
        {
            string keyword = txtSearch.Text; 
            string clientID = "kmb_CKONicDYwZwemVRf";
            string clientSecret = "xPDSjWAXq5";
            string openApiUri = $"https://openapi.naver.com/v1/search/news.json?start={txtStartNum.Text}&display=10&query={keyword}";
            string result;

            WebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                request = WebRequest.Create(openApiUri);
                request.Headers.Add("X-Naver-Client-Id", clientID); // 중요!
                request.Headers.Add("X-Naver-Client-Secret", clientSecret); // 중요!!

                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);

                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                stream.Close();
                response.Close();
            }

            //MessageBox.Show(result);

            var parsedJson = JObject.Parse(result); // string to json

            int total = Convert.ToInt32(parsedJson["total"]); //전체 검색결과 개수
            int display = Convert.ToInt32(parsedJson["display"]); //출력 결과 개수

            var items = parsedJson["items"];
            var json_array = (JArray)items;

            List<NewsItem> newsItems = new List<NewsItem>();

            foreach (var item in json_array)
            {
                var temp = DateTime.Parse(item["pubDate"].ToString());
                NewsItem news = new NewsItem()
                {
                    Title = Regex.Replace(item["title"].ToString(), @"<(.|\n)*?>", string.Empty),
                    OriginalLink = item["originallink"].ToString(),
                    Link = item["link"].ToString(),
                    Description = Regex.Replace(item["description"].ToString(), @"<(.|\n)*?>", string.Empty),
                    PubDate = temp.ToString("yyyy-MM-dd HH:mm")
                };
                newsItems.Add(news);
            }
            this.DataContext = newsItems;

        }

        private void dgrResult_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (dgrResult.SelectedItem == null) return; //두번째 검색부터 오류를 제거
            string link = (dgrResult.SelectedItem as NewsItem).Link;
            Process.Start(link);
        }
    }

    internal class NewsItem
    {
        public string Title { get; set; }
        public string OriginalLink { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
    }
}
