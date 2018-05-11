<# Script for powershell include var for testing: $root = 'C:\Users\borja\Downloads'#>

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