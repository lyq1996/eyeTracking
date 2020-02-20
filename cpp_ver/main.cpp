#include <iostream>
#include <vector>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/ioctl.h>
#include <net/if.h>
#include <opencv2/opencv.hpp>
#include <unistd.h>
#include <signal.h>
#include <malloc.h>
#include <getopt.h>
#include <time.h>
#include "TCPServer.h"
#include "constant.h"
#include "FindEyeCenter.h"

using namespace cv;
using namespace std;

TCPServer tcp;

//输出使用方法
void usage(char *argv0)
{
    cout << "Example: "
         << argv0
         << " -d wlan0"
         << " -r 6"
         << " -c 0.30"
         << " -o 0.50"
         << " -x 120"
         << " -y 280"
         << " -g 40"
         << endl;
    exit(1);
}

void check_arg(int argc, char *argv[])
{
    char optstring[] = "d:r:c:o:x:y:g:";
    int c, index = 0;
    bool dExist = false;
    bool rExist = false;
    bool cExist = false;
    bool oExist = false;
    bool xExist = false;
    bool yExist = false;
    bool grayExist = false;

    struct option options[] =
        {
            {"device", 1, NULL, 'd'},
            {"eyeRadii", 1, NULL, 'r'},
            {"minFrstPercentage", 1, NULL, 'c'},
            {"maxFrstPercentage", 1, NULL, 'o'},
            {"x", 1, NULL, 'x'},
            {"y", 1, NULL, 'y'},
            {"g", 1, NULL, 'g'},
            {0, 0, 0, 0},
        };

    while ((c = getopt_long(argc, argv, optstring, options, &index)) != -1)
    {
        switch (c)
        {
        case 'd':
            strcpy(dev, optarg);
            dExist = true;
            break;
        case 'r':
            eye_radii = atoi(optarg);
            rExist = true;
            cout << "Frst radii:" << eye_radii << endl;
            break;
        case 'c':
            minPer = atof(optarg);
            cExist = true;
            cout << "eye closing magnitude percentage:" << minPer << endl;
            break;
        case 'o':
            maxPer = atof(optarg);
            oExist = true;
            cout << "eye opening magnitude percentage:" << maxPer << endl;
            break;
        case 'x':
            left_eye_x = atoi(optarg);
            xExist = true;
            cout << "retange x:" << left_eye_x << endl;
            break;
        case 'y':
            left_eye_y = atoi(optarg);
            yExist = true;
            cout << "retange y:" << left_eye_y << endl;
            break;
        case 'g':
            grayThreshold = atoi(optarg);
            grayExist = true;
            cout << "Gray Threshold:" << grayThreshold << endl;
            break;
        default:
            usage(argv[0]);
            break;
        }
    }
    if (!dExist || !rExist || !cExist || !oExist || !xExist || !yExist || !grayExist)
    {
        usage(argv[0]);
    }
}

//IP地址初始化,获取指定网卡上的IP地址
void ip_init()
{
    int sockfd;

    if ((sockfd = socket(AF_INET, SOCK_DGRAM, 0)) == -1)
    {
        perror("Device init");
        exit(1);
    }

    struct ifreq addr;

    memset(&addr, 0x0, sizeof addr);
    strcpy(addr.ifr_name, dev);
    if (ioctl(sockfd, SIOCGIFADDR, (char *)&addr) == -1)
    {
        perror("Device init");
        exit(1);
    }
    strcpy(local_ip, inet_ntoa(((struct sockaddr_in *)&addr.ifr_addr)->sin_addr));
    close(sockfd);
}

//发送大图
void send_big_img(vector<unsigned char> &mat)
{
    string str(mat.begin(), mat.end());
    str.insert(0, "6");
    tcp.Send(str);
}

//发送眼部图
void send_img(vector<unsigned char> &mat)
{
    string str(mat.begin(), mat.end());
    str.insert(0, "5");
    tcp.Send(str);
}

//发送眼球中间指令
void center()
{
    tcp.Send("0");
}
//发送右指令
void right()
{
    tcp.Send("1");
}

//发送左指令
void left()
{
    tcp.Send("2");
}

//发送确认指令
void ensure()
{
    tcp.Send("3");
}

//发送初始化指令
void init()
{
    tcp.Send("4");
}

//发送上指令
void up()
{
    tcp.Send("7");
}

//发送下指令
void down()
{
    tcp.Send("8");
}

//处理ctrl + c,会进入TIME_WAIT状态,所以尽量不要关闭,减少麻烦
void handle_ctrl_c(int sig)
{
    cout << "\nYou Pressed Ctrl+C" << endl;
    tcp.detach();
    sleep(0.1);
    exit(0);
}

// 脸部初始化,对准矩形框
bool face_init()
{
    VideoCapture capture(camera_flag);
    Mat frame;
    vector<unsigned char> frame_encode;
    Rect rect_eye(left_eye_x, left_eye_y, eye_region_width, eye_region_height);
    Point pre_eye_center = Point(left_eye_x + eye_region_width / 2, left_eye_y + eye_region_height / 2);
    while (1)
    {
        capture.read(frame);
        //rotate(frame, frame, ROTATE_90_CLOCKWISE);
        rotate(frame, frame, ROTATE_90_COUNTERCLOCKWISE);
        flip(frame, frame, 1);

        rectangle(frame, rect_eye, Scalar(0, 255, 0), 2, 1, 0);
        circle(frame, pre_eye_center, 3, Scalar(0, 255, 0), -1, 8);

        resize(frame, frame, Size(240, 320));
        imencode(".jpg", frame, frame_encode);
        send_big_img(frame_encode);
        recvdata = tcp.Recv();
        if (recvdata == "0")
        {
            tcp.disconnect();
            capture.release();
            return false;
        }
        else if (recvdata == "8")
        {
            capture.release();
            return true;
        }
    }
}

//判断左右loop循环
int loop()
{
    VideoCapture capture(camera_flag);
    Mat frame;
    Mat eyeROI;
    Mat eyeGRAY;
    Rect rect_eye(left_eye_x, left_eye_y, eye_region_width, eye_region_height);
    vector<unsigned char> frame_encode;

    //增加判断左右的变量
    bool eyeLeft = false, eyeRight = false, eyeUp = false, eyeDown = false;
    bool eyeStatus = false;
    int blinkCount = 2;
    int sumCount = 30;
    double magSum = 0;
    double xSum = 0;
    double ySum = 0;
    double maxValFrst;
    time_t blinkSec;

    while (1)
    {
        capture >> frame;
        //rotate(frame, frame, ROTATE_90_CLOCKWISE);
        rotate(frame, frame, ROTATE_90_COUNTERCLOCKWISE);
        flip(frame, frame, 1);
        eyeROI = frame(rect_eye);
        resize(eyeROI, eyeROI, Size(50, 25));
        cvtColor(eyeROI, eyeGRAY, CV_BGR2GRAY);

        if (sumCount > 0)
        {
            maxValFrst = frstVal(eyeGRAY, eye_radii, grayThreshold);
            Point loc = findEyeCenter(eyeGRAY);
            sumCount--;
            //只跑一次作为x和y的初始值,x和y变动不大,且为整数,求平均值意义不大
            /*if (sumCount == 10)
            {
                Point loc = findEyeCenter(eyeGRAY);
                xSum = loc.x;
                ySum = loc.y;
            }*/

            //Drop the first val
            if (sumCount == 29)
            {
                continue;
            }

            magSum += maxValFrst;
            xSum += loc.x;
            ySum += loc.y;
            //cout << xSum << ySum << magSum << endl;

            if (sumCount == 0)
            {
                magSum /= 29;
                xSum /= 29;
                ySum /= 29;
                init();
                recvdata = tcp.Recv();
                // Client 异常断开连接
                if (recvdata == "0")
                {
                    tcp.disconnect();
                    capture.release();
                    cout << "line 289 continue" << endl;
                    return 1;
                }
                cout << "Magnitudde Average: " << magSum << endl;
                cout << "X Average: " << xSum << endl;
                cout << "Y Average: " << ySum << endl;
            }
            continue;
        }
        maxValFrst = frstVal(eyeGRAY, eye_radii, grayThreshold);
        //值小于min百分比*平均值,并且眼状态为true,则为 睁眼->闭眼 状态转变
        if ((maxValFrst < magSum * minPer) && (eyeStatus))
        {
            blinkCount -= 1;
            cout << "blink frist time" << endl;
            if (blinkCount == 0)
            {
                if (time(0) - blinkSec < 2) //据上一次眨眼不超过两秒,为确认操作
                {
                    cout << "comfirm blink" << endl;
                    ensure();
                    recvdata = tcp.Recv();
                    if (recvdata == "0")
                    {
                        tcp.disconnect();
                        capture.release();
                        cout << "line 313 continue" << endl;
                        return 1;
                    }
                }
                blinkCount = 2;
            }
            else
            {
                blinkSec = time(0); //此时刻眨眼的秒数
            }
            eyeStatus = false;
            continue;
        }
        //值大于max百分比*平均值,则断定 为睁眼,定位眼球中心
        else if (maxValFrst > magSum * maxPer)
        {
            Point loc = findEyeCenter(eyeGRAY);
            eyeStatus = true;
            circle(eyeROI, loc, 3, Scalar(0, 255, 0), -1, 8);
            //cout <<  loc  << endl;
            // 有眼睛,不在左,不在右,不在上,不在下
            if (!eyeRight && !eyeLeft && !eyeUp && !eyeDown)
            {
                if (loc.x - xSum > 4)
                {
                    cout << "eye right " << endl;
                    blinkCount = 2; //眨眼次数重置
                    eyeRight = true;
                    right();
                    //eyeLeft = true;
                    //left();
                    recvdata = tcp.Recv();
                    if (recvdata == "0")
                    {
                        tcp.disconnect();
                        capture.release();
                        cout << "line 340 continue" << endl;
                        return 1;
                    }
                }
                else if (loc.x - xSum < -6)
                {
                    cout << "eye left " << endl;
                    blinkCount = 2;
                    eyeLeft = true;
                    left();
                    //eyeRight = true;
                    //right();
                    recvdata = tcp.Recv();
                    if (recvdata == "0")
                    {
                        tcp.disconnect();
                        capture.release();
                        cout << "line 354 continue" << endl;
                        return 1;
                    }
                }
                else if (loc.y - ySum > 3.99)
                {
                    cout << "eye down " << endl;
                    blinkCount = 2;
                    eyeDown = true;
                    down();
                    recvdata = tcp.Recv();
                    if (recvdata == "0")
                    {
                        tcp.disconnect();
                        capture.release();
                        cout << "line 304 continue" << endl;
                        return 1;
                    }
                }
                else if (loc.y - ySum < -3.99)
                {
                    cout << "eye up " << endl;
                    blinkCount = 2;
                    eyeUp = true;
                    up();
                    recvdata = tcp.Recv();
                    if (recvdata == "0")
                    {
                        tcp.disconnect();
                        capture.release();
                        cout << "line 304 continue" << endl;
                        return 1;
                    }
                }
            }
            else if ((loc.x - xSum > -4) && (loc.x - xSum < 5) && (loc.y - ySum > -3) && (loc.y - ySum < 3))
            {
                cout << "eye on center" << endl;
                eyeLeft = false;
                eyeRight = false;
                eyeUp = false;
                eyeDown = false;
                center();
                recvdata = tcp.Recv();
                if (recvdata == "0")
                {
                    tcp.disconnect();
                    capture.release();
                    cout << "line 401 continue" << endl;
                    return 1;
                }
            }
        }
        //发送眼部图片
        imencode(".jpg", eyeROI, frame_encode);
        send_img(frame_encode);
        recvdata = tcp.Recv();
        if (recvdata == "0")
        {
            tcp.disconnect();
            capture.release();
            return 1;
        }
    }
    return 0;
}

int main(int argc, char *argv[])
{
    signal(SIGINT, &handle_ctrl_c);
    check_arg(argc, argv);
    ip_init();

    tcp.setup(local_ip, port);

    int ret;
    while (1)
    {
        client_ip = tcp.receive();
        cout << "Client:" << client_ip << endl;
        isinited = face_init();
        if (isinited)
        {
            ret = loop();
            cout << "ret:" << ret << endl;
        }
    }
    tcp.detach();
    cout << "tcp.detach" << endl;
    return 0;
}
