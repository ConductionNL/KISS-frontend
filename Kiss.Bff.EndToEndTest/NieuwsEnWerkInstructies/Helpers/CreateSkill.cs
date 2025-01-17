using Kiss.Bff.EndToEndTest.Helpers;

namespace Kiss.Bff.EndToEndTest.NieuwsEnWerkInstructies.Helpers
{
    internal static class CreateSkillExtension
    {

        private class DoOnDisposeAsync(Func<ValueTask> thingToDo) : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => thingToDo();
        }

        public static async Task<Skill> CreateSkill(this IPage page, string skillName)
        {
            // Step 1: Navigate to the "Skills" beheer page
            await page.NavigateToSkillsBeheer();

            // Step 2: Click on the "Toevoegen" button to add a new skill
            var toevoegenLink = page.GetByRole(AriaRole.Link, new() { Name = "toevoegen" });
            await toevoegenLink.ClickAsync();

            // Step 3: Fill in the skill name in the input field
            await page.GetByRole(AriaRole.Textbox, new() { Name = "Naam" }).FillAsync(skillName);

            // Step 4: Locate and click the "Opslaan" button to save the new skill
            var opslaanKnop = page.GetByRole(AriaRole.Button, new() { Name = "Opslaan" });

            // Ensure that the save button is visible and enabled before clicking
            while (await opslaanKnop.IsVisibleAsync() && await opslaanKnop.IsEnabledAsync())
            {
                await opslaanKnop.ClickAsync();
            }

            await page.GetByRole(AriaRole.Listitem).Filter(new () { HasText = skillName }).WaitForAsync();
            return new(page)
            {
                Naam = skillName
            };
        }
    }

    internal record class Skill(IPage Page) : IAsyncDisposable
    {
        public required string Naam { get; init; }

        public async ValueTask DisposeAsync()
        {
            await Page.Context.Tracing.GroupEndAsync();
            await Page.Context.Tracing.GroupAsync("Cleanup skill");
            // Step 1: Navigate to the Skills management page
            await Page.NavigateToSkillsBeheer();

            // Step 2: Locate the skill listitem by its name
            var skillLocator = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = Naam });

            // Step 3: Locate the delete button within the listitem
            var deleteButton = skillLocator.GetByRole(AriaRole.Button).And(skillLocator.GetByTitle("Verwijderen"));

            // Step 4: Click the delete button and accept the dialog
            using (var _ = Page.AcceptAllDialogs())
            {
                await deleteButton.ClickAsync();
            }

            // Step 5: Verify the skill is no longer present in the list
            await Page.GetByRole(AriaRole.Heading, new () { Name = "Skills"}).WaitForAsync();
            await skillLocator.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
            await Page.Context.Tracing.GroupEndAsync();
        }
    }
}
