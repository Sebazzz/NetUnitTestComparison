Param([Switch]$Debug)

$assemblyFile = "ExUnitTestProjectWithOrder\bin\ExUnitTestProjectWithOrder.dll"
if ((Test-Path -Path $assemblyFile) -eq $false) {
    Write-Error -Message "Please build the project first"
    Read-Host
    Exit -1
}

$cmd = "packages\xunit.runner.console.2.1.0\tools\xunit.console.exe"
$args = @($assemblyFile, "-nologo", "-noshadow", "-verbose")

if ($Debug -eq $true) {
    $args += @("-debug")
}

& $cmd $args