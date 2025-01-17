using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Configuration;


namespace Kiss.Bff.EndToEndTest
{
    /// <summary>
    /// Inherit this class in each test class. This does the following:<br/>
    /// 1. Makes sure the user is logged in before each test starts<br/>
    /// 2. Makes sure Playwright records traces for each test<br/>
    /// 3. Exposes a <see cref="Step(string)"/> method to define test steps. These show up in the Playwright traces and in the test report.<br/>
    /// 4. Builds a html test report after all tests in a test class are done.
    /// We upload these to <a href="https://klantinteractie-servicesysteem.github.io/KISS-frontend/">github pages</a>
    /// </summary>
    [TestClass]
    public class KissPlaywrightTest : PageTest
    {
        private const string StoragePath = "./auth.json";

        private static readonly IConfiguration s_configuration = new ConfigurationBuilder()
            .AddUserSecrets<KissPlaywrightTest>()
            .AddEnvironmentVariables()
            .Build();

        private static readonly UniqueOtpHelper s_uniqueOtpHelper = new(GetRequiredConfig("TestSettings:TEST_TOTP_SECRET"));
        
        // this is used to build a test report for each test
        private static readonly ConcurrentDictionary<string, string> s_testReports = [];

        private readonly List<string> _steps = [];

        /// <summary>
        /// This is run before each test
        /// </summary>
        /// <returns></returns>
        [TestInitialize]
        public virtual async Task TestInitialize()
        {
            // log in with azure ad
            var username = GetRequiredConfig("TestSettings:TEST_USERNAME");
            var password = GetRequiredConfig("TestSettings:TEST_PASSWORD");

            var loginHelper = new AzureAdLoginHelper(Page, username, password, s_uniqueOtpHelper);
            await loginHelper.LoginAsync();
            // store the cookie so we stay logged in in each test
            await Context.StorageStateAsync(new() { Path = StoragePath });

            // start tracing. we do this AFTER logging in so the password doesn't end up in the tracing
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            });
        }

        /// <summary>
        /// Start a test step. This ends up in the test report and as group in the playwright tracing
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        protected async Task Step(string description)
        {
            await Context.Tracing.GroupEndAsync();
            await Context.Tracing.GroupAsync(description);
            _steps.Add(description);
        }

        /// <summary>
        /// This is run after each test
        /// </summary>
        /// <returns></returns>
        [TestCleanup]
        public async Task TestCleanup()
        {
            // if we are in a group, end it
            await Context.Tracing.GroupEndAsync();
            var fileName = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}.zip";
            var fullPath = Path.Combine(Environment.CurrentDirectory, "playwright-traces", fileName);

            // stop tracing and save a zip file in the output directory
            await Context.Tracing.StopAsync(new()
            {
                Path = fullPath
            });

            // build a html report containing the test steps and a link to the playwright traces viewer
            var html = $"""
            <div data-outcome="{TestContext.CurrentTestOutcome}">
                <h2>{HtmlEncoder.Default.Encode(TestContext.TestName ?? "")}</h2>
                <a target="_blank" href="https://trace.playwright.dev/?trace=https://klantinteractie-servicesysteem.github.io/KISS-frontend/{fileName}">Playwright tracing</a>
                <p>Steps:</p>
                <ol>{string.Join("", _steps.Select(step => $"""
                    <li>{HtmlEncoder.Default.Encode(step)}</li>
                """))}
                </ol>
            </div>
            """;

            s_testReports.TryAdd(TestContext.TestName!, html);
        }

        /// <summary>
        /// This is run after all tests in a test class are done
        /// </summary>
        /// <returns></returns>
        [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public static async Task ClassCleanup()
        {
            // combine the reports for each test in a single html file
            var html = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src https://unpkg.com/simpledotcss@2.3.3/simple.min.css 'sha256-l0D//z1BZPnhAdIJ0lA8dsfuil0AB4xBpnOa/BhNVoU=';">
                <title>Playwright traces</title>
                <link rel="stylesheet" href="https://unpkg.com/simpledotcss@2.3.3/simple.min.css" integrity="sha384-Cxvt41nwdtHMOjpCqr+FaCybNL58LeIc0vPSLR4KlFSCBrHTb095iJbbw+hDTklQ" crossorigin="anonymous">
                <style>[data-outcome=Failed]{color: var(--code)}[data-outcome=Inconclusive]{color: var(--text-light);font-style: italic;}[data-outcome=Inconclusive] a{display:none}</style>
            </head>
            <body>
                <main>
                    {{string.Join("", s_testReports.OrderBy(x=> x.Key).Select(x=> x.Value))}}
                </main>
            </body>
            """;
            
            using var writer = File.CreateText(Path.Combine(Environment.CurrentDirectory, "playwright-traces", "index.html"));
            await writer.WriteLineAsync(html);
        }

        private static string GetRequiredConfig(string key)
        {
            var value = s_configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"'{key}' is missing from the configuration");
            }
            return value;
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            return new(base.ContextOptions())
            {
                BaseURL = GetRequiredConfig("TestSettings:TEST_BASE_URL"),
                // save auth state so we don't need to log in in every single test
                StorageStatePath = File.Exists(StoragePath) ? StoragePath : null,
            };
        }
    }
}
