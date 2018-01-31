echo "Downloading NSIS 3.02.1..."

(New-Object Net.WebClient).DownloadFile('https://ncu.dl.sourceforge.net/project/nsis/NSIS%203/3.02.1/nsis-3.02.1-setup.exe', $env:TEMP + '\nsis-3.02.1-setup.exe')

echo "Installing NSIS 3.02.1..."

& $env:TEMP"/nsis-3.02.1-setup.exe" /S /D=$env:TEMP"\nsis"

echo "Building installer of GMBT"

& makensis gmbt.nsi

if ($LASTEXITCODE -like 0) {
	write-host "Done without errors." -foregroundcolor "green"
	$LastExitCode = 0
} else {
	write-host "Error!" -foregroundcolor "red"
	$LastExitCode = 1
}
