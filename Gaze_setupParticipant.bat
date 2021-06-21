
start /wait Post_To_ND_Server\Python\Python3/python.exe Post_To_ND_Server\Python\EyeSetup.py

MKDIR C:\projects
MKDIR C:\projects\4C_Gaze
MKDIR C:\projects\4C_Gaze\Output

XCOPY /Y 4C_config.txt C:\projects\4C_Gaze\