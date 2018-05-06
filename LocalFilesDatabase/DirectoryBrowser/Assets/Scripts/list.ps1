<#$root = 'C:\Users\borja\Downloads'#>
<#$folders = Get-ChildItem -path $root -Recurse -Filter *.cb* | Select-Object -ExpandProperty DirectoryName -Unique 

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

return $myArray#>

$folders = Get-ChildItem -path $root -Recurse -Filter *.cb* | Select-Object -ExpandProperty DirectoryName -Unique 

$myArray = New-Object System.Collections.ArrayList


Foreach ($directory in $folders)
{
    $files = get-childitem $directory -recurse | where {$_.extension -Like ".cb*"}
	$Item = $files | Sort CreationTime | select -First 1             
    $Last = $files | Sort CreationTime | select -Last 1
	
	$fileArray = New-Object System.Collections.ArrayList
    

	foreach($_  in $files)  {	
     $fileArray.Add($_.FullName + "|")    
    }

    $obj = [PSCustomObject]@{
    FolderName = $directory
    FileNameFirst = $Item.FullName
    FileNameLast = $Last.FullName
    CreationDate = $Item.creationtime
    LastUpdate = $Last.creationtime
    Count = $fileArray.Count
	Files = $fileArray 
    TotalSize = ($files | Measure-Object -Sum Length).Sum / 1MB.ToString("#.##")
    }
	$myArray.Add($obj)
	
	
}

return $myArray