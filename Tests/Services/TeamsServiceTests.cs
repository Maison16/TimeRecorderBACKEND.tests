using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Graph;
using Microsoft.AspNetCore.Http;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Threading.Tasks;
using System;

public class TeamsServiceTests
{
    [Fact]
    public async Task SendPrivateMessageAsync_ThrowsIfAdapterNull()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Teams:BotAppId"]).Returns("bot-app-id");
        configMock.Setup(x => x["AzureAd:TenantId"]).Returns("tenant-id");

        Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
        GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

        TeamsService service = new TeamsService(httpContextAccessorMock.Object, configMock.Object, graphClient, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SendPrivateMessageAsync("userId", "msg"));
    }

    [Fact]
    public async Task SendPrivateMessageAsync_DoesNotThrow_WhenAdapterIsSet()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Teams:BotAppId"]).Returns("bot-app-id");
        configMock.Setup(x => x["AzureAd:TenantId"]).Returns("tenant-id");

        Mock<IBotFrameworkHttpAdapter> adapterMock = new Mock<IBotFrameworkHttpAdapter>();
        Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
        GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

        TeamsService service = new TeamsService(httpContextAccessorMock.Object, configMock.Object, graphClient, adapterMock.Object);

        await service.SendPrivateMessageAsync("userId", "msg");
    }

    [Fact]
    public void Constructor_ThrowsIfMissingConfig()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Teams:BotAppId"]).Returns((string)null);

        Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
        GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

        Assert.Throws<InvalidOperationException>(() =>
            new TeamsService(httpContextAccessorMock.Object, configMock.Object, graphClient, null));
    }
}