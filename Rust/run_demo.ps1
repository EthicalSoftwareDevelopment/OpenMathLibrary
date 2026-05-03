# PowerShell script to launch the OpenMathLibrary Vulkan demo
# Always waits for user input before closing

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

Write-Host "Launching OpenMathLibrary Vulkan demo..."
cargo demo
$exitCode = $LASTEXITCODE

if ($Host.Name -eq 'ConsoleHost') {
    Write-Host "Press any key to exit..."
    [void][System.Console]::ReadKey($true)
} else {
    Read-Host "Press Enter to exit..."
}

exit $exitCode