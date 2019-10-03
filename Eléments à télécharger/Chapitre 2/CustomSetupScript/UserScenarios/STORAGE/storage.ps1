# Create a storage context
$ctx = New-AzureStorageContext sawinarkstorage rSusxBXPNoeKuzjCZG5H7MsdPos+zhuULIryc/tYKE8JeI8DIJPxlAm4o58owbeyMMZchKnjbK1ZGxoF7IaDxA==

# Create a container
$containerName = "newcontainer"
New-AzureStorageContainer $containerName -Context $ctx -Permission blob

# Delete a container
#Remove-AzureStorageContainer -Container $containerName -Context $ctx