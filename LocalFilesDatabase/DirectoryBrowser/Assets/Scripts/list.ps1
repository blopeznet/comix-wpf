<#$root = 'C:\Users\borja\Downloads'#>
$folders = Get-ChildItem -path $root -Recurse -Filter *.cb* | Select-Object -ExpandProperty DirectoryName -Unique 

$myArray = New-Object System.Collections.ArrayList


Foreach ($FolderName in $folders)
{
	$Item = Get-ChildItem -path $FolderName -Filter *.cb* | Sort CreationTime | select -First 1             
	
	$fileArray = New-Object System.Collections.ArrayList

	get-childitem $FolderName -recurse | where {$_.extension -Like ".cb*"} | % {	
     $fileArray.Add($_.FullName + "|")
    }

    $obj = [PSCustomObject]@{
    FolderName = $FolderName
    FileName = $Item.FullName
    CreationDate = $Item.creationtime
    Count = (dir $FolderName | measure).Count
	Files = $fileArray 
    }
	$myArray.Add($obj)
	
	
}

return $myArray