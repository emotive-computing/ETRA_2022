To install
1) Create shortcut for FaceLauncher (in bin/debug) and runPostBat (right click -> create shortcut)
2) Put both shortcuts in startup folder
	To get to startup folder: press windows key + r and type shell:startup, press enter or press start and type run and press enter (to open desktop app) then type shell:startup and press enter

Config File should contain
1) userName (blank or -1 for lab)
2) 0 or 1 indicating if should delete output file
3) # indciating device # of camera, this is ignored for lab software
4) # of lines to skip before sending output data (i.e. send 1 out of every n lines)
5) filePath where the code is (currently not used, so might get rid of this)

If either #1 or #3 is blank or -1 then initial setup to put in userID and camera will run.

To Stop Everything (i.e. stop recording) 
1) double click on DOUBLE_CLICK_TO_KILL_PROCESSES
Note that when you do this, all recording and transmission of data is stopped. Assuming you put it in the startup folder correctly, this won't restart until you restart your computer.

DOUBLE_CLICK_TO_SNOOZE_FOR_TWO_HOURS is not ready