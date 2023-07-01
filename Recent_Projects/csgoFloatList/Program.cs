namespace CSGOInventoryScraper
{
    using System;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Interactions;
    using System.Globalization;
    using OpenQA.Selenium.Support.UI;
    using System.Diagnostics;

    class Program
    {
        static private string? steam_id;
        //76561197974083057
        private static dynamic? configuration;
        static readonly HttpClient client = new HttpClient();
        static private dynamic json = "";
        static private List<CSGOItem> skins = new List<CSGOItem>();
        //contains: assets && descriptions

        static async Task Main(string[] args)
        {
            //load config file
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


            //select steamID
            if (File.Exists("lastSearchedID.txt"))
            {
                bool validInput = false;
                string temp = File.ReadAllText("lastSearchedID.txt");
                while (!validInput)
                {
                    Console.WriteLine("Do you wish to continue with the last used steam ID " + temp + "? answer with y/n");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "y")
                    {
                        steam_id = temp;
                        validInput = true;
                    }
                    else if (input.ToLower() == "n")
                    {
                        Console.WriteLine("please provoide your steam id:");
                        steam_id = Console.ReadLine();
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter y or n.");
                    }
                }
            }
            else
            {
                Console.WriteLine("please provoide your steam id:");
                steam_id = Console.ReadLine();
            }

            // create directories for steamid
            if (!Directory.Exists("TempFiles/" + steam_id))
            {
                Directory.CreateDirectory("TempFiles/" + steam_id);
            }
            if (!Directory.Exists("results/" + steam_id))
            {
                Directory.CreateDirectory("results/" + steam_id);
            }

            //ensure skin data
            if (File.Exists("TempFiles/" + steam_id + "/jsonResponse.txt") || File.Exists("TempFiles/" + steam_id + "/FullSkinListJson.txt"))
            {
                bool validInput = false;
                while (!validInput)
                {
                    Console.WriteLine("Do you wish to used cached skin data? answer with y/n");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "y")
                    {
                        Console.WriteLine("please wait while your cached inventory is loaded");
                        await gatherSkinData(true);
                        validInput = true;
                    }
                    else if (input.ToLower() == "n")
                    {
                        Console.WriteLine("please wait while your inventory details is gatherd");
                        await gatherSkinData(false);
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter y or n.");
                    }
                }
            }
            else
            {
                await gatherSkinData(false);
            }

            Console.WriteLine("starting floatDB search...");
            checkeRankingInDB();
        }

        private static async Task gatherSkinData(bool useOldData)
        {
            if (File.Exists("TempFiles/" + steam_id + "/FullSkinListJson.txt") && useOldData)
            {
                Console.WriteLine("Fetching cached skin data");
                string temp = File.ReadAllText("TempFiles/" + steam_id + "/FullSkinListJson.txt");
                skins = JsonConvert.DeserializeObject<List<CSGOItem>>(temp);
            }
            else if (File.Exists("TempFiles/" + steam_id + "/jsonResponse.txt") && useOldData)
            {
                Console.WriteLine("Fetching cached json skin data, and converting to skin data");
                string temp = File.ReadAllText("TempFiles/" + steam_id + "/jsonResponse.txt");
                json = JsonConvert.DeserializeObject(temp);
                rawItemGenerator();
                skins = await requestDetailsFromDBAsync();
            }
            else
            {
                Console.WriteLine("Gathering new inventory data");
                string temp = await getJsonInventory();
                json = JsonConvert.DeserializeObject(temp);
                rawItemGenerator();
                skins = await requestDetailsFromDBAsync();
            }
        }

        private static async Task<string> getJsonInventory()
        {
            string url = $"https://steamcommunity.com/inventory/{steam_id}/730/2";
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
            try
            {
                using HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                await File.WriteAllTextAsync("TempFiles/" + steam_id + "/jsonResponse.txt", responseBody);
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return ("FAILED");
            }
        }

        private static void rawItemGenerator()
        {
            var assets = json.assets;
            var descriptions = json.descriptions;

            for (var i = 0; i < assets.Count; i++)
            {
                for (var k = 0; k < descriptions.Count; k++)
                {
                    if (assets[i].classid == descriptions[k].classid)
                    {
                        var asset = assets[i];
                        var item = descriptions[k];
                        string itemType = item.type;
                        if (item.commodity != 1 && item.marketable == 1 && !itemType.Contains("Agent"))
                        {
                            string fullItemName = item.name;
                            string endLink_id = item.market_actions[0].link;
                            Match match = Regex.Match(endLink_id, "D.(.+)$");
                            string asset_id = asset.assetid;
                            fullItemName = fullItemName.Replace("★", "");
                            fullItemName = fullItemName.TrimStart();
                            skins.Add(new CSGOItem(fullItemName, asset_id, match.Groups[1].Value));
                        }

                    }
                }
            }

        }

        private static async Task<List<CSGOItem>> requestDetailsFromDBAsync()
        {
            List<CSGOItem> refinedItems = new List<CSGOItem>();
            foreach (CSGOItem item in skins)
            {
                string url = $"https://api.csgofloat.com/?s={steam_id}&a={item.asset_id}&d={item.endLink_id}";
                try
                {
                    using HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic tempJson = JsonConvert.DeserializeObject(responseBody);
                    item.paintIndex = tempJson.iteminfo.paintindex;
                    item.defIndex = tempJson.iteminfo.defindex;
                    string floatString = tempJson.iteminfo.floatvalue;
                    decimal tempFloat = decimal.Parse(floatString, CultureInfo.InvariantCulture);
                    tempFloat = Math.Round(tempFloat, 14);
                    item.floatValue = tempFloat.ToString(CultureInfo.InvariantCulture);
                    item.minFloat = tempJson.iteminfo.min;
                    item.maxFloat = tempJson.iteminfo.max;
                    JArray temp = tempJson.iteminfo.stickers;
                    CSGOSticker[] stickers = item.stickers;
                    for (int i = 0; i < temp.Count; i++)
                    {
                        dynamic sticker = temp[i];
                        string id = sticker.stickerId;
                        string slot = sticker.slot;
                        string name = sticker.name;
                        stickers[i] = new CSGOSticker(id, slot, name);
                    }
                    item.stickers = stickers;
                    refinedItems.Add(item);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }

            }
            string saveJson = JsonConvert.SerializeObject(refinedItems);
            await File.WriteAllTextAsync("TempFiles/" + steam_id + "/FullSkinListJson.txt", saveJson);
            return refinedItems;
        }

        private static void checkeRankingInDB()
        {
            //visible for development
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArgument("--headless");
            var driver = new ChromeDriver(chromeOptions);
            Actions actions = new Actions(driver);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            //current error handeling
            int retries = 3;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                //navigate to steam
                Console.WriteLine("Opening steam and signing in ETA 6 seconds");
                driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/?goto=login");

                //Fill in the login form
                var email = driver.FindElement(By.CssSelector("input[type='text'].newlogindialog_TextInput_2eKVn"));
                email.SendKeys(decryptString(configuration["EncryptedCredentials:Username"]));

                var password = driver.FindElement(By.CssSelector("input[type='password'].newlogindialog_TextInput_2eKVn"));
                password.SendKeys(decryptString(configuration["EncryptedCredentials:Password"]));

                var acceeptTerms = driver.FindElement(By.Id("acceptAllButton"));
                acceeptTerms.Click();

                var checkbox = driver.FindElement(By.ClassName("newlogindialog_Checkbox_3tTFg"));
                checkbox.Click();

                var signin = driver.FindElement(By.ClassName("newlogindialog_SubmitButton_2QgFE"));
                signin.Click();

                Console.WriteLine("Bot steam sign in completed!");
                driver.FindElement(By.ClassName("profile_header"));

                //FloatDB login
                Console.WriteLine("Logging into float DB");
                driver.Navigate().GoToUrl("https://csgofloat.com/db");
                var element = driver.FindElement(By.CssSelector("img.ng-star-inserted"));
                element.Click();

                // steam signin button
                var confirm = driver.FindElement(By.Id("imageLogin"));
                confirm.Click();
            }
            catch (Exception)
            {
                driver.Quit();
                Console.WriteLine("Unknown steam connection error, please close and try again");
                retries = -1;
            }

            //TODO this is not TESTED
            Console.WriteLine("Confirming bot login on csgofloat");
            while (retries >= 0)
            {
                try
                {
                    var temp = driver.FindElement(By.ClassName("avatar"));
                    Console.WriteLine("Csgofloat Login confirmed");
                    //ensure we are on DB page
                    driver.Navigate().GoToUrl("https://csgofloat.com/db");
                    break;

                }
                catch (Exception)
                {
                    IWebElement snackBar = driver.FindElement(By.CssSelector("simple-snack-bar"));
                    string errorMessage = snackBar.FindElement(By.CssSelector("span")).Text;

                    if (errorMessage.Contains("20") || retries <= 0)
                    {
                        Console.WriteLine(errorMessage);
                        driver.Quit();
                        Console.WriteLine("close and try again later");
                        retries = -1;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("csgo float confirmation failed: " + errorMessage);
                        Console.WriteLine("retrying... retries left: " + retries);
                        var element1 = driver.FindElement(By.CssSelector("img.ng-star-inserted"));
                        element1.Click();
                        var confirm1 = driver.FindElement(By.Id("imageLogin"));
                        confirm1.Click();
                        retries--;
                    }
                }

            }

            //search funtionality
            if (retries >= 0)
            {
                List<Result> results = new List<Result>();
                int resume = 0;
                if (File.Exists("TempFiles/Resume_index.txt"))
                {
                    // If the file exists, read the index from it
                    resume = int.Parse(File.ReadAllText("TempFiles/Resume_index.txt"));
                    // If resuming from an index, recover previous searched items
                    results = JsonConvert.DeserializeObject<List<Result>>(File.ReadAllText("TempFiles/TempResults.json"));
                    Console.WriteLine("resuming from: " + skins[resume]);
                }
                for (int prog = resume; prog < skins.Count; prog++)
                {
                    try
                    {
                        CSGOItem skin = skins[prog];
                        (bool, decimal, decimal, string) floatInfo = floatHighOrLow(skin);
                        IWebElement skinNamefield = driver.FindElement(By.Id("mat-input-1"));
                        skinNamefield.Clear();
                        string inputname = skin.fullItemName.Replace("StatTrak™", "").Trim();
                        inputname = inputname.Replace("Souvenir", "");
                        skinNamefield.SendKeys(inputname);
                        skinNamefield.SendKeys(Keys.Enter);

                        //enter float limits
                        //lower bound
                        IWebElement lowFloat = driver.FindElement(By.Id("mat-input-3"));
                        lowFloat.Clear();
                        lowFloat.SendKeys(floatInfo.Item2.ToString());
                        lowFloat.SendKeys(Keys.Enter);
                        //upper bound 
                        IWebElement highFloat = driver.FindElement(By.Id("mat-input-4"));
                        highFloat.Clear();
                        highFloat.SendKeys(floatInfo.Item3.ToString());
                        highFloat.SendKeys(Keys.Enter);

                        //TODO make filter options for ANY, ST, souv, and normal

                        //select sorting direction
                        if (floatInfo.Item1)
                        {
                            IWebElement sortingSelector = driver.FindElement(By.Id("mat-select-4"));
                            sortingSelector.Click();
                            IWebElement selectHigh = driver.FindElement(By.Id("mat-option-5"));
                            selectHigh.Click();
                        }
                        else
                        {
                            IWebElement sortingSelector = driver.FindElement(By.Id("mat-select-4"));
                            sortingSelector.Click();
                            IWebElement selectHigh = driver.FindElement(By.Id("mat-option-4"));
                            selectHigh.Click();
                        }

                        // Click the search button 
                        IWebElement button = driver.FindElement(By.CssSelector("button.mat-raised-button.mat-primary"));
                        button.Click();
                        Console.WriteLine("searching for " + skin.fullItemName);

                        //serach function
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                        var element = wait.Until(drv => drv.FindElement(By.ClassName("mat-table")));
                        var table = driver.FindElement(By.ClassName("mat-table"));
                        var rows = table.FindElements(By.TagName("tr"));
                        int offset = 0;
                        int dups = 0;
                        decimal prevFloat = 0;
                        for (int i = 1; i < rows.Count; i++)
                        {
                            //parse float of item being searched
                            decimal itemFloat = decimal.Parse(skin.floatValue, CultureInfo.InvariantCulture);

                            //quickignore outside top 200 *suboptimal xpath value might be fragile
                            var quickignore = driver.FindElement(By.XPath("/html/body/app-root/div/div[2]/app-float-db/div/div/app-float-dbtable/div/div/table/tbody/tr[200]/td[3]"));
                            var quickfloatAndWear = quickignore.Text.Trim().Split(" ");
                            decimal endFloat = decimal.Parse(quickfloatAndWear[0], CultureInfo.InvariantCulture);

                            if (floatInfo.Item1 && itemFloat < endFloat || !floatInfo.Item1 && itemFloat > endFloat)
                            {
                                Result res = new Result("200+", skin.fullItemName, floatInfo.Item1, floatInfo.Item4, itemFloat, "null");
                                Console.WriteLine(skin.fullItemName + " is outside top 200");
                                results.Add(res);
                                break;
                            }

                            //find float value *suboptimal xpath value might be fragile
                            var floatElement = driver.FindElement(By.XPath("/html/body/app-root/div/div[2]/app-float-db/div/div/app-float-dbtable/div/div/table/tbody/tr[" + i + "]/td[3]"));
                            var floatAndWear = floatElement.Text.Trim().Split(" ");
                            decimal currFloat = decimal.Parse(floatAndWear[0], CultureInfo.InvariantCulture);
                            string condition = floatAndWear[1];
                            //find rank nr *suboptimal xpath value might be fragile
                            var ranking = driver.FindElement(By.XPath("/html/body/app-root/div/div[2]/app-float-db/div/div/app-float-dbtable/div/div/table/tbody/tr[" + i + "]/td[1]"));
                            string rankingString = ranking.Text.Replace('#', ' ');
                            rankingString = rankingString.Trim();
                            int rankingNr = Int32.Parse(rankingString);

                            //round floatinfo for correct comparisons
                            decimal lowerBound = decimal.Round(floatInfo.Item2, 14);
                            decimal upperBound = decimal.Round(floatInfo.Item3, 14);
                            if (floatInfo.Item1 && currFloat > upperBound || !floatInfo.Item1 && currFloat < lowerBound)
                            {
                                offset++;
                            }
                            else if (prevFloat == currFloat)
                            {
                                dups++;
                            }

                            string currentUrl = driver.Url;
                            if (currFloat == itemFloat)
                            {
                                rankingNr = rankingNr - offset - dups;
                                Result res = new Result(rankingNr.ToString(), skin.fullItemName, floatInfo.Item1, floatInfo.Item4, itemFloat, currentUrl);
                                Console.WriteLine("found " + res);
                                results.Add(res);
                                break;
                            }
                            else if (i == rows.Count - 1)
                            {
                                Console.WriteLine(i);
                                Result res = new Result("200+", skin.fullItemName, floatInfo.Item1, floatInfo.Item4, itemFloat, currentUrl);
                                Console.WriteLine(skin.fullItemName + " is outside top 200");
                                results.Add(res);
                            }
                            prevFloat = currFloat;
                        }
                        File.Delete("TempFiles/Resume_index.txt");
                        File.Delete("TempFiles/TempResults.json");
                        saveResult(results);
                    }
                    catch (Exception)
                    {
                        string json = JsonConvert.SerializeObject(results);
                        File.WriteAllText("TempFiles/TempResults.json", json);
                        File.WriteAllText("TempFiles/Resume_index.txt", prog.ToString());
                        saveResult(results);
                        driver.Quit();
                        Console.WriteLine("Search terminated becuase of too many searches, temporary results saved and exported, try again later to achive a full list");
                        break;
                    }
                }
                driver.Quit();
                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                Console.WriteLine("Search finished in {0:00}:{1:00}:{2:00}.{3}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            }

        }

        private static (bool, decimal, decimal, string) floatHighOrLow(CSGOItem item)
        {
            // consider simplifying searh too, will not work for skins that are not float ranked like low BF lore and can catch random skins also
            //if ( skin.floatValue[4] == '0') test = false;
            //else test = true;
            decimal f = decimal.Parse(item.floatValue, CultureInfo.InvariantCulture);
            decimal itemMin = decimal.Parse(item.minFloat, CultureInfo.InvariantCulture);
            decimal itemMax = decimal.Parse(item.maxFloat, CultureInfo.InvariantCulture);
            decimal minWear = 0;
            decimal maxWear = 1;

            if (f < 0.07m)
            {
                maxWear = 0.07m;
                if (itemMin > minWear) minWear = itemMin;
                if (itemMax < maxWear) maxWear = itemMax;
                if (Math.Abs(f - minWear) < Math.Abs(f - maxWear)) return (false, minWear, maxWear, "FN");
                else return (true, minWear, maxWear, "FN");
            }
            else if (0.07m < f && f < 0.15m)
            {
                minWear = 0.07m;
                maxWear = 0.15m;
                if (itemMin > minWear) minWear = itemMin;
                if (itemMax < maxWear) maxWear = itemMax;
                if (Math.Abs(f - minWear) < Math.Abs(f - maxWear)) return (false, minWear, maxWear, "MW");
                else return (true, minWear, maxWear, "MW");
            }
            else if (0.15m < f && f < 0.38m)
            {
                minWear = 0.15m;
                maxWear = 0.38m;
                if (itemMin > minWear) minWear = itemMin;
                if (itemMax < maxWear) maxWear = itemMax;
                if (Math.Abs(f - minWear) < Math.Abs(f - maxWear)) return (false, minWear, maxWear, "FT");
                else return (true, minWear, maxWear, "FT");
            }
            else if (0.38m < f && f < 0.45m)
            {
                minWear = 0.38m;
                maxWear = 0.45m;
                if (itemMin > minWear) minWear = itemMin;
                if (itemMax < maxWear) maxWear = itemMax;
                if (Math.Abs(f - minWear) < Math.Abs(f - maxWear)) return (false, minWear, maxWear, "WW");
                else return (true, minWear, maxWear, "WW");
            }
            else
            {
                minWear = 0.45m;
                if (itemMin > minWear) minWear = itemMin;
                if (itemMax < maxWear) maxWear = itemMax;
                if (Math.Abs(f - minWear) < Math.Abs(f - maxWear)) return (false, minWear, maxWear, "BS");
                else return (true, minWear, maxWear, "BS");
            }

        }
        private static string decryptString(String encryptedString)
        {
            byte[] encrypted = Convert.FromBase64String(encryptedString);


            using (Aes aes = Aes.Create())
            {
                byte[] input1 = Convert.FromBase64String(loadAssemblyFile("csgoFloatList.TempFiles.TempNotes.txt"));
                byte[] input2 = Convert.FromBase64String(loadAssemblyFile("csgoFloatList.TempFiles.IDHash.txt"));
                using (ICryptoTransform decryptor = aes.CreateDecryptor(input1, input2))
                {
                    byte[] plainTextBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                    return Encoding.UTF8.GetString(plainTextBytes);
                }

            }
        }
        private static String loadAssemblyFile(String name)
        {

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = name;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }

        }
        private static void saveResult(List<Result> results)
        {
            // prepare for results
            string folderPath = "results";
            Directory.CreateDirectory(folderPath);

            //save human readable result
            using (StreamWriter writer = new StreamWriter("results/" + steam_id + "/Readable_Result.txt"))
            {
                foreach (var result in results)
                {
                    string line = $"{result.Ranking} | {result.Skin} | {(result.Sorting ? "high" : "low")} | {result.Condition} | {result.Float} | {result.Url}";
                    writer.WriteLine(line);
                }
            }

            //save .csv file
            using (var writer = new StreamWriter("results/" + steam_id + "/CSV_Result.csv"))
            {
                // Write header row
                writer.WriteLine("Ranking;Skin Name;High/Low;Condition;Float;URL");
                // Write data rows
                foreach (var result in results)
                {
                    var line = $"{result.Ranking};{result.Skin};{(result.Sorting ? "high" : "low")};{result.Condition};{result.Float};{result.Url}";
                    writer.WriteLine(line);
                }
            }
        }
        class CSGOItem
        {

            //need to have for floatdb api info search
            public string fullItemName { get; }
            public string endLink_id;
            public string asset_id;

            public string? paintIndex { get; set; }
            public string? defIndex { get; set; }
            public string? floatValue { get; set; }
            public string? minFloat { get; set; }
            public string? maxFloat { get; set; }
            public CSGOSticker[] stickers = new CSGOSticker[4];

            public CSGOItem(string fullItemName, string asset_id, string endLink_id)
            {
                this.fullItemName = fullItemName;
                this.asset_id = asset_id;
                this.endLink_id = endLink_id;
            }

            public override string ToString()
            {
                return fullItemName + " - " + floatValue;
            }
        }
        class CSGOSticker
        {
            public string id;
            public string slot;
            public string name;
            public CSGOSticker(string id, string slot, string name)
            {
                this.id = id;
                this.slot = slot;
                this.name = name;
            }
        }
        class Result
        {
            public string Ranking { get; set; }
            public string Skin { get; set; }
            public bool Sorting { get; set; }
            public string Condition { get; set; }
            public decimal Float { get; set; }
            public string Url { get; set; }
            public Result(string Ranking, string Skin, bool Sorting, string Condition, decimal Float, string Url)
            {
                this.Ranking = Ranking;
                this.Skin = Skin;
                this.Sorting = Sorting;
                this.Condition = Condition;
                this.Float = Float;
                this.Url = Url;
            }

            public override string ToString()
            {
                return Skin + " #" + Ranking + (Sorting ? " high " : " low ") + Condition + " - " + Float;
            }
        }

    }
}