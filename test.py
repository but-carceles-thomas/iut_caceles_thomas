import math
import cv2
from matplotlib import pyplot as plt
#print(cv2.__version__)

import numpy as np
from urllib.request import urlopen
req = urlopen('http://www.vgies.com/downloads/robocup.png')
arr = np.asarray(bytearray(req.read()), dtype=np.uint8)
img = cv2.imdecode(arr, -1)
cv2.imshow('RoboCup_image', img)

B, G, R = cv2.split(img)
#cv2.imshow("original", img)
#cv2.imshow("blue", B)
#cv2.imshow("Green", G)
#cv2.imshow("Red", R)

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
#cv2.imshow("Image_Masque_Jaune", imagemaskyellow)

#Definition des limites basses et hautes de la couleur verte en HSV
#A noter que le vert se situe vers les 60 degres dans la roue de couleur HSV en H
lower_green = np.array([50, 50, 50])
upper_green = np.array([75,255,255])
#Masquage de l’image HSV pour ne garder que les zones vertes
imagemaskgreen = cv2.inRange(imagehsv, lower_green, upper_green)
#cv2.imshow("Image_Masque_Verte", imagemaskgreen)

height = 474
width = 850
img2 = np.zeros((height, width, 3), np.uint8)
img2[:, :] = (0, 255, 0)
#cv2.imshow('image', img2)
resultimage = cv2.bitwise_and(img2, img2, mask = imagemaskgreen)
#cv2.imshow('Merged_image', resultimage)

img3 = np.zeros((height, width, 3), np.uint8)
img3[:, :] = (0, 255, 255)
resultimage2 = cv2.bitwise_and(img3, img3, mask = imagemaskyellow)
#cv2.imshow('Merged_image2', resultimage2)

resultimage3 = cv2.bitwise_or(resultimage, resultimage2)
#cv2.imshow('Merged_image3', resultimage3)

height = img.shape[0]
width = img.shape[1]
channels = img.shape[2]
center = (width //2, height //2)
radius = min(center[0], center[1], width - center[0], height - center[1]) // 2

mask = np.zeros((height, width), dtype=np.uint8)
cv2.circle(mask, center, radius, (255), thickness=-1)
red_circle = np.zeros_like(img, np.uint8)
red_circle[mask == 255] = (0, 0, 255)

imgTransform = img
#for x in range(0, (int)(width)):
    #for y in range (0, (int)(height/2)):
        #imgTransform[y,x][0] *= x/width
        #imgTransform[y,x][1] *= x/width
        #imgTransform[y,x][2] *= math.sqrt((x^2)+(y^2))<width/2 \*

red_circle = cv2.bitwise_and(red_circle, red_circle, mask=mask)
imgTransform = cv2.addWeighted(img, 1, red_circle, 0.5, 0)
#cv2.imshow("Transformation_manuelle_de_l'image", imgTransform)

#Conversion de l’image en niveaux de gris
imageGray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
#cv2.imshow('Grayscale', imageGray)
#Calcul de l’histogramme
hist,bins = np.histogram(imageGray.flatten(),256,[0,256])
#Calcul de l’histogramme cumule
cdf = hist.cumsum()
cdf_normalized = cdf * float(hist.max()) / cdf.max()
#Affichage de l’histogramme cumule en bleu
plt.plot(cdf_normalized, color = 'b')
#Affichage de l’histogramme en rouge
plt.hist(imageGray.flatten(),256,[0,256], color = 'r')
plt.xlim([0,256])
plt.legend(('cdf','histogram'), loc = 'upper left')
#plt.show()

#Egalisation de l’histogramme
imgEqu = cv2.equalizeHist(imageGray)
#Calcul de l’histogramme egalise
histEq,binsEq = np.histogram(imgEqu.flatten(),256,[0,256])
#Calcul de l’histogramme écumul éégalis
cdfEq = histEq.cumsum()
cdfEq_normalized = cdfEq * float(histEq.max()) / cdfEq.max()
#Affichage de l’image egalisee
#cv2.imshow('Image égalise', imgEqu)
plt.clf()

#Affichage de l’histogramme egalise cumule en bleu
plt.plot(cdfEq_normalized, color = 'b')
#Affichage de l’histogramme egalise en rouge
plt.hist(imgEqu.flatten(),256,[0,256], color = 'r')
plt.xlim([0,256])
plt.legend(('cdfEq','histogramEq'), loc = 'upper left')
#plt.show()



Heq = cv2.equalizeHist(H)
Seq = cv2.equalizeHist(S)
Veq = cv2.equalizeHist(V)

imageHSVEq = cv2.merge([Heq, Seq, Veq])
imageBGR = cv2.cvtColor(imageHSVEq, cv2.COLOR_HSV2BGR)
#cv2.imshow('Image H Egalisee', Heq)
#cv2.imshow('Image S Égalisée', Seq)
#cv2.imshow('Image V Égalisée', Veq)
#cv2.imshow('Image HSV Égalisée', imageBGR)

img_blur = cv2.GaussianBlur(img,(3,3),0)
img_blur1 = cv2.GaussianBlur(img,(5,5),0)
img_blur2 = cv2.GaussianBlur(img,(9,9),0)
#cv2.imshow('Image Flout', img_blur)
#cv2.imshow('Image Flout1', img_blur1)
#cv2.imshow('Image Flout2', img_blur2)

sobelx = cv2.Sobel(src=img_blur2, ddepth=cv2.CV_64F, dx=1, dy=0, ksize=5) # Sobel Edge Detection on the
sobelx2 = cv2.Sobel(src=img_blur1, ddepth=cv2.CV_64F, dx=1, dy=0, ksize=5)
sobely = cv2.Sobel(src=img_blur2, ddepth=cv2.CV_64F, dx=0, dy=1, ksize=5) # Sobel Edge Detection on the
sobelxy = cv2.Sobel(src=img_blur2, ddepth=cv2.CV_64F, dx=1, dy=1, ksize=5) # Combined X and Y Sobel Edge

   # Display Sobel Edge Detection Images
#cv2.imshow('Sobel X', sobelx)
#cv2.imshow('Sobel X2', sobelx2)
#cv2.imshow('Sobel Y', sobely)
#cv2.imshow('Sobel X Y using Sobel() function', sobelxy)

kernel2 = np.array([[1, 2, 1], [0, 0, 0], [-1, -2, -1]], dtype=np.float32)
kernel1 = np.array([[-1, 0, 1], [-2, 0, 2], [-1, 0, 1]], dtype=np.float32)
identity1 = cv2.filter2D(src=img_blur, ddepth=-1, kernel=kernel1)
identity2 = cv2.filter2D(src=img_blur, ddepth=-1, kernel=kernel2)
sobeloutput = np.sqrt(np.square(identity1) + np.square(identity2))

#cv2.imshow('Sobel X fait main', identity1)
#cv2.imshow('Sobel Y fait main', identity2)
#cv2.imshow('Sobel XY fait main', sobeloutput)

# Detection de contours par filtre de Canny
edges = cv2.Canny(image=img_blur, threshold1=100, threshold2=200)
edges1 = cv2.Canny(image=img_blur, threshold1=400, threshold2=400)
#cv2.imshow('Canny Edge Detection', edges)
#cv2.imshow('Canny Edge1 Detection', edges1)

#EROSION - DILATION
# Utilisation d’une matrice de 1 de taille 5x5 comme noyau
kernel = np.ones((5, 5), np.uint8)
img_erosion = cv2.erode(img, kernel, iterations=1)
img_dilation = cv2.dilate(img, kernel, iterations=1)
#cv2.imshow('Erosion', img_erosion)
#cv2.imshow('Dilation', img_dilation)

cv2.waitKey(0)
cv2.destroyAllWindows()
