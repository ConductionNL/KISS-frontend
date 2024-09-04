﻿using Moq;
using Microsoft.Extensions.Configuration;
using Kiss.Bff.ZaakGerichtWerken.Contactmomenten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net;
using System.Text.Json;
using RichardSzalay.MockHttp;
using Kiss.Bff.Afdelingen;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;

namespace Kiss.Bff.Test
{
    [TestClass]
    public class PostKlantContactenCustomProxyTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<GetMedewerkerIdentificatie> _getMedewerkerIdentificatieMock;
        private PostKlantContactenCustomProxy _controller;
        private DefaultHttpContext _httpContext;

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _getMedewerkerIdentificatieMock = new Mock<GetMedewerkerIdentificatie>();

            _configurationMock.Setup(config => config["KLANTCONTACTEN_BASE_URL"]).Returns("https://fakeurl.com");

            _httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, "test@example.com"),
                    new Claim(ClaimTypes.Name, "Test User")
                }))
            };
        }

        [TestMethod]
        public async Task CheckIfActorExists_ShouldReturnActorUuid_WhenActorExists()
        {
            // Arrange
            var actorUuid = "095be615-a8ad-4c33-8e9c-c7612fbf6c9f";

            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("https://fakeurl.com/api/v1/actoren?actoridentificatorObjectId=test@example.com")
                .Respond("application/json", $@"
                {{
                    ""count"": 1,
                    ""results"": [
                        {{
                            ""uuid"": ""{actorUuid}"",
                            ""url"": ""http://example.com"",
                            ""naam"": ""string"",
                            ""soortActor"": ""medewerker"",
                            ""indicatieActief"": true,
                            ""actoridentificator"": {{
                                ""objectId"": ""string"",
                                ""codeObjecttype"": ""string"",
                                ""codeRegister"": ""string"",
                                ""codeSoortObjectId"": ""string""
                            }}
                        }}
                    ]
                }}");

            var httpClient = mockHttp.ToHttpClient();

            _controller = new PostKlantContactenCustomProxy(
                _getMedewerkerIdentificatieMock.Object,
                httpClient,
                new Extern.Klantinteracties.KlantinteractiesProxyConfig("https://fakeurl.com", "secret")
            );
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.GetActorId("test@example.com");

            // Assert
            Assert.AreEqual(actorUuid, result);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task CheckIfActorExists_ShouldReturnNull_WhenActorDoesNotExist()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("https://fakeurl.com/api/v1/actoren?actoridentificatorObjectId=nonexistent@example.com")
                .Respond("application/json", @"
                {
                    ""count"": 0,
                    ""results"": []
                }");

            var httpClient = mockHttp.ToHttpClient();

            _controller = new PostKlantContactenCustomProxy(
                _getMedewerkerIdentificatieMock.Object,
                httpClient,
                new Extern.Klantinteracties.KlantinteractiesProxyConfig("https://fakeurl.com", "secret")
            );
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.GetActorId("nonexistent@example.com");

            // Assert
            Assert.IsNull(result);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task PostKlantContact_ShouldReturnKlantContactUuid_WhenSuccessful()
        {
            // Arrange
            var klantContactUuid = "095be615-a8ad-4c33-8e9c-c7612fbf6c9f";

            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, "https://fakeurl.com/api/v1/klantcontacten")
                .WithContent(@"{""nummer"":""string"",""kanaal"":""string"",""onderwerp"":""string"",""inhoud"":""string"",""indicatieContactGelukt"":true,""taal"":""str"",""vertrouwelijk"":true,""plaatsgevondenOp"":""2019-08-24T14:15:22Z""}")
                .Respond("application/json", $@"
                {{
                    ""uuid"": ""{klantContactUuid}"",
                    ""url"": ""http://example.com"",
                    ""nummer"": ""string"",
                    ""kanaal"": ""string"",
                    ""onderwerp"": ""string"",
                    ""inhoud"": ""string"",
                    ""indicatieContactGelukt"": true,
                    ""taal"": ""str"",
                    ""vertrouwelijk"": true,
                    ""plaatsgevondenOp"": ""2019-08-24T14:15:22Z""
                }}");

            var httpClient = mockHttp.ToHttpClient();

            var parsedModel = new JsonObject
            {
                ["nummer"] = "string",
                ["kanaal"] = "string",
                ["onderwerp"] = "string",
                ["inhoud"] = "string",
                ["indicatieContactGelukt"] = true,
                ["taal"] = "str",
                ["vertrouwelijk"] = true,
                ["plaatsgevondenOp"] = "2019-08-24T14:15:22Z"
            };

            _controller = new PostKlantContactenCustomProxy(
                _getMedewerkerIdentificatieMock.Object,
                httpClient,
                new Extern.Klantinteracties.KlantinteractiesProxyConfig("https://fakeurl.com", "secret")
            );
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.PostKlantContact(parsedModel);
            var resultUuid = result?["uuid"]?.ToString(); // Extract uuid as string

            // Assert
            Assert.AreEqual(klantContactUuid, resultUuid);

            mockHttp.VerifyNoOutstandingExpectation();
        }


        [TestMethod]
        public async Task PostActoren_ShouldReturnCreatedActorUuid_WhenSuccessful()
        {
            // Arrange
            var actorUuid = "095be615-a8ad-4c33-8e9c-c7612fbf6c9f";

            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, "https://fakeurl.com/api/v1/actoren")
                .Respond("application/json", $@"
                {{
                    ""uuid"": ""{actorUuid}"",
                    ""url"": ""http://example.com"",
                    ""naam"": ""string"",
                    ""soortActor"": ""medewerker"",
                    ""indicatieActief"": true,
                    ""actoridentificator"": {{
                        ""objectId"": ""string"",
                        ""codeObjecttype"": ""mdw"",
                        ""codeRegister"": ""msei"",
                        ""codeSoortObjectId"": ""email""
                    }}
                }}");

            var httpClient = mockHttp.ToHttpClient();

            _controller = new PostKlantContactenCustomProxy(
                _getMedewerkerIdentificatieMock.Object,
                httpClient,
                new Extern.Klantinteracties.KlantinteractiesProxyConfig("https://fakeurl.com", "secret")
            );
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.PostActoren();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var jsonResponse = JsonDocument.Parse(okResult.Value.ToString());
            var returnedUuid = jsonResponse.RootElement.GetProperty("uuid").GetString();

            Assert.AreEqual(actorUuid, returnedUuid);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public void LinkActorWithKlantContact_ShouldReturnOkResult_WhenLinkedSuccessfully()
        {
            // Arrange
            var actorUuid = "actor-uuid";
            var klantcontactUuid = "klantcontact-uuid";

            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, "https://fakeurl.com/api/v1/actorklantcontacten")
                .Respond("application/json", $@"
                {{
                    ""uuid"": ""link-uuid"",
                    ""url"": ""http://example.com"",
                    ""actor"": {{
                        ""uuid"": ""{actorUuid}"",
                        ""url"": ""http://example.com""
                    }},
                    ""klantcontact"": {{
                        ""uuid"": ""{klantcontactUuid}"",
                        ""url"": ""http://example.com""
                    }}
                }}");

            var httpClient = mockHttp.ToHttpClient();

            _controller = new PostKlantContactenCustomProxy(
                _getMedewerkerIdentificatieMock.Object,
                httpClient,
                new Extern.Klantinteracties.KlantinteractiesProxyConfig("https://fakeurl.com", "secret")
            );
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = _controller.LinkActorWithKlantContact(actorUuid, klantcontactUuid);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ProxyResult));

            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
