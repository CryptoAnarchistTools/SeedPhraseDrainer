Add-Type -AssemblyName System.Windows.Forms

# Show pop-up when script starts
[System.Windows.Forms.MessageBox]::Show("Started", "Info", [System.Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Information)

# Generate a random file name (using a short random string)
$randomName = -join ((65..90) + (97..122) | Get-Random -Count 8 | % {[char]$_}) # Ex.: "KjPxLmNs"
$logFilePath = "$env:TEMP\$randomName.txt"

# Function to log clipboard content
function Log-Clipboard {
    param (
        [string]$content
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "$timestamp - $content"
    Add-Content -Path $logFilePath -Value $logEntry
}

# Variable to store the previous clipboard content
$previousContent = ""

# Infinite loop to monitor clipboard
while ($true) {
    $currentContent = Get-Clipboard -ErrorAction SilentlyContinue

    if ($currentContent -and $currentContent -ne $previousContent) {
        Log-Clipboard $currentContent
        $previousContent = $currentContent
    }

    Start-Sleep -Milliseconds 500
}