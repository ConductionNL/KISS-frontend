namespace Kiss.Bff.EndToEndTest.Helpers
{
    internal static class Pagination
    {
        public static ILocator GetCurrentPageLink(this ILocator locator) => locator.Locator("[aria-current=page]").First;
        public static ILocator GetNextPageLink(this ILocator locator) => locator.Locator("[rel='next']").First;
        public static ILocator GetPreviousPageLink(this ILocator locator) => locator.Locator("[rel='prev']").First;
        public static async Task<bool> IsDisabledPageLink(this ILocator locator)
        {
            await locator.WaitForAsync();

            var classes = await locator.GetAttributeAsync("class");
            if (classes == null) return false;
            // when the page is disabled it doesn't get a disabled attribute.
            // TODO: research if there is a better pattern for this that is compatible with the den haag pagination component:
            // https://nl-design-system.github.io/denhaag/?path=/docs/react-navigation-pagination--docs
            // Note that their component renders invalid html: a button with a rel attribute.
            // this might serve as a source of inspiration:
            // https://design.homeoffice.gov.uk/components?name=Pagination
            // https://design-system.w3.org/components/pagination.html
            return classes.Contains("denhaag-pagination__link--disabled")
                || classes.Contains("denhaag-pagination__link--current");
        }
    }
}
