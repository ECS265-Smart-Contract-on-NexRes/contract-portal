import socket
import json
import hashlib
import sys

client = socket.socket()  # 生成socket连接对象

ip_port = ("34.94.118.219", 6900)  # 地址和端口号

client.connect(ip_port)  # 连接

print("服务器已连接")

sol_file_name = "test.sol"
func_name, para, contract_id = 'add', sys.argv[1], 1001

input_para = [sol_file_name, func_name, para, contract_id]
input_para = json.dumps(input_para)
# if len(select_para) == 0: continue  # 如果传入空字符会阻塞

client.send(bytes(input_para.encode("utf-8")))

# 2.发送文件内容
# data = json.loads(client.recv(1024))  # 接收确认
# print("收到的：", data)

client.close()