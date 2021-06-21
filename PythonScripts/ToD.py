import datetime
import random
import time
import random
from subprocess import Popen
import os

hourStart = 8
hourFin = 17
samples = 5
minimumwindow = 30
lastDay = None
daycount = 0
sampleMins = 15
debugFile = os.path.dirname( __file__ ) + "\\debug2.txt"

waitMin = 5
sampleProb = 0.05


times = []

def readConfig():
    global sampleMins
    global sampleProb
    configFileName = os.path.dirname( __file__ ) + "\\sampleConfig.txt"
    with open(configFileName, 'r') as config:
        lines = config.readlines()
        line = lines[0].strip().split("\t")[1]
        sampleMins = int(lines[0].strip().split("\t")[1])  # first line is the header so just read the second line
        probability = float(lines[1].strip().split("\t")[1])
        sampleProb = probability

        print(sampleMins)
        print(sampleProb)
        

def checkCurrentTime():
	h = datetime.datetime.now().hour
	if h < hourStart or h >= hourFin:
		with open(debugFile, 'a') as out:
			out.write(str(datetime.datetime.now())  +"\t Out of Hours " + "\n")
		return False
	else:
		return True

def checkProb():
	if checkCurrentTime():
		n = random.random()
		with open(debugFile, 'a') as out:
			out.write(str(datetime.datetime.now())  +"\t Probability Generated: " + str(n) + "\n")
		if n < sampleProb:
			triggerRecord()
			time.sleep(minimumwindow * 60)

def mainProcess():
	checkProb()
	time.sleep(waitMin*60)



#Returns a new time object, that is at least 30 minutes away from existing times 
def getNewTime(t=False):
	global times
	h = datetime.datetime.now().hour
	today_random_time = datetime.datetime.now().replace(hour=random.randint(hourStart,hourFin-1),minute=random.randint(0,59))
	if (h >= hourFin):
		today_random_time = today_random_time + datetime.timedelta(days=1)
	for x in times:			
		seconds = (today_random_time-x).total_seconds()
		#print(str(x) + " " + str(today_random_time) + " "  + str(seconds) + "s " + str(seconds/60) +"minutes")
		diff = abs(seconds / 60)
		#print(diff)
		if diff < minimumwindow:
			#print("Resample")
			return(getNewTime(t))
	return today_random_time

def populateList(tomorrow = False):
	global times
	while (len(times) < samples):
		newDate = getNewTime(tomorrow)
		times.append(newDate)
	times.sort()
	with open(debugFile, 'a') as out:
		for t in times:
			out.write("Time Generated : " + str(t) + "\n")


def cycleTimes():
	global times
	global daycount
	while (len(times) > 0):
		nextT = times.pop(0)
		secs = (nextT - datetime.datetime.now()).total_seconds()
		with open(debugFile, 'a') as out:
			out.write("Next Time: " + str(nextT) + "\n")
			out.write("Time to wait : " + str(secs) + "\n")
		if(secs > 0):
			time.sleep(secs)
			triggerRecord()
			daycount = daycount + 1
			with open(debugFile, 'a') as out:
				out.write("Daily Record Count: " + str(daycount) + "\n")
		else:
			with open(debugFile, 'a') as out:
				out.write("Time Ignored: " + str(nextT) + "\n")
				

def triggerRecord():
	with open(debugFile, 'a') as out:
		out.write(str(datetime.datetime.now())  +"\t Record Triggered " + "\n")
	runBat()

def runBat():
	print("Attempting to Run bat")
	path = os.path.abspath(os.path.join(os.path.dirname( __file__ ), '../../Utilities'))
	#path = "C:/Users/sthu0966/Dropbox (Emotive Computing)/IARPA/PackageHeadless/Utilities/"
	print(path)
	p = Popen(['sample.bat', str(sampleMins)], cwd=path, shell = True)
		#time.sleep(sampleMins * 60)

#while True:
#	t = False
#	if daycount >= samples:
#		t = True
#		daycount = 0
#		with open(debugFile, 'a') as out:
#			out.write("Daily Record Count Reset ")
#			out.write("Daily Record Count: " + str(daycount) + "\n" )
#	populateList(t)
#
#	print (times)
#	cycleTimes()
#	exit()

readConfig()
while True:
	mainProcess()
