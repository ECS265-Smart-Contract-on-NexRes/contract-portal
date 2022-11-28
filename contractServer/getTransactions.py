import socket
import json
import hashlib

client = socket.socket()  # 生成socket连接对象

ip_port = ("34.94.118.219", 6910)  # 地址和端口号

client.connect(ip_port)  # 连接

print("服务器已连接")

select_type = 'transcation'  # or 'getbalance'
contract_id = 1001
para_name = 'vaultData'

select_para = [select_type, contract_id, para_name]
select_para = json.dumps(select_para)
# if len(select_para) == 0: continue  # 如果传入空字符会阻塞

client.send(bytes(select_para.encode("utf-8")))

# 2.发送文件内容
data = json.loads(client.recv(1024))  # 接收确认
print(data)

client.close()