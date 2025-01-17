 
using Kiss.Bff.EndToEndTest.Helpers;
using Kiss.Bff.EndToEndTest.NieuwsEnWerkInstructies.Helpers;

namespace Kiss.Bff.EndToEndTest.NieuwsEnWerkInstructies;

[TestClass]
public class NieuwsEnWerkInstructiesScenarios : KissPlaywrightTest
{
    [TestMethod]
    public async Task Scenario01()
    {
        await Step("Given there is at least 1 nieuwsbericht");
        await using var news = await Page.CreateBericht(new() { Title = "Playwright test nieuwsbericht", BerichtType = BerichtType.Nieuws });

        await Step("And there is at least 1 werkinstructie");
        await using var werkbericht = await Page.CreateBericht(new() { Title = "Playwright test werkinstructie", BerichtType = BerichtType.Werkinstructie });

        await Step("When the user navigates to the HOME Page");
        await Page.GotoAsync("/");

        await Step("Then nieuwsberichten are displayed");
        await Expect(Page.GetNieuwsSection().GetByRole(AriaRole.Article).First).ToBeVisibleAsync();

        await Step("And werkinstructies are displayed");
        await Expect(Page.GetWerkinstructiesSection().GetByRole(AriaRole.Article).First).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Scenario02()
    {
        await Step("Given there is at least 1 important message");
        await using var testbericht = await Page.CreateBericht(new() { Title = "Playwright test bericht belangrijk", IsImportant = true });

        await Step("When navigates to the HOME Page");
        await Page.GotoAsync("/");

        await Step("Then the count of the important messages is displayed in the News and Instructions tabs.");
        var count = await Page.GetFeaturedCount();
        Assert.AreNotEqual(0, count);
    }

    [TestMethod]
    public async Task Scenario03()
    {
        await Step("Given there is at least 1 nieuwsbericht");
        await using var testbericht = await Page.CreateBericht(new() { Title = "Playwright test bericht", Body = "Inhoud die we gaan verbergen" });
        var article = Page.GetBerichtOnHomePage(testbericht);
        var markeerGelezenButton = article.GetByRole(AriaRole.Button).And(article.GetByTitle("Markeer als gelezen"));
        var markeerOngelezenButton = article.GetByRole(AriaRole.Button).And(article.GetByTitle("Markeer als ongelezen"));
        var body = article.GetByText(testbericht.Body!);

        await Step("When the user navigates to the HOME Page");
        await Page.GotoAsync("/");

        await Step("And clicks on the book icon within the nieuwsbericht card");
        await markeerGelezenButton.ClickAsync();

        await Step("Then the button title on hover changes to 'markeer ongelezen'");
        await Expect(markeerGelezenButton).ToBeHiddenAsync();
        await Expect(markeerOngelezenButton).ToBeVisibleAsync();

        await Step("And the body of the nieuwsbericht is hidden");
        await Expect(body).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task Scenario04()
    {
        var newsArticles = Page.GetNieuwsSection().GetByRole(AriaRole.Article);

        await Step("Given there are at least 20 nieuwsberichten");
        var berichtRequests = Enumerable.Range(1, 20)
            .Select(x => new CreateBerichtRequest
            {
                Title = "Playwright test bericht" + x
            });
        await using var berichten = await Page.CreateBerichten(berichtRequests);

        await Step("When the user navigates to the HOME Page");
        await Page.GotoAsync("/");

        var initialFirstArticleAriaSnapshot = await newsArticles.First.AriaSnapshotAsync();

        await Step("And clicks on the \"Next page\" button for the 'Nieuws' section to go to the next page");
        var nextPageButton = Page.GetNieuwsSection().GetNextPageLink();
        await nextPageButton.ClickAsync();

        await Step("Then the user should see 10 new articles on the next page");
        await Expect(newsArticles).ToHaveCountAsync(10);
        await Expect(newsArticles.First).Not.ToMatchAriaSnapshotAsync(initialFirstArticleAriaSnapshot);

        await Step("And the current page number should be 2");
        var currentPageButton = Page.GetNieuwsSection().GetCurrentPageLink();
        var page2Button = Page.GetNieuwsSection().GetByLabel("Pagina 2");
        var aButtonThatIsTheCurrentPageAndHasLabelPagina2 = currentPageButton.And(page2Button);
        await Expect(aButtonThatIsTheCurrentPageAndHasLabelPagina2).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Scenario05()
    {
        var newSection = Page.GetNieuwsSection();
        var newsArticles = Page.GetNieuwsSection().GetByRole(AriaRole.Article);

        await Step("Given there are at least 20 nieuwsberichten");
        var berichtRequests = Enumerable.Range(1, 20)
            .Select(x => new CreateBerichtRequest
            {
                Title = "Playwright test bericht" + x
            });
        await using var berichten = await Page.CreateBerichten(berichtRequests);

        await Step("And the user is on the HOME page");
        await Page.GotoAsync("/");
        await Expect(Page.GetNieuwsSection()).ToBeVisibleAsync();

        // Locate the 'Next' page button using the pagination structure
        await Step("And is on page 2 with 10 articles displayed");
        var nextPageButton = Page.GetNieuwsSection().GetNextPageLink();
        await nextPageButton.ClickAsync();
        await Expect(newsArticles).ToHaveCountAsync(10);

        var intialAriaSnapshot = await newsArticles.First.AriaSnapshotAsync();

        await Step("When the user clicks on the \"Previous\" button for the 'Nieuws' section to go to the next page");
        var previousPageButton = Page.GetNieuwsSection().GetPreviousPageLink();
        await previousPageButton.ClickAsync();

        await Step("Then the user should see 10 different articles on the first page");
        await Expect(newsArticles).ToHaveCountAsync(10);
        await Expect(newsArticles.First).Not.ToMatchAriaSnapshotAsync(intialAriaSnapshot);

        await Step("And the current page number should be 1");
        var currentPageButton = Page.GetNieuwsSection().GetCurrentPageLink();
        var page1Button = Page.GetNieuwsSection().GetByLabel("Pagina 1");
        var aButtonThatIsTheCurrentPageAndHasLabelPagina1 = currentPageButton.And(page1Button);
        await Expect(aButtonThatIsTheCurrentPageAndHasLabelPagina1).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Scenario06()
    {
        await Step("Given there are at least 20 werkinstructies");
        var berichtRequests = Enumerable.Range(1, 20)
            .Select(x => new CreateBerichtRequest
            {
                Title = "Playwright test bericht" + x,
                BerichtType = BerichtType.Werkinstructie
            });
        await using var berichten = await Page.CreateBerichten(berichtRequests);
        var articles = Page.GetWerkinstructiesSection().GetByRole(AriaRole.Article);

        await Step("And the user is on the HOME Page");
        await Page.GotoAsync("/");
        await Expect(Page.GetWerkinstructiesSection()).ToBeVisibleAsync();

        await Step("And is on page 2 with 10 werkinstructies displayed");
        var nextPageButton = Page.GetWerkinstructiesSection().GetNextPageLink();
        await nextPageButton.ClickAsync();
        await Expect(articles).ToHaveCountAsync(10);

        var intialFirstArticleAriaSnapshot = await articles.First.AriaSnapshotAsync();

        await Step("When the user clicks on the \"Previous\" button for the 'Werkinstructies' section to go to the next page");
        var previousPageButton = Page.GetWerkinstructiesSection().GetPreviousPageLink();
        await previousPageButton.ClickAsync();

        await Step("Then the user should see 10 different werkinstructies on the first page");
        await Expect(articles).ToHaveCountAsync(10);
        await Expect(articles.First).Not.ToMatchAriaSnapshotAsync(intialFirstArticleAriaSnapshot);

        await Step("And the current page number should be 1");
        var currentPageButton = Page.GetWerkinstructiesSection().GetCurrentPageLink();
        var page1Button = Page.GetWerkinstructiesSection().GetByLabel("Pagina 1");
        var aButtonThatIsTheCurrentPageAndHasLabelPagina1 = currentPageButton.And(page1Button);
        await Expect(aButtonThatIsTheCurrentPageAndHasLabelPagina1).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Scenario07()
    {
        await Step("Given there are at least 20 werkinstructies");
        var berichtRequests = Enumerable.Range(1, 20)
            .Select(x => new CreateBerichtRequest
            {
                Title = "Playwright test bericht" + x,
                BerichtType = BerichtType.Werkinstructie
            });

        await using var berichten = await Page.CreateBerichten(berichtRequests);
        var articles = Page.GetWerkinstructiesSection().GetByRole(AriaRole.Article);

        await Step("And the user is on the HOME Page");
        await Page.GotoAsync("/");
        await Expect(Page.GetWerkinstructiesSection()).ToBeVisibleAsync();

        // Locate the 'Next' page button using the pagination structure
        var nextPageButton = Page.GetWerkinstructiesSection().GetNextPageLink();
        var werkinstructies = Page.GetWerkinstructiesSection().GetByRole(AriaRole.Article);

        await Step("And is on the last page of werkinstructies");

        // keep clicking on the next page button until it's disabled
        while (!await nextPageButton.IsDisabledPageLink())
        {
            await nextPageButton.ClickAsync();
            await werkinstructies.First.WaitForAsync();
        }

        var initialFirstArticleAriaSnapshot = await articles.First.AriaSnapshotAsync();

        await Step("When the user clicks on the \"Next\" button for the ‘Werkinstructies’ section");

        await Assert.ThrowsExceptionAsync<TimeoutException>(
            () => nextPageButton.ClickAsync(new() { Timeout = 1000 }),
         "Expected the button to not be clickable, but it was");

        await Step("Then the user remains on the last page");

        await Step("And no additional werkinstructies are displayed");
        await Expect(articles.First).ToMatchAriaSnapshotAsync(initialFirstArticleAriaSnapshot);
    }

    [TestMethod]
    public async Task Scenario08()
    {
        await Step("Given there is a nieuwsbericht that is read");
        await using var bericht = await Page.CreateBericht(new() { Title = "Bericht playwright gelezen/ongelezen", Body = "Text to look for" });
        await Page.GotoAsync("/");
        var article = Page.GetBerichtOnHomePage(bericht);
        var articleBody = article.GetByText(bericht.Body);
        var markeerGelezenButton = article.GetByRole(AriaRole.Button).And(article.GetByTitle("Markeer als gelezen"));
        var markeerOngelezenButton = article.GetByRole(AriaRole.Button).And(article.GetByTitle("Markeer als ongelezen"));
        var articleHeading = article.GetByRole(AriaRole.Heading);
        await markeerGelezenButton.ClickAsync();
        await Expect(articleBody).ToBeHiddenAsync();

        await Step("And the user is on the HOME Page");
        await Page.GotoAsync("/");

        await Step("When the user clicks the 'Markeer als ongelezen' button");
        await markeerOngelezenButton.ClickAsync();

        await Step("Then content of the nieuwsbericht is visible");
        await Expect(article).ToContainTextAsync(bericht.Body);
    }

    [TestMethod]
    public async Task Scenario09()
    {
        await Step("Given there are at least two skills");
        await using var skill1 = await Page.CreateSkill(Guid.NewGuid().ToString());
        await using var skill2 = await Page.CreateSkill(Guid.NewGuid().ToString());

        await Step("And there is exactly one nieuwsbericht related to the first skill");
        await using var berichtWithSkill1 = await Page.CreateBericht(new CreateBerichtRequest { Title = Guid.NewGuid().ToString(), Skill = skill1.Naam });

        await Step("And there is exactly one nieuwsbericht related to the second skill");
        await using var berichtWithSkill2 = await Page.CreateBericht(new CreateBerichtRequest { Title = Guid.NewGuid().ToString() });

        await Step("And there is at least one nieuwsbericht without a relation to any skill");
        await using var berichtWithoutSkill = await Page.CreateBericht(new CreateBerichtRequest { Title = Guid.NewGuid().ToString(), Skill = skill2.Naam });

        await Step("And the user is on the HOME Page");
        await Page.GotoAsync("/");

        await Step("When the user selects the first skill from the filter options");
        await Page.GetSkillsSummaryElement().ClickAsync();
        await Page.GetSkillsFieldset().GetByRole(AriaRole.Checkbox, new() { Name = skill1.Naam }).CheckAsync();

        await Step("Then only the article related to the first skill is visible");
        var articles = Page.GetNieuwsSection().GetByRole(AriaRole.Article);
        await Expect(articles).ToHaveCountAsync(1);
        await Expect(articles.GetByRole(AriaRole.Heading, new() { Name = berichtWithSkill1.Title })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Scenario10()
    {
        await Step("Given there is a skill that is not linked to any article");
        await using var skill = await Page.CreateSkill(Guid.NewGuid().ToString());

        await Step("And the user is on the HOME Page");
        await Page.GotoAsync("/");

        await Step("When the user selects the skill from the filter options");
        await Page.GetSkillsSummaryElement().ClickAsync();
        await Page.GetSkillsFieldset().GetByRole(AriaRole.Checkbox, new() { Name = skill.Naam }).CheckAsync();

        await Step("Then no articles are visible");
        // wait until the spinner is gone
        await Expect(Page.Locator(".spinner")).ToBeHiddenAsync();
        var articles = Page.GetByRole(AriaRole.Article);
        await Expect(articles).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task Scenario11()
    {
        await Step("Given a unique text (uuid)");

        var uniqueTitle = Guid.NewGuid().ToString();

        await Step("Given there is exactly 1 werkinstructie with this text in the title");

        var werkbericht = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie });
       
        await Step("And there is exactly 1 nieuwsbericht with this text in the title");
        
        var nieuws = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws });

        await Step("And the user is on the HOME Page");

        await Page.GotoAsync("/");
        
        await Step("When the user selects 'Nieuws' from the filter dropdown");

        await Page.GetWerkberichtTypeSelector().SelectOptionAsync("Nieuws");

        await Step("And searches for the unique text");

        await Page.GetNieuwsAndWerkinstructiesSearch().FillAsync(uniqueTitle);
        await Page.GetNieuwsAndWerkinstructiesSearch().PressAsync("Enter");


        await Step("Then exactly one news article should be displayed");

        await Expect(Page.GetSearchResult().GetByRole(AriaRole.Article)).ToHaveCountAsync(1);

        await Step("And no work instructions should be visible");

        await Expect(Page.GetWerkinstructiesSection()).ToBeHiddenAsync();

      
    }

    [TestMethod]
    public async Task Scenario12()
    {
        await Step("Given a unique text (uuid)");

        var uniqueTitle = Guid.NewGuid().ToString();

        await Step("Given there is exactly 1 werkinstructie with this text in the title");

        await using var werkbericht = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie });

        await Step("And there is exactly 1 nieuwsbericht with this text in the title");

        await using var nieuws = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws });
       
        await Step("And the user is on the HOME Page");

        await Page.GotoAsync("/");

        await Step("When the user selects 'Werkinstructie' from the filter dropdown");
        
        await Page.GetWerkberichtTypeSelector().SelectOptionAsync("Werkinstructie");
 
        await Step("And searches for the unique text");

        await Page.GetNieuwsAndWerkinstructiesSearch().FillAsync(uniqueTitle);
        await Page.GetNieuwsAndWerkinstructiesSearch().PressAsync("Enter");

        await Step("Then exactly 1 work instruction should be displayed");

        await Expect(Page.GetSearchResult().GetByRole(AriaRole.Article)).ToHaveCountAsync(1);

        await Step("And no news articles should be visible");

        await Expect(Page.GetNieuwsSection()).ToBeHiddenAsync();

    }

    [TestMethod]
    public async Task Scenario13()
    {
        await Step("Given there are at least 3 skills");

        var skill1 = Guid.NewGuid().ToString();
        var skill2 = Guid.NewGuid().ToString();
        var skill3 = Guid.NewGuid().ToString();

        await using var skillItem1 = await Page.CreateSkill(skill1);
        await using var skillItem2 = await Page.CreateSkill(skill2);
        await using var skillItem3 = await Page.CreateSkill(skill3);

        await Step("And there is exactly one nieuwsbericht related to the first skill");

        string uniqueTitle = Guid.NewGuid().ToString();
        await using var nieuwsWithSkill1 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws, Skill = skill1 });


        await Step("And there is exactly one werkinstructie related to the first skill");
        
        uniqueTitle = Guid.NewGuid().ToString();
        await using var werkberichtWithSkill1 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie, Skill = skill1 });

        await Step("And there is exactly one nieuwsbericht related to the second skill");

        uniqueTitle = Guid.NewGuid().ToString();
        await using var nieuwsWithSkill2 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws, Skill = skill2 });

        await Step("And there is exactly one werkinstructie related to the second skill");

        uniqueTitle = Guid.NewGuid().ToString();
        await using var werkberichtWithSkill2 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie, Skill = skill2 });

        await Step("And there is exactly one nieuwsbericht related to the third skill");

        uniqueTitle = Guid.NewGuid().ToString();
        await using var nieuwsWithSkill3 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws, Skill = skill3 });

        await Step("And there is exactly one werkinstructie related to the third skill");

        uniqueTitle = Guid.NewGuid().ToString();
        await using var werkberichtWithSkill3 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie, Skill = skill3 });

        await Step("And there is at least one nieuwsbericht without a relation to any skill");

        uniqueTitle = Guid.NewGuid().ToString();
        await using var nieuwsWithSkill4 =  await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws });

        await Step("And there is at least one werkinstructie without a relation to any skill");

        uniqueTitle = Guid.NewGuid().ToString();
        await using var werkberichtWithSkill4 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie });

        await Step("And the user is on the HOME Page");

        await Page.GotoAsync("/");

        await Step("When the user selects the first skill from the filter options");

        await Page.GetSkillsSummaryElement().ClickAsync();
        await Page.GetSkillsFieldset().GetByRole(AriaRole.Checkbox, new() { Name = skill1 }).CheckAsync();


        await Step("And the user selects the second skill from the filter options");

        await Page.GetSkillsFieldset().GetByRole(AriaRole.Checkbox, new() { Name = skill2 }).CheckAsync();

        await Step("Then only the two nieuwsberichten and werkinstructies related to the first and second skill are visible");
        
        var nieuwsSection = Page.GetNieuwsSection();
        var werkinstructiesSection = Page.GetWerkinstructiesSection();

        await Expect(nieuwsSection.GetByRole(AriaRole.Article)).ToHaveCountAsync(2);
        await Expect(nieuwsSection.GetByRole(AriaRole.Heading, new() { Name = nieuwsWithSkill1.Title })).ToBeVisibleAsync();
        await Expect(nieuwsSection.GetByRole(AriaRole.Heading, new() { Name = nieuwsWithSkill2.Title })).ToBeVisibleAsync();
        await Expect(nieuwsSection.GetByRole(AriaRole.Heading, new() { Name = nieuwsWithSkill3.Title })).ToBeHiddenAsync();

        await Expect(werkinstructiesSection.GetByRole(AriaRole.Article)).ToHaveCountAsync(2);
        await Expect(werkinstructiesSection.GetByRole(AriaRole.Heading, new() { Name = werkberichtWithSkill1.Title })).ToBeVisibleAsync();
        await Expect(werkinstructiesSection.GetByRole(AriaRole.Heading, new() { Name = werkberichtWithSkill2.Title })).ToBeVisibleAsync();
        await Expect(werkinstructiesSection.GetByRole(AriaRole.Heading, new() { Name = werkberichtWithSkill3.Title })).ToBeHiddenAsync();

    }

    [TestMethod]
    public async Task Scenario14()
    {
        await Step("Given a unique text (uuid)");

        var uniqueTitle = Guid.NewGuid().ToString();

        await Step("Given there is exactly one nieuwsbericht with that text as the title");

        await using var nieuwsbericht = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws });
        
        await Step("And there is exactly one werkinstructie with that text as the title");

        await using var werkinstructie = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie });

        await Step("And the user is on the HOME Page");

        await Page.GotoAsync("/");

        await Step("When the user selects 'Alle' from the filter dropdown");

        await Page.GetWerkberichtTypeSelector().SelectOptionAsync("Alle");

        await Step("And searches for the unique text");

        await Page.GetNieuwsAndWerkinstructiesSearch().FillAsync(uniqueTitle);
        await Page.GetNieuwsAndWerkinstructiesSearch().PressAsync("Enter");

        await Step("Then exactly one nieuwsbericht and exactly one werkinstructie are visible");

        await Expect(Page.GetSearchResultFilteredByType("Werkinstructie")).ToHaveCountAsync(1);
        await Expect(Page.GetSearchResultFilteredByType("Nieuws")).ToHaveCountAsync(1);


    }

    [TestMethod]
    public async Task Scenario15()
    {
        await Step("Given a unique text (uuid)");

        var uniqueTitle = Guid.NewGuid().ToString();
        var otherText = Guid.NewGuid().ToString();

        await Step("Given there is exactly one nieuwsbericht with that text as the title");

        await using var nieuws1 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Nieuws });

        await Step("And there is exactly one werkinstructie with that text as the title");

        await using var werkinstructie1 = await Page.CreateBericht(new() { Title = uniqueTitle, BerichtType = BerichtType.Werkinstructie });

        await Step("And there is at least one nieuwsbericht without that text");

        await using var nieuws2 = await Page.CreateBericht(new() { Title = otherText, BerichtType = BerichtType.Nieuws });

        await Step("And there is at least one werkinstructie without that text");

        await using var werkinstructie2 = await Page.CreateBericht(new() { Title = otherText, BerichtType = BerichtType.Werkinstructie });

        await Step("And the user is on the HOME Page");

        await Page.GotoAsync("/");

        await Step("And has selected 'Alle' from the filter dropdown");

        await Page.GetWerkberichtTypeSelector().SelectOptionAsync("Alle");

        await Step("And has searched for the unique text");

        await Page.GetNieuwsAndWerkinstructiesSearch().FillAsync(uniqueTitle);

        await Step("When the user clicks on the close icon in the search bar");

        await Page.GetNieuwsAndWerkinstructiesSearch().ClearAsync();
     
        await Step("Then at least two werkinstructies should be visible");

        Assert.IsTrue((await Page.GetWerkinstructiesSection().GetByRole(AriaRole.Article).CountAsync()) >= 2, "at least two werkinstructies should be visible");

        await Step("And at least two nieuwsberichten should be visible");

        Assert.IsTrue((await Page.GetNieuwsSection().GetByRole(AriaRole.Article).CountAsync()) >= 2, "at least two nieuwsberichten should be visible");
        

    }
    [TestMethod]
    public void Scenario16()
    {
        Assert.Inconclusive($"This scenario seems to be a duplicate of {nameof(Scenario09)}");
    }

    [TestMethod]
    public async Task Scenario17()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        await using var nieuws = await Page.CreateBericht(new() { Title =  Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws });

        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.GotoAsync("/");

        await Step("Then the nieuwsbericht should be displayed in a list");

        var nieuwsSection = Page.GetNieuwsSection();
        await Expect(nieuwsSection.GetByRole(AriaRole.Heading, new() { Name = nieuws.Title })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Scenario18()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        var nieuws = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws });

        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.NavigateToNieuwsWerkinstructiesBeheer();

        await Step("When user clicks on the delete icon of the nieuwsbericht in the list");

        var nieuwsRow = Page.GetBeheerRowByValue(nieuws.Title);                  
             
        await Step("And confirms a pop-up window with the message ‘Weet u zeker dat u dit bericht wilt verwijderen?’");

        var deleteButton = nieuwsRow.GetByTitle("Verwijder").First;

        using (var _ = Page.AcceptAllDialogs())
        {
            await deleteButton.ClickAsync();
        }

        await Step("Then the nieuwsbericht is no longer in the list");

        var deletedRow = Page.GetBeheerRowByValue(nieuws.Title); 

        await Expect(deletedRow).ToBeHiddenAsync();


    }

    [TestMethod]
    public async Task Scenario19()
    {
        await Step("Given there is at least 1 werkinstructie");

        var werkinstructie = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Werkinstructie });

        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.NavigateToNieuwsWerkinstructiesBeheer();

        await Step("When user clicks on the delete icon of the werkinstructie in the list");

        var werkinstructieRow = Page.GetBeheerRowByValue(werkinstructie.Title);

        await Step("And confirms a pop-up window with the message ‘Weet u zeker dat u dit bericht wilt verwijderen?’");

        var deleteButton = werkinstructieRow.GetByTitle("Verwijder").First;

        using (var _ = Page.AcceptAllDialogs())
        {
            await deleteButton.ClickAsync();
        }

        await Step("Then the werkinstructie is no longer in the list");
         
        await Expect(Page.GetBeheerRowByValue(werkinstructie.Title)).ToBeHiddenAsync();

    }

    [TestMethod]
    public async Task Scenario20()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        await using var skill = await Page.CreateSkill(Guid.NewGuid().ToString());
        await using var nieuw = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws, Skill=skill.Naam , Body= Guid.NewGuid().ToString()});

        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.NavigateToNieuwsWerkinstructiesBeheer();

        await Step("When the user clicks on the arrow button of the nieuwsbericht");

        await Page.GetBeheerRowByValue(nieuw.Title).GetByRole(AriaRole.Link).ClickAsync();

        await Step("Then the Type, Titel, Inhoud, Publicatiedatum, Publicatie-einddatum and Skills of the nieuwsbericht are visible in a details screen");
        
        await Expect(Page.Locator("#titel")).ToHaveValueAsync(nieuw.Title);
        await Expect(Page.GetByText("Nieuws", new() { Exact = true })).ToBeCheckedAsync();
        await Expect(Page.Locator("label:text('Inhoud') + div")).ToContainTextAsync(nieuw.Body);
        await Expect(Page.Locator("#publicatieDatum")).ToHaveValueAsync(nieuw.PublicatieDatum.ToString("yyyy-MM-ddTHH:mm"));
        await Expect(Page.GetByLabel("Publicatie-einddatum")).ToHaveValueAsync(nieuw.PublicatieEinddatum.ToString("yyyy-MM-ddTHH:mm"));
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = skill.Naam })).ToBeCheckedAsync();
    }

    [TestMethod]
    public async Task Scenario21()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        await using var skill = await Page.CreateSkill(Guid.NewGuid().ToString());
        await using var nieuws = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws, Skill = skill.Naam });

        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.NavigateToNieuwsWerkinstructiesBeheer();

        await Step("And the user has clicked on the arrow button of the nieuwsbericht");

        await Page.GetBeheerRowByValue(nieuws.Title).GetByRole(AriaRole.Link).ClickAsync();

        await Step("And the news detail screen is displayed");

        await Expect(Page.Locator("#titel")).ToHaveValueAsync(nieuws.Title);
        await Expect(Page.GetByText("Nieuws", new() { Exact = true })).ToBeCheckedAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = skill.Naam })).ToBeCheckedAsync();

        await Step("When the user updates the title section of news");

        var updatedTitle = nieuws.Title+"_Updated";
        await Page.GetByLabel("Titel").FillAsync(updatedTitle);
         
        await Step("And clicks on the submit button");

        await Page.OnSaveBericht();

        await Step("Then the updated news title is displayed in Berichten screen");
      
        await Expect(Page.GetBeheerTableCell(1,1)).ToHaveTextAsync(updatedTitle);

        await Step("And the “Gewijzigd op” field gets updated with the latest time"); 
       
        await Expect(Page.GetBeheerTableCell(5, 1)).ToHaveTextAsync(DateTime.Now.ToString("dd-MM-yyyy, HH:mm"));
    }

    [TestMethod]
    public async Task Scenario22()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        await using var skill = await Page.CreateSkill(Guid.NewGuid().ToString());
        await using var nieuw = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws, Skill = skill.Naam });


        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.NavigateToNieuwsWerkinstructiesBeheer();

        await Step("And the user has clicked on the arrow button of the nieuwsbericht");

        await Page.GetBeheerRowByValue(nieuw.Title).GetByRole(AriaRole.Link).ClickAsync();

        await Step("And the news detail screen is displayed");

        await Expect(Page.Locator("#titel")).ToHaveValueAsync(nieuw.Title);
        await Expect(Page.GetByText("Nieuws", new() { Exact = true })).ToBeCheckedAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = skill.Naam })).ToBeCheckedAsync();

        await Step("When the user updates the Publicatiedatum section of the nieuwsbericht to a future date");
       
        var updatedPublicatieDatum = nieuw.PublicatieDatum.AddDays(30); 

        await Page.GetByLabel("Publicatiedatum").FillAsync(updatedPublicatieDatum.ToString("yyyy-MM-ddTHH:mm"));

        await Step("And clicks on the submit button");

        await Page.OnSaveBericht();

        await Step("Then the nieuwsbericht with the updated Publicatiedatum is displayed in the Berichten screen");

        await Expect(Page.GetBeheerTableCell(3, 1)).ToHaveTextAsync(updatedPublicatieDatum.ToString("dd-MM-yyyy, HH:mm"));

    }

    [TestMethod]
    public async Task Scenario23()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        await using var skill = await Page.CreateSkill(Guid.NewGuid().ToString());
        await using var nieuws = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws, Skill = skill.Naam });

        await Step("And the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Page.NavigateToNieuwsWerkinstructiesBeheer();

        await Step("And the user has clicked on the arrow button of the nieuwsbericht");

        await Page.GetBeheerRowByValue(nieuws.Title).GetByRole(AriaRole.Link).ClickAsync();

        await Step("And the news detail screen is displayed");

        await Expect(Page.Locator("#titel")).ToHaveValueAsync(nieuws.Title);
        await Expect(Page.GetByText("Nieuws", new() { Exact = true })).ToBeCheckedAsync();
        await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = skill.Naam })).ToBeCheckedAsync();


        await Step("When the user checks the ‘belangrijk’ checkbox");
        
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Belangrijk" }).CheckAsync();


        await Step("And clicks on the submit button");

        await Page.OnSaveBericht();

        await Step("And navigates to the home screen of the KISS environment");

        await Page.GotoAsync("/");

        await Step("And navigates to the page containing the nieuwsbericht selected earlier");
        await Page.GetNieuwsAndWerkinstructiesSearch().FillAsync(nieuws.Title);
        await Page.GetNieuwsAndWerkinstructiesSearch().PressAsync("Enter");

        await Step("Then the nieuwsbericht should be displayed with the ‘belangrijk’ flag");

        await Expect(Page.GetSearchResultFilteredByType("Nieuws")).ToHaveCountAsync(1);
        await Expect(Page.GetSearchResultFilteredByType("Nieuws").GetByText("Belangrijk")).ToBeVisibleAsync();

    }

    [TestMethod]
    public async Task Scenario24()
    {
        await Step("Given the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await using var nieuws = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws });

        await Step("Then the nieuwsbericht is displayed in Berichten");

        await Expect(Page.GetBeheerTableCell(1, 1)).ToHaveTextAsync(nieuws.Title);
        await Expect(Page.GetBeheerTableCell(2, 1)).ToHaveTextAsync(BerichtType.Nieuws.ToString());
    }

    [TestMethod]
    public async Task Scenario25()
    {
        await Step("Given there is at least 1 Werkinstructie");

        await using var nieuws = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Nieuws });

        await Step("And navigates to the page containing the nieuwsbericht created earlier ");

        await Page.GetBeheerRowByValue(nieuws.Title).GetByRole(AriaRole.Link).ClickAsync();

        await Step("Then the nieuwsbericht should be displayed");

        await Expect(Page.GetByRole(AriaRole.Textbox, new() { Name = "Titel" })).ToHaveValueAsync(nieuws.Title);
        await Expect(Page.GetByText("Nieuws", new() { Exact = true })).ToBeCheckedAsync(); 


    }

    [TestMethod]
    public async Task Scenario26()
    {
        await Step("Given the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await using var werkinstructie = await Page.CreateBericht(new() { Title = Guid.NewGuid().ToString(), BerichtType = BerichtType.Werkinstructie });

        await Step("Then the werkinstructie is displayed in Berichten");

        await Expect(Page.GetBeheerTableCell(1, 1)).ToHaveTextAsync(werkinstructie.Title); 
    }

    [TestMethod]
    public async Task Scenario27()
    {
        await Step("Given the user is on the Nieuws and werkinstructiesscreen available under Beheer");

        await Step("When the user clicks on the “Toevoegen” button");

        await Step("And selects Werkinstructieas ‘Type’");

        await Step("And fills in the ‘Titel’ and ‘Inhoud’ fields");

        await Step("And clicks on the submit button");

        await Step("And navigates to the page containing the werkinstructie created earlier ");

        await Step("Then the werkinstructie should be displayed");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public async Task Scenario28()
    {
        await Step("Given there is at least 1 nieuwsbericht");

        await Step("When the user navigates to the Nieuws and werkinstructiesscreen available under Beheer");

        await Step("Then there is a table titled ‘Berichten’ with rows named as “Titel”, “Type”,”publicatiedatum”, “Aangemaakt op” and “ Gewijzigd op”");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public void Scenario29()
    {
        Assert.Inconclusive("This scenario in the document is still unclear");
    }

    [TestMethod]
    public async Task Scenario30()
    {
        await Step("Given a nieuwsbericht for with a publicatiedatum in the future");

        await Step("When the user navigates to the HOME Page");

        await Step("And browses through all pages of the Nieuws section");

        await Step("Then the nieuwsbericht should not be visible");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public async Task Scenario31()
    {
        await Step("Given a nieuwsbericht for a publicatiedatum in the past");

        await Step("When the user navigates to the HOME PageAnd the user browses through all pages of the Nieuws section");

        await Step("Then the nieuwsbericht should be visible");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public async Task Scenario32()
    {
        await Step("Given a nieuwsbericht for a publicatie-einddatum in the past");

        await Step("When the user navigates to the HOME Page");

        await Step("And browses through all pages of the Nieuws section");

        await Step("Then the nieuwsbericht should not be visible");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public async Task Scenario33()
    {
        await Step("Given a nieuwsbericht for a publicatie-einddatum in the future");

        await Step("When the user navigates to the HOME Page");

        await Step("And browses through all pages of the Nieuws section");

        await Step("Then the nieuwsbericht should be visible");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public async Task Scenario34()
    {
        await Step("Given a nieuwsbericht with multiple skills");

        await Step("When the user navigates to the HOME Page");

        await Step("And browses through all pages of the Nieuws section");

        await Step("Then the nieuwsbericht should be displayed with the corresponding skills as labels");

        Assert.Inconclusive("Not implemented yet");
    }

    [TestMethod]
    public async Task Scenario35()
    {
        await Step("Given a werkinstructie with multiple skills");

        await Step("When the user navigates to the HOME Page");

        await Step("And browses through all pages of the Nieuws section");

        await Step("Then the werkinstructie should be displayed with the corresponding skills as labels");

        Assert.Inconclusive("Not implemented yet");
    }
}
