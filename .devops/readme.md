Prerequisites:

Ypu must have az cli installed.
Must be logged in to azure in your current terminal windows (use az login)


To provision infrastructure using ARM tempates:
1. Go to the .devops folder
2. Run: .\arm-deploy.ps1 -subscriptionId '<ENTER_SUBSCRIPTION_ID>' -servicesName 'ddm2021-demo' -environment 'prod'

To provision infrastructure using AZ CLI:
1. Go to the .devops folder
2. Run: .\azcli-deploy.ps1 -subscriptionId '<ENTER_SUBSCRIPTION_ID>' -servicesName 'ddm2021-demo' -environment 'prod'

To Deploy function
1. Go to the Functions project folder
2. Run:  func azure functionapp publish 'ddm2021-demo-func;

