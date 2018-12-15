function SetCustomTagOnResource {
    param (
        $resourceId
    )

    process {
        Write-Verbose "Starting tags settings for resource $resourceId";
        $azureResourceInfo = Get-AzureRmResource -ResourceId $resourceId -ev resourceNotPresent -ea 0;
        #do not why, but resource retrieval fails sometimes
        if ($resourceNotPresent) {
            Write-Verbose "Could not get resource for $resourceId";
        } 
        else 
        {
            $rName = $azureResourceInfo.ResourceName;
            $rType = $azureResourceInfo.resourceType;
            $rRgName = $azureResourceInfo.ResourceGroupName;
            Write-Verbose "Settings tags for $resourceId named $rName, belonging to type $rType in resource group $rRgName";
            Set-AzureRmResource -Tag @{ billingCategory="DevProductivity"; environment="Dev"; resourceType="AzureDevOps" } -ResourceName $rName -ResourceType $rType -ResourceGroupName $rRgName -Force;
        }

        Write-Verbose "Ended tags settings"
    }
}