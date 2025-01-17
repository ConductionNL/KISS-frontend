# End to end tests
These tests are scheduled to run on Github Actions. [An html report](https://klantinteractie-servicesysteem.github.io/KISS-frontend/) is generated on Github Pages.

## Run/debug the tests locally
1. In Visual Studio, right click the `Kiss.Bff.EndToEndTest` project and select `Manage user secrets`
1. Fill in the following values:
```jsonc
{
  "TestSettings": {
    "TEST_BASE_URL": "", // a valid base url for an environment where an instance of kiss is running
    "TEST_USERNAME": "", // a valid username to login to Azure Entra Id
    "TEST_PASSWORD": "", // a valid password to login to Azure Entra Id
    "TEST_TOTP_SECRET": "" // a secret to generate 2 Factor Authentication codes for Azure Entra Id
  }
}
```