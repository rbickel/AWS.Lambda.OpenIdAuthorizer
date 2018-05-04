# AWS.Lambda.OpenIdAuthorizer
1. Deploy the Lambda from market place
2. Edit the lambda environment variables with your audience, issuer, and openidconnect endpoint

![Env variables](https://s3.eu-central-1.amazonaws.com/bickel-marketplace/Opera+Snapshot_2018-05-04_152221_console.aws.amazon.com.png)

3. Create an Authorizer for your API Gateway that reference your Lamdba
4. Precise which header to forward to the Lamdba as authorization token. Warning, this should be the full token, no "bearer ' prefix. 

![Authorizer](https://s3.eu-central-1.amazonaws.com/bickel-marketplace/Opera+Snapshot_2018-05-04_152639_ap-southeast-1.console.aws.amazon.com.png)

5. Test the authorizer by passing a valid / and invalid bearer token. A valid token should return "Effect": "Allow" while an invalid token "Effect": "Deny"

![Valid token](https://s3.eu-central-1.amazonaws.com/bickel-marketplace/Opera+Snapshot_2018-05-04_153041_ap-southeast-1.console.aws.amazon.com.png)
