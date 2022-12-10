import socket
import json
import base64
import sys

from Crypto.Hash import SHA256
from Crypto.Signature import PKCS1_v1_5
from Crypto.PublicKey import RSA


def signing(key, msg):
    data = msg.encode("utf-8") 

    rsa_key = RSA.importKey(key)
    sign = PKCS1_v1_5.new(rsa_key)

    sha_data = SHA256.new(data)  

    sign_data = sign.sign(sha_data) 

    result = base64.b64encode(sign_data) 
    result = result.decode("utf-8")  
    return result


client = socket.socket() 

ip_port = ("localhost", 6900) 

client.connect(ip_port) 

print("Server connected")

private_key = sys.argv[1] 
func_name = sys.argv[2]
para = sys.argv[3]
contract_id = 1001
user_id = sys.argv[4]
user_id_to = None
if len(sys.argv) > 5:
    user_id_to = sys.argv[5]

sol_file_name = "test.sol"
# func_name, para, contract_id, user_id = 'add', [50], 1001, '0001'

input_para = [user_id, sol_file_name, func_name, para, contract_id, user_id_to]
input_para = json.dumps(input_para)
private_key = private_key.encode()

signature = signing(private_key, input_para)

input_para = [user_id, sol_file_name, func_name, para, contract_id, user_id_to, signature]
input_para = json.dumps(input_para)
print(100)
client.send(bytes(input_para.encode("utf-8")))
data = json.loads(client.recv(1024))

client.close()