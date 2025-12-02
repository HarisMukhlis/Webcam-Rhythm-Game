import json
import socket
import mediapipe as mp
import cv2
import numpy as np

mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
# sock.setblocking(0)
sock.settimeout(0.01)
UNITY_ADDR = ("127.0.0.1", 5052)

DEBUG_MODE = True

# def calcAngle(a, b, c):
#     a = np.array(a)
#     b = np.array(b)
#     c = np.array(c)
    
#     radian = np.arctan2(c[1]-b[1], c[0]-b[0]) - np.arctan2(a[1]-b[1], a[0]-b[0])
#     angle = np.abs(radian*180.0 / np.pi) #radian to degree
    
#     if (angle > 180.0):
#         angle = 360-angle
    
#     return angle

# class pointData:
#     def __init__(self, x, y, visibility):
#         self.x = x
#         self.y = y
#         self.visibility = visibility
#         pass

with mp_pose.Pose(min_detection_confidence=0.5,
                  min_tracking_confidence=0.5,
                  model_complexity=0) as pose:
    # start opencv cam
    cap = cv2.VideoCapture(0)
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
    
    while cap.isOpened():
        _, frame= cap.read()
        
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False #reduce memory usage
        
        results = pose.process(image) #process detection
        
        
        try:
            landmarks = results.pose_landmarks.landmark #extract each landmark/points
        except:
            pass
        
        if (DEBUG_MODE):
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
            
            mp_drawing.draw_landmarks(image, results.pose_landmarks,
                                      mp_pose.POSE_CONNECTIONS,
                                      mp_drawing.DrawingSpec(color=(255, 105, 21), thickness=2, circle_radius=2), # dot colors
                                      mp_drawing.DrawingSpec(color=(255, 255, 255), thickness=2, circle_radius=2) # line colors
                                      )
            cv2.imshow('Cam', image)
        else:
            cv2.imshow('Cam', frame)
        
        left_wrist = landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value]
        right_wrist = landmarks[mp_pose.PoseLandmark.RIGHT_WRIST.value]
        # left_elbow = landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value]
        # right_elbow = landmarks[mp_pose.PoseLandmark.RIGHT_ELBOW.value]
        
        data = {
            "left_x": left_wrist.x,
            "left_y": left_wrist.y,
            "left_vis": True if left_wrist.visibility > 0.5 else False,
            "right_x": right_wrist.x,
            "right_y": right_wrist.y,
            "right_vis": True if right_wrist.visibility > 0.5 else False,
        }
        
        dataStr = json.dumps(data) #turn obj to a json string
        # print(dataStr)
        sock.sendto(dataStr.encode(), UNITY_ADDR)

        
        if (cv2.waitKey(10)) & 0xFF == ord('q'):
            break
        
    cap.release()
    cv2.destroyAllWindows()
    sock.close()