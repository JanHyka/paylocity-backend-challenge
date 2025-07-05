using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiTests.Infrastructure
{
    /// <summary>
    /// A custom web application factory for integration testing the API.
    /// </summary>
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestWebApplicationFactory"/> class.
        /// </summary>
        public TestWebApplicationFactory()
        {
            //task: configure the factory if needed
        }

        /// <summary>
        /// Configures the web host builder for the test server.
        /// Override this method to customize the test server configuration.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to configure.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //task: configure the web host if needed
            base.ConfigureWebHost(builder);
        }
    }
}
