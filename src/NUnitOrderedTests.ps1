Param([Switch]$Debug)

$assemblyDir = "NetUnitTestProjectWithOrder\bin"
$assemblyFile = "NetUnitTestProjectWithOrder.dll"
$assemblyFilePath = "$assemblyDir\$assemblyFile"
$packageDir = "packages\NUnit.ConsoleRunner.3.2.1\tools"
if ((Test-Path -Path $assemblyFilePath) -eq $false) {
    Write-Error -Message "Please build the project first"
    Read-Host
    Exit -1
}

Copy-Item -Path "$assemblyDir\*" -Destination "packages\NUnit.ConsoleRunner.3.2.1\tools\" -Force
Push-Location "$packageDir\"

$cmd = "$packageDir\nunit3-console.exe"
$args = @($assemblyFile, "--noheader", "--verbose", "--process=InProcess", "--domain=None")

if ($Debug -eq $true) {
    $args += @("--debug", "--pause")
}

& $cmd $args
Pop-Location