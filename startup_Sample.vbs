newDir = "."
Set oShell = CreateObject ("Wscript.Shell") 
oShell.CurrentDirectory = newDir
Dim strArgs
strArgs = "cmd /c StartRandomSample.bat"
oShell.Run strArgs, 0, false