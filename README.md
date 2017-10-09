# gnatta.facebook
Tool to help during facebook "conversations" webhook downtime

#### Basic usage
<pre>Gnatta.Facebook.WebhookBackup.exe conv <b>&lt;fb_access_token&gt;</b></pre>
This will output any payloads generated to the console, this will show the json webhook payload that would be delivered. Updates shown will be from the last 15 minutes. This will happen for 1 minute until the application terminates.

#### Options
--start-time       ISO 8601 Time to start checking from, will send information for any conversation updated after this time, not before. (Default: 15 minutes ago)
--end-time         ISO 8601 Time to stop checking, will stop polling at this time. (Default: 1 minute in the future, then the application will terminate)
--endpoint         Endpoint to send webhooks to. (Default: console output)
--secret           Facebook app secret of the app to sign requests. (Default: requests will not be signed)
--poll-interval    (Default: 30) Poll interval in seconds. (Default: 30)
--fb-page-size     (Default: 10) Page size during poll. (Default: 10)

#### Expected usage
<pre>
Gnatta.Facebook.WebhookBackup.exe conv <b>&lt;fb_access_token&gt;</b> 
    --end-time <b>&lt;time to stop processing&gt;</b>
    --endpont <b>&lt;your configured endpoint with FB&gt;</b>
    --secret <b>&lt;your application secret for signing&gt;</b>
</pre>
