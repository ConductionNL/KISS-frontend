namespace Kiss.Bff.EndToEndTest.NieuwsEnWerkInstructies.Helpers
{
    internal static class Locators
    {
        public static ILocator GetNieuwsSection(this IPage page) => page.Locator("section").Filter(new() { Has = page.GetByRole(AriaRole.Heading, new() { Name = "Nieuws" }) });
        public static ILocator GetWerkinstructiesSection(this IPage page) => page.Locator("section").Filter(new() { Has = page.GetByRole(AriaRole.Heading, new() { Name = "Werkinstructies" }) });
        public static ILocator GetBerichtOnHomePage(this IPage page, Bericht bericht) => page.GetByRole(AriaRole.Article).Filter(new() { Has = page.GetByRole(AriaRole.Heading, new() { Name = bericht.Title }) });
        public static ILocator GetSummaryElement(this IPage page) => page.Locator("summary");
        public static ILocator GetSkillsSummaryElement(this IPage page) => page.GetSummaryElement().Filter(new() { HasText = "Filter op categorie" });
        public static ILocator GetSkillsFieldset(this IPage page) => page.GetByRole(AriaRole.Group, new() { Name = "Filter op categorie" });
        public static ILocator GetWerkberichtTypeSelector(this IPage page) => page.Locator($"#werkbericht-type-input");
        public static ILocator GetNieuwsAndWerkinstructiesSearch(this IPage page) => page.Locator($"#search-input"); 
       
        public static ILocator GetSearchResult(this IPage page) => page.Locator("section").Filter(new() { Has = page.GetByRole(AriaRole.Heading, new() { Name = "Zoekresultaten" }) });
        public static ILocator GetSearchResultFilteredByType(this IPage page,string type) => page.GetSearchResult().GetByRole(AriaRole.Article).Filter(new() { Has = page.Locator("small", new() { HasText = type }) });

        public static ILocator GetBeheerRowByValue(this IPage page, string title) => page.GetByRole(AriaRole.Row)
                      .Filter(new()
                      {
                          Has = page.GetByRole(AriaRole.Cell, new() { Name = title, Exact = true }).First
                      });
        public static ILocator GetBeheerTableCell(this IPage page, int col, int row) =>
                     page.Locator($"table tbody tr:nth-child({row}) td:nth-child({col})");

    }
}
