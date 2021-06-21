import datetime
import os
import sys
import time
import requests
import urllib3
import asyncio

# helpers
# https://docs.python.org/3/library/asyncio-task.html#asyncio.Future.set_result
# http://www.giantflyingsaucer.com/blog/?p=5557

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
ACCESS_TOKEN_FACE = 'token'
ACCESS_TOKEN_POSE = 'token'
ACCESS_TOKEN_OPENFACE_GAZE = 'token'
ACCESS_TOKEN_OPENFACE_4C = "token"  


url = 'https://url.com'
headers_face = {"Authorization": "Token "+ACCESS_TOKEN_FACE}
headers_pose = {"Authorization": "Token "+ACCESS_TOKEN_POSE}
headers_openface_gaze = {"Authorization": "Token "+ACCESS_TOKEN_OPENFACE_GAZE}
headers_4C = {"Authorization": "Token "+ACCESS_TOKEN_OPENFACE_4C}

count = 0;

def readConfig(filePath):
    configFileName = os.path.normpath(os.path.join(filePath, "bin\\Gaze_config.txt"))
    with open(configFileName, 'r') as config:
        lines = config.readlines()
        line = lines[1].strip().split(",") # first line is the header so just read the second line
        if (len(line) != 5):
            print("Something wrong with config file. Do some error handling here.") # FIX
            exit()
        return (int(line[1]),int(line[3]),line[4])

def checkIfClosed(userID, filePath):
    closedFileName = os.path.normpath(os.path.join(filePath, "bin\\Gaze_closed.txt"))
    with open(closedFileName, 'r') as closed:
        lines = closed.readlines()
        lines = [s.rstrip() for s in lines]
        if (userID in lines):
            return True
    return False

def checkIfPosted(userID, filePath):
    postedFileName = os.path.normpath(os.path.join(filePath, "bin\\Gaze_posted.txt"))
    with open(postedFileName, 'r') as posted:
        lines = posted.readlines()
        lines = [s.rstrip() for s in lines]
        if (userID in lines):
            return True
    return False

def addToPosted(userID, filePath):
    postedFileName = os.path.normpath(os.path.join(filePath, "bin\\posted.txt"))
    with open(postedFileName, 'a') as posted:
        posted.write(userID + "\n")

def printDone(userID, dirPath):
    serverSummary = os.path.normpath(os.path.join(dirPath, userID)) + "-Done.txt"
    with open(serverSummary, 'a+') as outputFile:
        outputFile.write(str(count) + "\n")


def writeServerPostSummary(dirPath, userID, fileOpened, fileDeleted, fileName, nLines, totalInputLines, attemptedSendLines, faceSentLines, faceNotSentLines, modality):

    serverSummary = os.path.normpath(os.path.join(dirPath, userID)) + "-ServerPostSummary.txt"
    print (serverSummary)
    with open(serverSummary, 'a+') as outputFile:
        outputFile.write("userID: " + userID + "\n")
        outputFile.write("Openface Modality: " + modality + "\n")
        outputFile.write("Input File Name: " + fileName + "\n")
        outputFile.write("Input File Successfully  Opened: " + fileOpened + "\n")
        outputFile.write("Send Every N Lines: " + str(nLines)+ "\n")
        outputFile.write("Total Number of Lines in Input File: " + str(totalInputLines)+ "\n")
        outputFile.write("Total Number of Nth Lines: " + str(attemptedSendLines)+ "\n")
        outputFile.write("Invalid Nth Lines (not sent to server): " + str(faceNotSentLines) + "\n")
        outputFile.write("Valid Nth Lines (sent to server): " + str(faceSentLines) + "\n")
        outputFile.write("Input File Deleted: " + fileDeleted + "\n")
        outputFile.write("\n\n")

def extractAndPost(userID, deleteFile, nthLine, dirPath, modality):
    print ("Hello")
    dirPath = os.path.normpath(os.path.join(filePath, "Output"))
    dirPath = os.path.normpath(os.path.join(dirPath, userID))
    
    if (modality == "face"):
        inFile = os.path.normpath(os.path.join(dirPath, userID)) + "-aus.txt"
        validityIndex = 3

    elif (modality == "gaze"):
        inFile = os.path.normpath(os.path.join(dirPath, userID)) + "-clmgaze.txt"
        validityIndex = 4

    elif (modality == "pose"):
        inFile = os.path.normpath(os.path.join(dirPath, userID)) + "-pose.txt"
        validityIndex = 4
    elif(modality == "4C"):
        #print ("Yay")
        validityIndex = 7
        inFile = os.path.normpath(os.path.join(dirPath, userID)) + "_GazeLog4C.txt"
        print ("Transferring File for " + userID)
    
    else:
        print("Invalid modality. Probably should log this instead of just exiting.") # FIX
        exit()
    
    outLines = []
    totalLines = 0
    attemptedLines = 0
    validLines = 0
    invalidLines = 0
    opened = "No"
    deleted = "No"
    firstLine = True
        
    timeOutTries = 0
    print(inFile)
    #exit()
    while((not os.path.isfile(inFile)) and timeOutTries < 5):
        print("File not accessed, waiting to try again ")
        time.sleep(3) # sleep for 3 seconds
        timeOutTries = timeOutTries + 1
        
    if ((not os.path.isfile(inFile)) and timeOutTries == 5):
        writeServerPostSummary(dirPath, userID, opened, deleted, inFile, nthLine, totalLines, attemptedLines, validLines, invalidLines, modality)
        #sys.exit()
        return False

    with open(inFile, 'r') as fileToRead:
        opened = "Yes"
        lines = fileToRead.readlines()
        for line in lines:
            if (firstLine):
                firstLine = False
            else:
                totalLines = totalLines + 1
                if totalLines % nthLine == 0:
                    if type(line) != list:
                        line = line.strip()
                        
                        if modality == "4C":
                            line = line.split("\t")
                        else:
                            line = line.split(', ')
                    attemptedLines = attemptedLines + 1
                    if line[validityIndex] == "0":        
                        invalidLines = invalidLines + 1
                    else:
                        validLines = validLines + 1
                        outLines.append(line)
    
    if (deleteFile == 1):
        if (os.path.isfile(inFile)):
            os.remove(inFile)
            deleted = "Yes"
    writeServerPostSummary(dirPath, userID, opened, deleted, inFile, nthLine, totalLines, attemptedLines, validLines, invalidLines, modality)

    # actually do the post
    # code stolen from http://www.giantflyingsaucer.com/blog/?p=5557
    loop = asyncio.get_event_loop()
    tasks = []
    for line in outLines:
        tasks.append(post(line, userID, modality))# tasks.append(post(asyncio.Future(), line, userID))
    
    if (len(tasks) > 0):
        loop.run_until_complete(asyncio.wait(tasks))
    printDone(userID,dirPath)
    addToPosted(userName,softPath)
    return True
    #loop.close()

@asyncio.coroutine
def post(line, userID, modality):
    global count
    if (modality == "4C"):
        #print("In4C")
        data = {
            'data': {
				"ParticipantID" : line[0],
				"condition" : line[1],
				"timeGMT" : line[2],
				"ComputerTimeStamp" : line[3],
				"EyeTrackerTimeStamp" : line[4],
				"LeftGazePointX" : line[5],
				"LeftGazePointY" : line[6],
				"RightGazePointX" : line[7],
				"RightGazePointY" : line[8],
				"LeftGazePointXUser" : line[9],
				"LeftGazePointYUser" : line[10],
				"RightGazePointXUser" : line[11],
				"RightGazePointYUser" : line[12],
				"LeftGazePointXMonitor" : line[13],
				"LeftGazePointYMonitor" : line[14],
				"RightGazePointXMonitor" : line[15],
				"RightGazePointYMonitor" : line[16],
				"LeftPupilDiameter" : line[17],
				"RightPupilDiameter" : line[18],
				"LeftEyeValidity" : line[19],
				"RightEyeValidity" : line[20],
				"LeftPupilValidity" : line[21],
				"RightPupilValidity" : line[22],
				"SystemTime" : line[23],
				"DeviceTime" : line[24],
				"Width" : line[25],
				"Height" : line[26],
				"Count" : line[27],

            },
            'id': userID,
            'timestamp': datetime.datetime.strptime(line[2], '%m/%d/%Y %H:%M:%S.%f').timestamp()
        }
        #print (line)
        request = requests.post(url, json=data, headers=headers_4C, verify=False)
        count = count + 1

        #print (request.status_code)
        #print (request.text)	
    
    else:
        print("Error. Invalid modality.")
        exit() 
    
    

 



if __name__ == '__main__':
    print(os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..')))
   

    configData = readConfig(os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..')))
    deleteFile = 0
    sendN = configData[1]
    filePath = os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..'))
    softPath = os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..'))
    filePath = configData[-1]
    outputPath = os.path.normpath(os.path.join(filePath, "Output"))

    print(outputPath)
    print(softPath)
    print(sys.argv)

    if (len(sys.argv) == 2):
        print("offWeGo")
        userName = sys.argv[1]
        closed = True
        posted = checkIfPosted(userName, softPath)
        if (closed and not posted):
            
            extractAndPost(userName, deleteFile, sendN, filePath, "4C")

    elif (len(sys.argv) == 1):
        # make this program run forever
        run = True
        while(run):
            userNames = [d for d in os.listdir(outputPath) if os.path.isdir(os.path.join(outputPath, d))]
            print (userNames)
            #x = input()
            #exit()
            for userName in  userNames:
                
                #start = time.time()
                #closed = checkIfClosed(userName, softPath)
                closed = True
                posted = checkIfPosted(userName, softPath)
                if posted:
                    print("Data for " + userName + " has already been sent")
                if (closed and not posted):

                    t = extractAndPost(userName, deleteFile, sendN, filePath, "4C")

                    if t:
                        print("AsyncStarted")

                        addToPosted(userName,softPath)
                    else:
                        print("File not found, No data transferred for " + userName)
            run = False
            #time.sleep(30)
            #time.sleep(7200) # sleep for 2 hours and try this again
