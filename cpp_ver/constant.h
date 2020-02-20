#ifndef CONSTANT_H
#define CONSTANT_H

using namespace std;
using namespace cv;

char local_ip[0x10];
char dev[0xc];
const int port = 8080;

string client_ip;
string recvdata;
string senddata;

// Face init
bool isinited;

// Eye Region
//const int eye_region_width = 220;
//const int eye_region_height = 100;
const int eye_region_width = 220;
const int eye_region_height = 110;


const int resizex = 50;
const int resizey = 25;
const int camera_flag = 0;

int eye_radii;
float minPer;
float maxPer;
int left_eye_x;
int left_eye_y;
int grayThreshold;

#endif
