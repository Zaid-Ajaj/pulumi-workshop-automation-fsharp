# Pulumi Automation API workshop using F#

This workshop will walk you through the basics of using the [Automation API](https://www.pulumi.com/docs/guides/automation-api/) of [Pulumi](https://www.pulumi.com/) to create and deploy a Pulumi stack programmatically.

This project implements a simple web API in F# using [Falco](https://www.falcoframework.com/) with two endpoints:
 - `POST /onboard` which creates a stack for a customer and deploys an S3 bucket as a website
 - `POST /offboard` which destroys the stack for a customer

 ## Prerequisites

  - [Pulumi CLI](https://www.pulumi.com/docs/get-started/install/)
  - [.NET SDK](https://dotnet.microsoft.com/download)

## Running the project

 1. Clone this repository
 2. Run `dotnet run` in the root of the project

You will need to have a Pulumi account and have logged in to the Pulumi CLI before running the project.

You can create an access token from the Pulumi service and once created, you can run the project with that token by setting the `PULUMI_ACCESS_TOKEN` environment variable:
```
PULUMI_ACCESS_TOKEN=<your-token> dotnet run
```
## Using the API

Send a `POST` request to `http://localhost:5000/onboard` with the following JSON body:

```json
{
    "tenant": "Zaid",
    "license": "MIT"
}
```
Which will create a stack with name "Zaid" under a project "tenant-infra" and deploy an S3 bucket as a website.

Using the same request body, you can destroy the stack by sending a `POST` request to `http://localhost:5000/offboard`.