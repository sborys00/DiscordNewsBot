# Discord News Bot
Simple news aggregator for websites of Politechika Białostocka, capable of sending updates to multiple Discord channels in real time.
It utilizes Discord Webhooks to deliver content from websites set up in the config file. 
![alt text](https://i.imgur.com/FVNiulj.png)

Example config (appsetings.json)
```json
{
  "ScanningInterval": 60000,
  "WebhookSendInterval": 2000,
  "NewsWebsites": [
    {
      "Name": "Politechnika Białostocka",
      "Url": "https://pb.edu.pl/aktualnosci/",
      "NewsUrlDirectory": "https://pb.edu.pl/",
      "Color": 74756,
      "ArticleSelector": "//article",
      "TitleSelector": ".//h3",
      "ThumbnailSelector": ".//img",
      "UrlSelector": ".//a",
      "ContentSelector": ".//a",
      "DateSelector": ".//span"
    },
    {
      "Name": "Wydział Informatyki PB",
      "Url": "https://wi.pb.edu.pl/aktualnosci/",
      "NewsUrlDirectory": "https://wi.pb.edu.pl/blog/",
      "Color": 17497,
      "ArticleSelector": "//article",
      "TitleSelector": ".//h3",
      "ThumbnailSelector": ".//img",
      "UrlSelector": ".//a",
      "ContentSelector": ".",
      "DateSelector": ".//span"
    },
    {
      "Name": "Biuro Karier i Współpracy z Absolwentami",
      "Url": "https://pb.edu.pl/biurokarier/aktualnosci/",
      "NewsUrlDirectory": "https://pb.edu.pl/biurokarier/",
      "Color": 7912513,
      "ArticleSelector": "//article",
      "TitleSelector": ".//h3",
      "ThumbnailSelector": ".//img",
      "UrlSelector": ".//a",
      "ContentSelector": ".//a",
      "DateSelector": ".//span"
    }
  ],
  "WebhookUrls": [
      "https://discord.com/api/webhooks/77583445123204457/VAv5O46Ye2vSnhFKQegm5XF6ZRZnZzwz_Ovgxgdfg345345ZVF-nAlAgQxzcvdfldeCeMAJ"
  ],
  "MemoryFileName": "archive.txt",
  "FileCleaningThreshold": 250,
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    }
  }
}
```
