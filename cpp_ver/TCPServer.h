#ifndef TCP_SERVER_H
#define TCP_SERVER_H

#include <iostream>
#include <vector>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <string.h>
#include <arpa/inet.h>

using namespace std;

// 接收数据包大小
#define MAXPACKETSIZE 1

class TCPServer
{
  public:
	int sockfd, newsockfd, n, pid;
	struct sockaddr_in serverAddress;
	struct sockaddr_in clientAddress;
	char msg[MAXPACKETSIZE];
	static string Message;

	void setup(char local_ip[], int port);
	string receive();
	string Recv();
	void Send(string msg);
	void detach();
	void clean();
	void disconnect();
};

#endif
