# -*- coding: utf-8 -*
import cv2
import numpy as np
import math


def computeGradientx(img):
    rows, cols = img.shape
    img = img.astype('float')

    grad = np.zeros((img.shape), 'float')
    for y in range(rows):
        grad[y][0] = img[y][1] - img[y][0]
        for x in range(1, cols - 1):
            grad[y][x] = (img[y][x+1] - img[y][x-1])/2
        grad[y][cols-1] = img[y][cols-1] - img[y][cols-2]
    stdgrad = np.std(grad, ddof=1)
    return grad


def computeGradienty(img):
    rows, cols = img.shape
    img = img.astype('float')

    grad = np.zeros((img.shape), 'float')
    for x in range(cols):
        grad[0][x] = img[1][x] - img[0][x]
        for y in range(1, rows - 1):
            grad[y][x] = (img[y+1][x] - img[y-1][x])/2
        grad[rows-1][x] = img[rows-1][x] - img[rows-2][x]
    return grad


def matrixMagnitude(mat_x, mat_y):
    rows, cols = np.shape(mat_x)
    res_arr = np.zeros((rows, cols))
    for y in range(rows):
        for x in range(cols):
            gX = mat_x[y][x]
            gY = mat_y[y][x]
            magnitude = math.sqrt((gX * gX) + (gY * gY))
            res_arr[y][x] = magnitude
    return res_arr

# 选择的半径范围为0.5倍的瞳孔半径到1.5倍的瞳孔半径


def frst(img, radii, beta):
    imgx, imgy = img.shape
    output = np.zeros(img.shape, np.uint8)

    O_n = np.zeros(img.shape, np.int16)
    M_n = np.zeros(img.shape, np.int16)
    gx = computeGradientx(img)
    gy = computeGradienty(img)
    # gnorms = abs(gx) + abs(gy)
    gnorms = matrixMagnitude(gx, gy)
    gthresh = np.amax(gnorms)*beta

    # g/gnorms * radii
    gpx = np.multiply(np.divide(gx, gnorms, out=np.zeros(
        gx.shape), where=gnorms != 0), radii).round().astype(int)
    gpy = np.multiply(np.divide(gy, gnorms, out=np.zeros(
        gy.shape), where=gnorms != 0), radii).round().astype(int)

    # print("gpx\n", gpx)
    # print("gpy\n", gpy)
    # print(gnorms)

    for coords, gnorm in np.ndenumerate(gnorms):
        i, j = coords
        if gnorm >= gthresh:
            ppve = (i+gpy[i, j], j+gpx[i, j])
            if ppve[0] > imgx - 1 or ppve[1] > imgy - 1 or ppve[0] < 0 or ppve[1] < 0:
                continue
            M_n[ppve] += gnorm
    minVal, maxVal, minLoc, MaxLoc = cv2.minMaxLoc(M_n)
    return MaxLoc, maxVal
