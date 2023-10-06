import math
import cv2
print(cv2.__version__)

import numpy as np
from urllib.request import urlopen
req = urlopen('http://www.vgies.com/downloads/robocup.png')
arr = np.asarray(bytearray(req.read()), dtype=np.uint8)
img = cv2.imdecode(arr, -1)
cv2.imshow('RoboCup_image', img)

B, G, R = cv2.split(img)
cv2.imshow("original", img)
cv2.imshow("blue", B)
cv2.imshow("Green", G)
cv2.imshow("Red", R)

#converting the image to HSV color space using cvtColor function
imagehsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
H, S, V = cv2.split(imagehsv)
cv2.imshow("Hue", H)
cv2.imshow("Saturation", S)
cv2.imshow("Value", V)

#Definition des limites basses et hautes de la couleur jaune en HSV
#A noter que le jaune se situe vers les 25 degres dans la roue de couleur HSV en H
lower_yellow = np.array([20, 100, 100])
upper_yellow = np.array([30,255,255])
#Masquage de l’image HSV pour ne garder que les zones jaunes
imagemaskyellow = cv2.inRange(imagehsv, lower_yellow, upper_yellow)
cv2.imshow("Image_Masque_Jaune", imagemaskyellow)

#Definition des limites basses et hautes de la couleur verte en HSV
#A noter que le vert se situe vers les 60 degres dans la roue de couleur HSV en H
lower_green = np.array([50, 50, 50])
upper_green = np.array([75,255,255])
#Masquage de l’image HSV pour ne garder que les zones vertes
imagemaskgreen = cv2.inRange(imagehsv, lower_green, upper_green)
cv2.imshow("Image_Masque_Verte", imagemaskgreen)

height = 474
width = 850
img2 = np.zeros((height, width, 3), np.uint8)
img2[:, :] = (0, 255, 0)
#cv2.imshow('image', img2)
resultimage = cv2.bitwise_and(img2, img2, mask = imagemaskgreen)
cv2.imshow('Merged_image', resultimage)

img3 = np.zeros((height, width, 3), np.uint8)
img3[:, :] = (0, 255, 255)
resultimage2 = cv2.bitwise_and(img3, img3, mask = imagemaskyellow)
cv2.imshow('Merged_image2', resultimage2)

resultimage3 = cv2.bitwise_or(resultimage, resultimage2)
cv2.imshow('Merged_image3', resultimage3)

height = img.shape[0]
width = img.shape[1]
channels = img.shape[2]

imgTransform = img
for x in range(0, (int)(width)):
    for y in range (0, (int)(height/2)):
        imgTransform[y,x][0] *= x/width
        imgTransform[y,x][1] *= x/width
        imgTransform[y,x][2] *= math.sqrt((x^2)+(y^2))<width/2
cv2.imshow("Transformation_manuelle_de_l'image", imgTransform)

cv2.waitKey(0)
cv2.destroyAllWindows()
