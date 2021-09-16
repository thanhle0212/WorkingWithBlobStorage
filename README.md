# WorkingWithBlobStorage
Cancel changes
This project contains a MVC application that allows use to work with Azure Storage - Blob Storage by using Microsoft.WindowsAzure.Storage and Microsoft.WindowsAzure.Storage.Blob library.

Main functions:
- List all Blob Container
- Create new Blob Container
- Delete Blob Container
- List all Blob by container 
- Upload new blob 
- Delete a blob.

# NOTE

Make sure you update your connetion string to your storage account in appsettings.json as well as localsettings.json before running

"AzureStorageAccount": {
    "ConnectionString": "your-connection-string-here"
  },

 "Values": {
    "AzureWebJobsStorage": "your connection to storage account here",
  }
