# -*- coding: utf-8 -*
import frst
import frst1
import socket
import cv2
import time
import imutils
from imutils.video import VideoStream
from imutils.video import FPS


def socket_init():
    # 建立tcp socket
    host = '127.0.0.1'
    port = 8080
    print('Start a socket:TCP...')
    socket_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    host_addr = (host, port)
    socket_tcp.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    socket_tcp.bind(host_addr)
    socket_tcp.listen(1)
    return socket_tcp


def init(socket_link):
    # 初始化,需要将头部对准眼睛区域
    vs = VideoStream(src=camera_flag).start()
    while True:
        frame = vs.read()
        frame = cv2.flip(frame, 0)  # 水平镜像
        cv2.rectangle(frame, (left_eye_x, left_eye_y), (left_eye_x +
                                                        eye_region_width, left_eye_y+eye_region_height), (0, 255, 0), 2)
        frame = imutils.resize(frame, width=320, height=240)

        # frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        # cv2.imshow("FRAME", frame)

        ret, data = cv2.imencode('.jpg', frame)
        data = data.tostring()
        try:
            socket_link.send(('6').encode('utf-8') + data)
            # print(len(data))
            # cv2.imwrite('1.jpg',frame)
            recvdata = socket_link.recv(1)
        except Exception:
            print('client disconnected!')
            vs.stop()
            time.sleep(0.5)
            socket_link.close()
            return False
        if recvdata == b'8':
            # cv2.destroyAllWindows()
            vs.stop()
            time.sleep(0.5)
            return True


def loop(socket_link):
    magsum = 0
    magavg = 0
    locxsum = 0
    locysum = 0
    locxavg = 0
    locyavg = 0
    eye_state = False
    eye_left = False
    eye_right = False
    sumcount = 30
    # 眨眼次数
    blink_count = 2

    loctestx = []
    loctesty = []

    vs = VideoStream(src=camera_flag).start()

    while True:
        frame = vs.read()
        frame = cv2.flip(frame, 0)  # 水平镜像
        # 获取眼部区域
        eye_region = frame[left_eye_y:left_eye_y+eye_region_height,
                           left_eye_x:left_eye_x + eye_region_width]
        eye_region = cv2.resize(eye_region, (resizex, resizey))

        # 眼部转换成灰度图
        gray = cv2.cvtColor(eye_region, cv2.COLOR_BGR2GRAY)
        gray = cv2.GaussianBlur(gray, (15, 15), 1)

        # 对眼图区域灰度图阈值取反
        for i in range(resizex):
            for j in range(resizey):
                if gray[j][i] < gray_threshold:
                    gray[j][i] = 255 - gray[j][i]
                else:
                    gray[j][i] = 0
        loc, mags = frst.frst(gray, eye_radius, frst_threshold)

        if sumcount > 0:
            locxsum += loc[0]
            locysum += loc[1]
            magsum += mags
            sumcount -= 1
            if sumcount == 0:
                magsum /= 30
                locxsum /= 30
                locysum /= 30
                try:
                    socket_link.send(('4').encode('utf-8'))
                    recvdata = socket_link.recv(1)
                except Exception:
                    print('client disconnected!')
                    break
                print("Mag average:", magsum, "X average:",
                      locxsum, "Y average:", locysum)
            else:
                continue

        # 0 center、 1 right 、2 left 、3 ensure
        if mags < 0.3 * magsum and eye_state:
            blink_count = blink_count - 1
            if blink_count == 0:
                blink_count = 2
                try:
                    socket_link.send(('3').encode('utf-8'))
                    recvdata = socket_link.recv(1)
                except Exception:
                    print('client disconnected!')
                    break
            eye_state = False
            continue

        elif mags > 0.5 * magsum:
            eye_state = True
            cv2.circle(eye_region, loc, 2, (0, 255, 0))
            loctestx.append(loc[0])
            loctesty.append(loc[1])
            if (not eye_left) and (not eye_right):
                if loc[0] - locxsum > 5:
                    try:
                        socket_link.send(('1').encode('utf-8'))
                        recvdata = socket_link.recv(1)
                    except Exception:
                        print('client disconnected!')
                        break
                    blink_count = 2
                    eye_left = True
                    continue
                elif loc[0] - locxsum < -5:
                    try:
                        socket_link.send(('2').encode('utf-8'))
                        recvdata = socket_link.recv(1)
                    except Exception:
                        print('client disconnected!')
                        break
                    blink_count = 2
                    eye_right = True
                    continue
            elif (loc[0] - locxsum < 3) and (loc[0] - locxsum > -2):
                try:
                    socket_link.send(('0').encode('utf-8'))
                    recvdata = socket_link.recv(1)
                except Exception:
                    print('client disconnected!')
                    break
                eye_left = False
                eye_right = False
        # cv2.imshow("tracking eye", eye_region)
        ret, data = cv2.imencode('.jpg', eye_region)
        data = data.tostring()
        try:
            socket_link.send(('5').encode('utf-8') + data)
        # key = cv2.waitKey(1) & 0xFF
        # if key == ord('q'):
        # break
            recvdata = socket_link.recv(1)
        except Exception:
            print('client disconnected!')
            break
    socket_link.close()
    # cv2.destroyAllWindows()
    vs.stop()
    time.sleep(1)
    return


def main():
    socket_tcp = socket_init()
    while True:
        try:
            socket_link, addr = socket_tcp.accept()
        except KeyboardInterrupt:
            socket_tcp.close()
            break
        client_ip, port = addr
        print('Client:', client_ip, 'Port:', port)
        isinited = init(socket_link)
        if(isinited):
            loop(socket_link)


if __name__ == "__main__":
    # 全局变量
    left_eye_x, left_eye_y = 180, 100
    eye_region_width, eye_region_height = 100, 50
    eye_radius = 6
    print('frst radius:', eye_radius)
    # 灰度阈值
    gray_threshold = 30
    print('gray_threshold', gray_threshold)
    # 径向变换阈值参数
    frst_threshold = 0.5
    # 缩放后大小
    resizex, resizey = 50, 25
    # 摄像头标志位, 0为第0个摄像头、1为第1个摄像头
    camera_flag = 0
    main()
