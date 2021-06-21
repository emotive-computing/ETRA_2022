import datetime
import os
import sys
import time
import requests
import urllib3
import asyncio
import calendar
import sys

# helpers
# https://docs.python.org/3/library/asyncio-task.html#asyncio.Future.set_result
# http://www.giantflyingsaucer.com/blog/?p=5557

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
ACCESS_TOKEN_FACE = 'token'
ACCESS_TOKEN_POSE = 'token'
ACCESS_TOKEN_OPENFACE_GAZE = 'token'
ACCESS_TOKEN_VALIDITY = 'token'

url = 'https://url.edu'
headers_face = {"Authorization": "Token "+ACCESS_TOKEN_FACE}
headers_pose = {"Authorization": "Token "+ACCESS_TOKEN_POSE}
headers_openface_gaze = {"Authorization": "Token "+ACCESS_TOKEN_OPENFACE_GAZE}
headers_validity = {"Authorization": "Token "+ACCESS_TOKEN_VALIDITY}

def readConfig(filePath):
	configFileName = os.path.normpath(os.path.join(filePath, "Face_Config.txt"))
	with open(configFileName, 'r') as config:
		lines = config.readlines()
		line = lines[1].split(",") # first line is the header so just read the second line
		if (len(line) != 4):
			# config file is wrong so return reasonable defaults (don't delete, send every line)
			return (0, 1)
		return (int(line[1]),int(line[3]))

def checkIfClosed(userID, filePath):
	closedFileName = os.path.normpath(os.path.join(filePath, "Face_closed.txt"))
	with open(closedFileName, 'r') as closed:
		lines = closed.readlines()
		lines = [s.rstrip() for s in lines]
		if (userID in lines):
			return True
	return False

def checkIfPosted(userID, filePath):
	postedFileName = os.path.normpath(os.path.join(filePath, "Face_posted.txt"))
	with open(postedFileName, 'r') as posted:
		lines = posted.readlines()
		lines = [s.rstrip() for s in lines]
		if (userID in lines):
			return True
	return False

def addToPosted(userID, filePath):
	postedFileName = os.path.normpath(os.path.join(filePath, "Face_posted.txt"))
	with open(postedFileName, 'a') as posted:
		posted.write(userID + "\n")

def writeServerPostSummary(dirPath, userID, faceOpened, gazeOpened, poseOpened, faceDeleted, gazeDeleted, poseDeleted, nLines, totalInputLines, attemptedSendLines, faceSentLines, faceNotSentLines):
	dirPath = os.path.normpath(os.path.join(dirPath, userID))
	serverSummary = os.path.normpath(os.path.join(dirPath, userID)) + "-ServerPostSummary.txt"
	with open(serverSummary, 'a+') as outputFile:
		outputFile.write("userID: " + userID + "\n")
		outputFile.write("Face File Successfully  Opened: " + faceOpened + "\n")
		outputFile.write("Gaze File Successfully  Opened: " + gazeOpened + "\n")
		outputFile.write("Pose File Successfully  Opened: " + poseOpened + "\n")
		outputFile.write("Send Every N Lines: " + str(nLines)+ "\n")
		outputFile.write("Total Number of Lines in Input File: " + str(totalInputLines)+ "\n")
		outputFile.write("Total Number of Nth Lines: " + str(attemptedSendLines)+ "\n")
		outputFile.write("Invalid Nth Lines (not sent to server): " + str(faceNotSentLines) + "\n")
		outputFile.write("Valid Nth Lines (sent to server): " + str(faceSentLines) + "\n")
		outputFile.write("Face File Deleted: " + faceDeleted + "\n")
		outputFile.write("Gaze File Deleted: " + gazeDeleted + "\n")
		outputFile.write("Pose File Deleted: " + poseDeleted + "\n")

def concatLines(userID, dirPath, deleteFile):
	dirPath = os.path.normpath(os.path.join(dirPath, userID))
	
	faceFile = os.path.normpath(os.path.join(dirPath, userID)) + "-aus.txt"
	gazeFile = os.path.normpath(os.path.join(dirPath, userID)) + "-clmgaze.txt"
	poseFile = os.path.normpath(os.path.join(dirPath, userID)) + "-pose.txt"
	faceOpened = "No"
	gazeOpened = "No"
	poseOpened = "No"
	faceDeleted = "No"
	gazeDeleted = "No"
	poseDeleted = "No"
	
	if (os.path.isfile(faceFile)):
		with open(faceFile, 'r') as fileToRead:
			faceOpened = "Yes"
			faceLines = fileToRead.readlines()
		if (deleteFile == 1):
			os.remove(faceFile)
			faceDeleted = "Yes"
	
	if (os.path.isfile(gazeFile)):
		with open(gazeFile, 'r') as fileToRead:
			gazeOpened = "Yes"
			gazeLines = fileToRead.readlines()
		if (deleteFile == 1):
			os.remove(gazeFile)
			gazeDeleted = "Yes"
	
	if (os.path.isfile(poseFile)):
		with open(poseFile, 'r') as fileToRead:
			poseOpened = "Yes"
			poseLines = fileToRead.readlines()
		if (deleteFile == 1):
			os.remove(poseFile)
			poseDeleted = "Yes"

	outLines = []
	
	# fix up what happens if diff # of lines in each file
	if (not len(faceLines) == len(gazeLines) or not len(faceLines) == len(poseLines)):
		for line in faceLines:
			line = line.split(',')
			outLines.append(line)
		
		for line in gazeLines:
			line = line.split(',')
			outLines.append(line)
		
		for line in poseLines:
			line = line.split(',')
			outLines.append(line)
	
	else:
		for i in range(0,len(faceLines)):
			faceLine = faceLines[i].split(', ')
			gazeLine = gazeLines[i].split(', ')
			poseLine = poseLines[i].split(', ')
			if (faceLine[0] == gazeLine[0] and faceLine[0] == poseLine[0]):
				line = faceLine + gazeLine + poseLine
			elif (faceLine[0] == gazeLine[0]):
				line = faceLine + gazeLine + ['NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA']
			elif (faceLine[0] == poseLine[0]):
				line = faceLine + ['NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA'] + poseLine
			else:
				line = faceLine + ['NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA', 'NA']
			outLines.append(line)
			
	
	return (outLines, faceOpened, gazeOpened, poseOpened, faceDeleted, gazeDeleted, poseDeleted)

def extractAndPost(userID, deleteFile, nthLine, dirPath):
	concatOutput = concatLines(userID, dirPath, deleteFile)
	lines = concatOutput[0]
	faceOpened = concatOutput[1]
	gazeOpened = concatOutput[2]
	poseOpened = concatOutput[3]
	faceDeleted = concatOutput[4]
	gazeDeleted = concatOutput[5]
	poseDeleted = concatOutput[6]
	
	outLines = []
	validityIndex = 3
	totalLines = 0
	attemptedLines = 0
	validLines = 0
	invalidLines = 0
   
	for line in lines:
		# TO DO: HANDLE WHEN NOT ALL THREE MODALITIES CONCAT TOGETHER IN A SINGLE LINE
		# I COULD ALSO PROBABLY PUT THIS CODE IN CONCAT
		if(not line[0].startswith('timestamp_gmt') and len(line) == 53):
			totalLines = totalLines + 1
			if totalLines % nthLine == 0:
				attemptedLines = attemptedLines + 1
				if line[validityIndex] == "0":		
					invalidLines = invalidLines + 1
				else:
					validLines = validLines + 1
					outLines.append(line)
	writeServerPostSummary(dirPath, userID, faceOpened, gazeOpened, poseOpened, faceDeleted, gazeDeleted, poseDeleted, nthLine, totalLines, attemptedLines, validLines, invalidLines)

	# actually do the post
	# code stolen from http://www.giantflyingsaucer.com/blog/?p=5557
	loop = asyncio.get_event_loop()
	tasks = []
	for line in outLines:
		tasks.append(post(line, userID, "webcam"))
	
	tasks.append(post([nthLine, totalLines, attemptedLines, validLines, invalidLines], userID, "validity"))
		

	if (len(tasks) > 0):
		loop.run_until_complete(asyncio.wait(tasks))	

@asyncio.coroutine
def post(line, userID, modality):
	if (modality == "webcam"):
		data = {
			'data': {
				'modality': modality,
				'timestamp_gmt_face': line[0],
				'timestamp_local_face': line[1],
				'frame_face': line[2],
				'success_face': line[3],
				'confidence_face': line[4],
				'AU01_r': line[5],
				'AU04_r': line[6],
				'AU06_r': line[7],
				'AU10_r': line[8],
				'AU12_r': line[9],
				'AU14_r': line[10],
				'AU17_r': line[11],
				'AU25_r': line[12],
				'AU02_r': line[13],
				'AU05_r': line[14],
				'AU09_r': line[15],
				'AU15_r': line[16],
				'AU20_r': line[17],
				'AU26_r': line[18],
				'AU12_c': line[19],
				'AU23_c': line[20],
				'AU28_c': line[21],
				'AU04_c': line[22],
				'AU15_c': line[23],
				'AU45_c': line[24],
				'timestamp_gmt_gaze': line[25],
				'timestamp_local_gaze': line[26],
				'frame_gaze': line[27],
				'confidence_gaze': line[28],
				'success_gaze': line[29],
				'x_0': line[30],
				'y_0': line[31],
				'z_0': line[32],
				'x_1': line[33],
				'y_1': line[34],
				'z_1': line[35],
				'x_h0': line[36],
				'y_h0': line[37],
				'z_h0': line[38],
				'x_h1': line[39],
				'y_h1': line[40],
				'z_h1': line[41],
				'timestamp_gmt_pose': line[42],
				'timestamp_local_pose': line[43],
				'frame_pose': line[44],
				'confidence_pose': line[45],
				'success_pose': line[46],
				'Tx': line[47],
				'Ty': line[48],
				'Tz': line[49],
				'Rx': line[50],
				'Ry': line[51],
				'Rz': line[52]
			},
			'id': userID,
			'timestamp': datetime.datetime.strptime(line[0], '%m/%d/%Y %H:%M:%S.%f').timestamp()
		}
		request = requests.post(url, json=data, headers=headers_face, verify=False)
		
	elif (modality == "validity"):
		data = {
			'data': {
				'modality': modality,
				'sendN': line[0],
				'totalInputLines': line[1],
				'totalNthLines': line[2],
				'validNthLines': line[3],
				'invalidNthLines': line[4],
			},
			'id': userID,
			'timestamp': calendar.timegm(time.gmtime())
		}
		request = requests.post(url, json=data, headers=headers_validity, verify=False)
	# should probably handle if the modality is s/t unexpected

if __name__ == '__main__':
	configData = readConfig(os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..', '..')))
	deleteFile = configData[0]
	sendN = configData[1]
	filePath = os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..', "bin"))
	filePath = os.path.normpath(filePath)

	outputPath = filePath.replace("bin", "")
	outputPath = os.path.normpath(os.path.join(outputPath, '..', 'Face', 'Output'))	
	
	if (len(sys.argv) == 2):
		userName = sys.argv[1]
		closed = checkIfClosed(userName, filePath)
		posted = checkIfPosted(userName, filePath)
		if (closed and not posted):
			extractAndPost(userName, deleteFile, sendN, outputPath)
			addToPosted(userName,filePath)

	elif (len(sys.argv) == 1):
		# make this program run forever
		run = True
		while(run):
			userNames = [d for d in os.listdir(outputPath) if os.path.isdir(os.path.join(outputPath, d))]
			for userName in  userNames:
				
				#start = time.time()
				closed = checkIfClosed(userName, filePath)
				posted = checkIfPosted(userName, filePath)
				if (closed and not posted):
					extractAndPost(userName, deleteFile, sendN, outputPath)
					addToPosted(userName,filePath)
					
			run = False
			#time.sleep(7200) # sleep for 2 hours and try this again

	else:
		print("Enter the username as a command line arg, or 0 to check all files.")
		exit()
