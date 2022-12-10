import socket
import json
import base64

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

sol_file_name = "test.sol"
func_name, para, contract_id, user_id = 'add', [50], 1001, '0001'

input_para = [user_id, sol_file_name, func_name, para, contract_id]
input_para = json.dumps(input_para)
private_key = '-----BEGIN RSA PRIVATE KEY-----\nMIICXQIBAAKBgQDiS5F+PgNCzj6iETZ13AlWw23yroAtMeALFmeZGEOg3+WhLU6uKUryWWg+Vzrqowp4F99rAsdXs4EKLpYVzUPRE73c+gWZz9x84qGWaqxK5o+z6VkK7KcbAgLN6zb555kVFq04fafZqkmJadoyXleLYjEqcJ7W6UteWoLs0Qy7YwIDAQABAoGAUG3lZ0YpKIxfTIDrp1YuZ40MPe3xlp6cb7Rl28748mvBlNiil1oLzjkiyM1+HjkWlnp9qO4S5cPiADlwlI0hJbpNFRf8Kqu00CHWseHrfk7yhyUrOoxjSKn6xVw6fq7I50GoqHz9UmnxFBXGisGmUliH01uTHLNqBcIpJ1v+FxECQQDskBUboEg/ArXMwLEMpptTONDIAAA80ZhQSZ5M9rYKbzPPQTypc8+bVCZ9zJ60iJOJ3S27upNl/QBNvdEH6dsLAkEA9OODm0CdR7+AWAryynMaoOGj7XrZ9aLVRf2NwE0MRXH/rB32B6P7vCmJIyyTMElV6eku80/mo1XT9tKyrsEYCQJBAOi8lFe6qHl9lBkeltGodHY7FoU+Iv2zA5Qx6ZE0xEKdxy4ns6PPMbhS4Q+xKY7aM7VWKnFgjTWw5QSXNDkB5aMCQCw1UlHZpUsJiCrctx3TD7CRa114uxY78hJzhn57qkZzIPu6YOraMJy0RtyBtISYCJl0jhRAjVtZKC27taQUmbkCQQCFOFCDlgGoOpNBY763l/12hnaWE7++iLi6D8ysseRE7aXsufvHOVNQiACYDM65MrbM3HwmzpH+uQhmtX/HZ1kl\n-----END RSA PRIVATE KEY-----'.encode(
)
signature = signing(private_key, input_para)

input_para = [user_id, sol_file_name, func_name, para, contract_id, signature]
input_para = json.dumps(input_para)

client.send(bytes(input_para.encode("utf-8")))

client.close()