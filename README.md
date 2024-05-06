## Development Environment Setup

Setting Up .NET User Secrets

This project uses .NET user secrets to manage sensitive information during development. Before running the application, you will need to set up the following secrets:

Email Domain Name:
Navigate to your project directory and run the following command to set the email domain name:
    ```bash
    dotnet user-secrets set "Email:DomainName" "your-domain-name-here"

Communication Service Connection String:
Set the connection string for the communication service by executing:
    ```bash
    dotnet user-secrets set "CommunicationService:ConnectionString" "your-connection-string-here"

OpenAI API Key:
To integrate OpenAI services, store the API key as follows:

    ```bash
    dotnet user-secrets set "OpenAI:ApiKey" "your-api-key-here"

## Production Environment Setup

In the production environment, it is critical to secure sensitive information using Azure Key Vault. Follow these steps to configure the secrets in Azure Key Vault:

1. Set up Azure Key Vault:
   Ensure that you have created an Azure Key Vault and set up the appropriate access policies. Store the URI of your key vault in the `appsettings.json` under a specific configuration section to manage it more securely and flexibly across different environments. Here is how you can set it up in `appsettings.json`:
   ```json
   {
     "AzureKeyVault": {
       "Uri": "https://kv-flashcardgenerator.vault.azure.net/"
     }
   }
   
2. Add Secrets to Azure Key Vault:
Store the following secrets in Azure Key Vault with the exact names as follows:
- OpenAI API Key:
    ```plaintext
    Secret Name: OpenAi--ApiKey
    Value: [Your OpenAI API Key]
- Email Domain Name:
    ```plaintext
    Secret Name: EMail--DomainName
    Value: [Your Email Domain Name]
- Communication Service Connection String:
    ```plaintext
    Secret Name: CommunicationService--ConnectionString
    Value: [Your Connection String]
