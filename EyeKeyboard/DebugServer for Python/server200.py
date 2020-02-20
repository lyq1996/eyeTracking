# -*- coding: utf-8 -*
import socket
import time
import cv2
import numpy as np
import imutils
from imutils.video import VideoStream

# define one byte flag:
# 0center 1right 2left 3ensure 4inited 5img 6bigimg 7up 8down


def sendimg(sock, img):
    data = img.tostring()
    sock.send(('5').encode('utf-8') + data)
    return


def sendbigimg(sock, img):
    data = img.tostring()
    sock.send(('6').encode('utf-8') + data)
    return


def center(sock):
    sock.send(('0').encode('utf-8'))
    return


def left(sock):
    sock.send(('2').encode('utf-8'))
    return


def right(sock):
    sock.send(('1').encode('utf-8'))
    return


def down(sock):
    sock.send(('8').encode('utf-8'))
    return


def up(sock):
    sock.send(('7').encode('utf-8'))
    return


def init(sock):
    sock.send(('4').encode('utf-8'))
    return


def ensure(sock):
    sock.send(('3').encode('utf-8'))
    return


def main():
    host = '127.0.0.1'
    port = 8080
    print('Start a socket:TCP...')
    socket_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    print('TCP listen in: ', host, 'port:', port)
    host_addr = (host, port)
    socket_tcp.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    socket_tcp.bind(host_addr)
    socket_tcp.listen(1)
    print('Try to receiving...')
    socket_con, addr = socket_tcp.accept()

    client_ip, port = addr
    print('Client:', client_ip, 'Port:', port)

    img = cv2.imread('des.png')
    img1 = cv2.imread('1.png')
    img2 = cv2.imread('2.png')
    ret, img = cv2.imencode('.jpg', img)
    ret1, img1 = cv2.imencode('.jpg', img1)
    ret, img2 = cv2.imencode('.jpg', img2)
    time.sleep(1)
    # vs = VideoStream(src=0).start()

    # 4 init 、 0 center、 1 right 、2 left 、3 ensure
    while True:
        try:

            # 从摄像头获取大图
            '''
            while True:
                frame = vs.read()
                frame = imutils.resize(frame, width=500)
                cv2.rectangle(frame, (100, 100), (200, 150), (0, 255, 0), 2)
                frame = imutils.resize(frame, height=240, width=250)
                ret, imgss = cv2.imencode('.jpg', frame)
                sendbigimg(socket_con, imgss)
                recvdata = socket_con.recv(1)
                if recvdata == b'8':
                    print('inited successed!')
                    break
                # print(recvdata)

            # 静态大图，不含指令
            '''
            while True:
                sendbigimg(socket_con, img2)
                recvdata = socket_con.recv(1)
                if recvdata == b'8':
                    print('inited successed!')
                    break
                # print(recvdata)

            # 初始化
            init(socket_con)
            # 接收来自client的1字节,阻塞,但不判断
            recvdata = socket_con.recv(1)

            # 指令 + 小图,并非一定一起发,可能单独小图,但若发指令随后必发小图

            # 右移5次到达6
            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            # 下移2次到达H

            down(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            down(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 确定输入H
            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            # 左移3次到达D
            left(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            left(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            left(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 上移1次到达E
            up(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 确认输入E
            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            # 右移6次到达O
            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 下移1次到达L
            down(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 确认输入L
            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            # 再次L
            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 上移到达O
            up(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 确认输入O
            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            # 下移三次到达 del
            down(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            down(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            down(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            # 删除hello
            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)
            ensure(socket_con)

            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            ensure(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(0.1)
            sendimg(socket_con, img)
            recvdata = socket_con.recv(1)

            #zaidiwuhang zuoyouceshi
            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            left(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            right(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)

            left(socket_con)
            recvdata = socket_con.recv(1)
            time.sleep(1)
            sendimg(socket_con, img1)
            recvdata = socket_con.recv(1)
            """
            test(socket_con)
            """

            socket_con.close()
            socket_tcp.close()
            time.sleep(1)
            break

        except KeyboardInterrupt:
            socket_con.close()
            socket_tcp.close()
            time.sleep(1)
            break


if __name__ == "__main__":

    main()
