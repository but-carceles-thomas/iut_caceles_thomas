import cv2
print(cv2.__version__)

import numpy as np
from urllib.request import urlopen
req = urlopen('http://www.vgies.com/downloads/robocup.png')
arr = np.asarray(bytearray(req.read()), dtype=np.uint8)
img = cv2.imdecode(arr, -1)
cv2.imshow('RoboCup_image', img)
cv2.waitKey(0)

B, G, R = cv2.split(img)
cv2.imshow("original", img)
cv2.waitKey(0)
cv2.imshow("blue", B)
cv2.waitKey(0)
cv2.imshow("Green", G)
cv2.waitKey(0)
cv2.imshow("Red", R)
cv2.waitKey(0)

#converting the image to HSV color space using cvtColor function
imagehsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
H, S, V = cv2.split(imagehsv)
cv2.imshow("Hue", H)
cv2.waitKey(0)
cv2.imshow("Saturation", S)
cv2.waitKey(0)
cv2.imshow("Value", V)
cv2.waitKey(0)

#Definition des limites basses et hautes de la couleur jaune en HSV
#A noter que le jaune se situe vers les 25 degres dans la roue de couleur HSV en H
lower_yellow = np.array([20, 100, 100])
upper_yellow = np.array([30,255,255])
#Masquage de l’image HSV pour ne garder que les zones jaunes
imagemaskyellow = cv2.inRange(imagehsv, lower_yellow, upper_yellow)
cv2.imshow("Image_Masque_Jaune", imagemaskyellow)
cv2.waitKey(0)

#Definition des limites basses et hautes de la couleur verte en HSV
#A noter que le vert se situe vers les 60 degres dans la roue de couleur HSV en H
lower_green = np.array([50, 50, 50])
upper_green = np.array([75,255,255])
#Masquage de l’image HSV pour ne garder que les zones vertes
imagemaskgreen = cv2.inRange(imagehsv, lower_green, upper_green)
cv2.imshow("Image_Masque_Verte", imagemaskgreen)
cv2.waitKey(0)

