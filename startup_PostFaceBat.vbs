newDir = "Face"
Set oShell = CreateObject ("Wscript.Shell") 
oShell.CurrentDirectory = newDir
Dim strArgs
strArgs = "cmd /c postData.bat"
oShell.Run strArgs, 0, false