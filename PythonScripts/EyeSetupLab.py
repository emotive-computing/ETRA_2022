import os

print("EyeTracking Setup")

print("Please enter a participant ID:")

i = input()

j = "120"

with open ("4C_config.txt", 'w') as o:
	o.write("PID	"+ i)
	o.write("\nBinWindow	"+j+"\nRecordGaze	1\nRecordHeadPos	1\nExpName	Pilot")


