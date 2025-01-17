namespace Kiss.Bff.EndToEndTest.NieuwsEnWerkInstructies.Helpers
{
    internal static class Navigation
    {
        public static async Task NavigateToNieuwsWerkinstructiesBeheer(this IPage page)
        {
            var beheerlink = page.GetByRole(AriaRole.Link, new() { Name = "Beheer" });
            var berichtenlink = page.GetByRole(AriaRole.Link, new() { Name = "Nieuws en werkinstructies" });

            await beheerlink.Or(berichtenlink).First.WaitForAsync();

            if (await beheerlink.IsVisibleAsync())
            {
                await beheerlink.ClickAsync();
            }

            await beheerlink.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            if (await berichtenlink.GetAttributeAsync("aria-current") != "page")
            {
                await berichtenlink.ClickAsync();
                await page.GetByRole(AriaRole.Heading, new() { Name = "Berichten" }).WaitForAsync();
            }
        }

        public static async Task NavigateToSkillsBeheer(this IPage page)
        {
            var beheerlink = page.GetByRole(AriaRole.Link, new() { Name = "Beheer" });
            var skillslink = page.GetByRole(AriaRole.Link, new() { Name = "Skills" });
            await beheerlink.Or(skillslink).First.WaitForAsync();

            if (await beheerlink.IsVisibleAsync())
            {
                await beheerlink.ClickAsync();
            }

            await beheerlink.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            if (await skillslink.GetAttributeAsync("aria-current") != "page")
            {
                await skillslink.ClickAsync();
                await page.GetByRole(AriaRole.Heading, new() { Name = "Skills" }).WaitForAsync();
            }
        }
    }
}
