using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Xunit;

namespace UrlShortner.AppHostTests;
public class AspireTestBase : IAsyncLifetime
{
    public DistributedApplication App { get; private set; }

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UrlShortner_AppHost>();

        App = await appHost.BuildAsync();

        await App.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
    }
}
