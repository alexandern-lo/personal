param ($DeployEnvironment)

nuget install >$null 2>&1

$WinSCP = "WinSCP.5.9.3"
Add-Type -Path "$WinSCP\lib\WinSCPnet.dll"

$BuildDir = Resolve-Path "..\Avend.API\bin\production\netcoreapp1.0\publish\"
$WebConfig = Join-Path $BuildDir "web.config"
$RemoteRoot = "/site/wwwroot"

if ($DeployEnvironment -eq $null) {
  $DeployEnvironment='development'
}

switch ($DeployEnvironment) {
  'stage' { 
        $Confirmation = Read-Host -Prompt 'Please type ''staging'' to confirm deployment to stage server'
        if ($Confirmation -ne "staging") {
          echo "Deployment cancelled"
          exit 1;
        }

	$FtpHost="waws-prod-bay-065.ftp.azurewebsites.windows.net"
	$FtpUser="avend-stage-api\avend-deployer"
	$FtpPass="qweASD123"
  }
  'development' { 
	$FtpHost="waws-prod-bay-065.ftp.azurewebsites.windows.net"
	$FtpUser="avend-dev-api\avend-deployer"
	$FtpPass="qweASD123"
  }
  'production' {
        $Confirmation = Read-Host -Prompt 'Please type ''PROD'' to confirm deployment to production server'
        if ($Confirmation -ne "PROD") {
          echo "Deployment cancelled"
          exit 1;
        }

	$FtpHost="waws-prod-bay-065.ftp.azurewebsites.windows.net"
	$FtpUser="avend-api\avend-deployer"
	$FtpPass='qweASD123'
  }
}

if ($FtpHost -eq $Null) {
	echo "Invalid deploy environment '$DeployEnvironment'. Valid ('development', 'production', 'stage')";
	exit 1;
} else {
	echo "===================== Deploy environment '$DeployEnvironment' =====================";
}

Push-Location .
cd ..\Avend.Api\
$rev = $(git rev-list --max-count=1 HEAD)
echo $rev
echo "[assembly:System.Reflection.AssemblyInformationalVersion(""$rev"")]" | Set-Content GitSha.cs
$BuildReport = $(dotnet publish -c production)

echo "-------------"
echo "Build report:"
echo "-------------"
echo $BuildReport

$BuildResult = (Select-String -Pattern "(?sm)Published (.*) projects successfully" -AllMatches -InputObject $BuildReport).Matches.Value
Pop-Location

echo "-------------"
echo "Build result:"
echo "-------------"
echo $BuildResult

if ($BuildResult -notmatch 'Published 1/1 projects successfully') {
  echo "----------------------------"
  echo "Build is failed with result:"
  echo "----------------------------"
  echo $BuildResult
  exit 1;
}

echo "-------------------------------------------"
echo "Starting deployment to '$DeployEnvironment'"
echo "-------------------------------------------"

$ftpOptions = New-Object WinSCP.SessionOptions
$ftpOptions.ParseUrl("ftp://${FtpUser}:${FtpPass}@${FtpHost}/")
$ftp = New-Object WinSCP.Session
$ftp.ExecutablePath = ".\$WinSCP\content\winscp.exe"
$ftp.SessionLogPath = "log.txt"
function FileTransferred
{
    param($e)
    if ($e.Error -eq $Null) {
        echo "Upload of ${e.FileName} succeeded"
    } else {
        echo "-----------------------------------------"
        echo "Upload of ${e.FileName} failed ${e.Error}"
        echo "-----------------------------------------"
    }
}
$ftp.add_FileTransferred( { FileTransferred($_) } )
try {
	$ftp.Open($ftpOptions)
	echo "Remove ${RemoteRoot}/web.config to stop api service"
	try { $ftp.RemoveFiles("$RemoteRoot/web.config").Check(); } catch {  }
	echo "Sync $BuildDir with $RemoteRoot"
    $result = $ftp.SynchronizeDirectories([WinSCP.SynchronizationMode]::Remote, $BuildDir, $RemoteRoot, $True)
    $result.Check()
	echo "Upload web.config to start api service"
	$remoteFilePath = $ftp.TranslateLocalPathToRemote($WebConfig, $BuildDir, $RemoteRoot)
	$ftp.PutFiles($WebConfig, $remoteFilePath, $True).Check()
} finally {
	$ftp.Dispose()
}