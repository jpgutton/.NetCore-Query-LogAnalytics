# .NetCore-Query-LogAnalytics
Basic Web Api project retrieving secret from KeyVault using cert authn and query Log Analytics with the secret retriever previously.

This is very basic:
  1 - use certificate to authenticate against Azure Key Vault
  2 - retrieve the password(secret) for an existing service principal
  3 - use the SP and its secret to post a simple query to Azure Log Analytics 

It can be simoplified as they are no reason to not use only the cert authn also for LA.

The code is provided as-is and is not production ready, its main use if for demonstration only
