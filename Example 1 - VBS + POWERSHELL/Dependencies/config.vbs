Set objShell = CreateObject("WScript.Shell")

' Show pop-up when VBS starts
MsgBox "Starting ...", vbInformation, "Info"

powershellPath = "powershell.exe"
screenshotScriptPath = "source.ps1" ' Assuming both files are in the same folder

screenshotCommand = powershellPath & " -ExecutionPolicy Bypass -File """ & screenshotScriptPath & """"

' Function to terminate PowerShell processes
Sub KillPowerShellProcesses()
    Dim objWMIService, colProcesses, objProcess
    Set objWMIService = GetObject("winmgmts:\\.\root\cimv2")
    Set colProcesses = objWMIService.ExecQuery("Select * from Win32_Process Where Name = 'powershell.exe'")

    For Each objProcess In colProcesses
        objProcess.Terminate() 
    Next
End Sub

' Kill existing PowerShell processes
KillPowerShellProcesses()

' Execute the PowerShell script hidden
objShell.Run screenshotCommand, 0, False