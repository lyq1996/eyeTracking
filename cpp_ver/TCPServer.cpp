#include "TCPServer.h"

string TCPServer::Message;

// 初始化tcp连接
void TCPServer::setup(char local_ip[], int port)
{
	cout << "Starting a TCP server.." << endl;
	sockfd = socket(AF_INET, SOCK_STREAM, 0);
	memset(&serverAddress, 0, sizeof(serverAddress));
	serverAddress.sin_family = AF_INET;
	serverAddress.sin_addr.s_addr = inet_addr(local_ip);
	serverAddress.sin_port = htons(port);
	bind(sockfd, (struct sockaddr *)&serverAddress, sizeof(serverAddress));
	int opt = 1;  
    setsockopt( sockfd, SOL_SOCKET,SO_REUSEADDR,(const void *)&opt, sizeof(opt) );
	listen(sockfd, 1);
	cout << "TCP server listening in " << local_ip << ":" << port << endl;
}

// 阻塞式接受连接,返回Client的IP地址
string TCPServer::receive()
{
	string str;
	socklen_t sosize = sizeof(clientAddress);
	newsockfd = accept(sockfd, (struct sockaddr *)&clientAddress, &sosize);
	str = inet_ntoa(clientAddress.sin_addr);
	return str;
}

//阻塞式接受数据
string TCPServer::Recv()
{
	int n;
	char msg[MAXPACKETSIZE + 1];

	n = recv(newsockfd, msg, MAXPACKETSIZE, 0);
	if (n == 0 || n == -1)
	{
		//close(newsockfd);
		cout << "Client Disconnect!" << endl;
		Message = "0";
		return Message;
	}
	msg[MAXPACKETSIZE] = '\0';
	Message = string(msg);
	//cout << "Client Message: " << Message << endl;
	return Message;
}

//发送
void TCPServer::Send(string msgsend)
{
	//cout << "Send Data Length: " << msgsend.length() << endl;
	send(newsockfd, msgsend.c_str(), msgsend.length(), 0);
	msgsend = "";
}

//清理msg和Message,可以清但感觉没必要
void TCPServer::clean()
{
	Message = "";
	memset(msg, 0, MAXPACKETSIZE);
}

//断开连接
void TCPServer::detach()
{
	close(newsockfd);
	close(sockfd);
}

//断开客户端连接
void TCPServer::disconnect()
{
	close(newsockfd);
}
