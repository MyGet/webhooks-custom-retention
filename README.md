# Custom Retention

Web project that can perform a custom retention policy for MyGet feeds. Uses webhooks to trigger whenever a new package is added to a feed.

## Deploy and Configure

The idea of this application is to create a custom retention policy. Therefore, it is recommended to customize the logic in ```RetentionController```. Next, deploy to a web server, for example Microsoft Azure Websites.

Two settings are available as utilities, and may (or may not) be used by your custom code:

* ```Signature:NuGetFeedApiKey``` - API key for performing operations on the feed that triggers the web hook

## Setup a MyGet Webhook

For the MyGet feed you wish to run this application's logic on, configure a new HTTP Post webhook. The following options must be configured:

* **URL to POST JSON data to** - URL to the deployed application's API endpoint, for example ```http://customretention.azurewebsites.net/api/retention``` (do not forget the ```/api/retention```)
* ***Content type*** - set to ```application/json```
* ***Events that trigger this web hook*** - make sure that only ```Package Added``` is selected 

![MyGet webhook configuration](https://raw.githubusercontent.com/myget/webhooks-custom-retention/master/docs/edit-webhook.png)

From now on, all packages that are added to your feed will trigger the logic implemented in this application.

## Triggering Custom Logic on Other Events

Refer the [MyGet documentation on webhooks](http://docs.myget.org/docs/reference/webhooks) to learn about the different events that can trigger a web hook application which performs custom logic.