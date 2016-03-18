# RedditRankedFlairs

The official repository for [the site](http://redditrankedflairs.azurewebsites.net) used by [/r/summonerschool](https://reddit.com/r/summonerschool) 
to add verified ranked flairs to contributors.

**Configuration**

All configuration is performed using the Web.Config. For security reasons, the actual Web.Config is not included. [Example web.config](http://pastebin.com/eJWSYTRY). *Don't forget to update your Machine Key.*

App Settings Reference:

* `Hangfire.disabled` (bool) - When true, the Hangfire.io server is not started.

* `reddit.auth.clientId` (string) - The OAuth ClientId for the 'web app'. (see below)

* `reddit.auth.clientSecret` (string) - The OAuth ClientSecret for the 'web app'.

* `reddit.script.clientId` (string) - The OAuth ClientId for the 'personal use script'.

* `reddit.script.clientSecret` (string) - The OAuth ClientSecret for the 'personal use script'.

* `reddit.modUserName` (string) - The Reddit UserName for the Flair Mod account.

* `reddit.modPassword` (string) - The Reddit Password for the Flair Mod account.

* `riot.apiKey` (string) - The API key given by Riot for use with their public API.

* `riot.maxAttempts` (int) - The maximum number of retries when a problem is encountered with Riot's API.

* `riot.retryInterval` (System.TimeSpan) - The interval to wait after a problem before retrying.

* `riot.maxRequestsPer10Seconds` (int) - The maximum number of requests per 10 second period before hitting the rate limiter.

* `website.leagueUpdateStaleTime` (System.TimeSpan) - The duration after which a player's flair is considered stale and requires update.

[Configuring OAuth](https://www.reddit.com/prefs/apps/): Add a 'web app' and a 'personal use script'.

## What's this all about?

[League of Legends](https://www.leagueoflegends.com) is the [astonishingly popular](https://www.google.ca/url?sa=t&rct=j&q=&esrc=s&source=web&cd=3&cad=rja&uact=8&ved=0ahUKEwjthcqmzsrLAhVDmYMKHa86CVEQFggmMAI&url=http%3A%2F%2Fwww.forbes.com%2Fsites%2Finsertcoin%2F2014%2F01%2F27%2Friots-league-of-legends-reveals-astonishing-27-million-daily-players-67-million-monthly%2F&usg=AFQjCNHMpPx45j6T40Fs9F6DvhkAP1JUng&sig2=abwf3efOnI3xx15Wvinxzg) action strategy game by Riot Games.
Over the course of year long sprints, or "seasons", more than 60 million players worldwide compete to achieve standings on a competitive ladder.
Those who successfully climb the ladder are awarded ranks: Bronze, Silver, Gold, Platinum, Diamond, Master, and Challenger.

[Summoner School](https://reddit.com/r/summonerschool) is an online community of players who have come together to help one another be better players.

Every day, thousands of players come together to share information on Summoner School. In order for the information given there to be vetted correctly, it's sometimes
important to know the rank of the person giving the information. A "Silver" player may not be wrong, but players might want to prefer the opinion of a "Diamond" or "Master" instead.

**Troubled Beginnings**

![Image of Flair](http://i.imgur.com/k5PDjdg.png)

> *A reddit user with a ranked flair.*

The [moderator team](https://www.reddit.com/r/summonerschool/about/moderators) at Summoner School made several attempts to highlight the contributions of high-calibre players.
Some of these included the use of Reddit's "flair" system. Flairs are badges that appear next to a person's name in all of their contributions.

Unfortunately, previous attempts at solving the flair system suffered from dishonesty. Contributors were cheating and finding ways
to obtain flairs which they hadn't earned. Meanwhile, personally validating each player was too time consuming.

The flair system was discontinued.

**Systematic verification**

In the summer of 2015, Summoner School's founder put out a call for somebody to try to solve the flair system's problem: verifying contributors in a systematic way.

This application is the result of that effort.

## How it works

Reddit users visit the site and are prompted to sign in using their Reddit account. Reddit's OAuth 2.0 protocol allows the site to verify that 
their Reddit account is authentic.

After signing in, the visitor can specify one or more League of Legends accounts that they own. To verify ownership of those accounts, a challenge is made:

* The user must sign in to League of Legends and change an in-game field to a "validation code".

* Using Riot Games' [Developer API](https://developer.riotgames.com/) the field is retrieved and checked for authenticity.

Once these steps have been completed, the user is considered registered. From time to time, the user's rank is retrieved using the Riot API and
a flair is composed and sent using Reddit's OAuth 2.0 API.

## Technical Info

The solution was made using **Microsoft Visual Studio Community 2015** and targets **.NET Framework 4.5.2**. 

The web application project is built on **ASP.NET MVC 5**. Pages are rendered using the **Razor View Engine**. 
The majority of the website is a single HTML file which behaves like an app using **Angular.JS**.

Data Persistence is handled using **Entity Framework 6 (Code First)**. The production site uses the **System.Data.SqlClient** data provider and is backed
by **Microsoft SQL Server 2012**.

The website is hosted on the cloud using **Microsoft Azure App Services**.

## Contributions

* The solution uses a trimmed down branch of [OWIN OAuth Providers](https://github.com/RockstarLabs/OwinOAuthProviders). 
I dropped all the providers except Reddit. Slight changes were made to the Reddit provider for support of features which didn't end up being used.

The following NuGet packages are included:

* **[Autofac](http://autofac.org/)** - IoC container and dependency injection.

* **[dotless](http://www.dotlesscss.org/)** - LESS to CSS converter.

* **[Hangfire](http://hangfire.io)** - Scheduling server for ASP.NET applications.

* **[Hangfire.Autofac](https://github.com/HangfireIO/Hangfire.Autofac)** - Job activator for Autofac.

* **[JSON.NET](http://www.newtonsoft.com/json)** - JSON framework for .NET.
