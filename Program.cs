﻿using HtmlAgilityPack;                                                                   //Required library for Web Scraping functions.

var htmlAdvertisements = await GetHtml("https://www.sahibinden.com");
var data = ParseHtml(htmlAdvertisements);
int totalPrice = 0;
foreach (var item in data)
{
    var priceData = await GetHtml("https://www.sahibinden.com" + item.Url);
    item.Price = GetPriceForAdvertisement(priceData);
    totalPrice += Convert.ToInt16(item.Price);
}
CreatePriceListFile(data, totalPrice);

static Task<string> GetHtml(string uri)                                                 // Getting Html Data for scraping by using Uri.
{
    try
    {
        var client = new HttpClient();
        return client.GetStringAsync(uri);
    }    
    catch (Exception Ex)    
    {    
        Console.WriteLine(Ex.ToString());
        return null;
    }    
}

static List<Advertisement> ParseHtml(string html)                                       // Html data is parsing for getting advertisements on the main
{                                                                                       // page of the site. They are also parsed for the filling
    try                                                                                 // Advertisement DTO values by URL and Name.
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var repositories =
            htmlDoc
                .DocumentNode
                .SelectNodes("//div[@id='container']/div[3]/div/div[3]/div[3]/ul/li");  // Xpath value for the main ads on the website.

        List<Advertisement> data = new();

        foreach (var repo in repositories)
        {
            var name = repo.SelectSingleNode("a/span").InnerText;
            var url = repo.SelectSingleNode("a").Attributes["href"].Value.ToString();
            data.Add(new Advertisement()
            {
                Name = name,
                Url = url
            });
        }

        return data;
    }    
    catch (Exception Ex)    
    {    
        Console.WriteLine(Ex.ToString());
        return null;
    }    
}

static string GetPriceForAdvertisement(string html)                                     // Parsing data from advertisement main page for filling price
{                                                                                       // value in DTO.
    try    
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        return   htmlDoc
                .DocumentNode
                .SelectSingleNode("//div[@id='favoriteClassifiedPrice']").Attributes["value"].Value.ToString();
    }
    catch (Exception Ex)    
    {    
        Console.WriteLine(Ex.ToString());
        return null;
    }
}

static void CreatePriceListFile(List<Advertisement> advertisements, int totalPrice)     // Creating a file to Desktop with Name, Price and Total Price
{                                                                                       // for collected data from website.
    var fileName = @"C:\Users\PC\Desktop\SahibindenPriceList.txt";
    try    
    {
        if (File.Exists(fileName))    
        {    
            File.Delete(fileName);    
        }    
           
        using (StreamWriter sw = File.CreateText(fileName))    
        {
            foreach (var item in advertisements)
            {
                sw.WriteLine("Name: {0}, Price: {1}", item.Name, item.Price);
            }   
            sw.WriteLine("Total Price: " + totalPrice);    
        }
    }    
    catch (Exception Ex)    
    {    
        Console.WriteLine(Ex.ToString());    
    }
}