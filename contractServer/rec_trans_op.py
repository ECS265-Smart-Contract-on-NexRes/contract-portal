import socket
import json
import sqlite3
import time
import base64

from Crypto.Hash import SHA256
from Crypto.Signature import PKCS1_v1_5
from Crypto.PublicKey import RSA


# check signature
def verifying(key, signature, msg):
    signature = base64.b64decode(signature)  # base64 encoding
    data = msg.encode("utf-8")
    rsa_key = RSA.importKey(key)
    sign = PKCS1_v1_5.new(rsa_key)
    sha_data = SHA256.new(data)  
    result = sign.verify(sha_data, signature)
    return result


def split_function(filepath):  
    f = open(filepath, 'r')
    lines = f.readlines()
    f.close()
    flag = -1  
    startinit = 0
    init = {}
    function_list = {}
    for line in lines:
        text = line.strip() 
        if len(text) > 0 and text != "\n":
            # phrase init
            if text.split()[0] == "contract":
                startinit = 1
            # phrase function
            elif text.split()[0] == "function":
                name = text.split()[1]
                name = name.split('(')[0]
                function_list[name] = []
                startinit = 0
                flag += 1
            elif len(function_list) > 0 and text != "}":
                func_detail = text.strip(';')
                function_list[name].append(func_detail)
            elif startinit:
                init_line = text.strip(';')
                init_line = init_line.split()
                init[init_line[1]] = init_line[0]
    return init, function_list


def try_transaction(user_id, func, input_para, func_list, init_para,
                    contract_id, check_para, signature):
    # get init para
    for i in init_para:
        cur.execute("SELECT data,pub_key FROM paramater where para_name='" +
                    i + "' and user_id ='" + user_id + "'")
        tmp = cur.fetchall()
        data, pub_key = tmp[0][0], tmp[0][1].encode()
        init_para[i] = data
    op_time = time.time()
    operation = func
    Nonempty, success = True, False
    print(init_para, type(check_para))
    check_para = json.dumps(check_para, separators=(',', ':'))
    print(pub_key, signature, check_para)
    if not (verifying(pub_key, signature, check_para)):
        return {'success': False, 'ret': "Signature verification failed!"}
    if func in func_list:
        if init_para['vaultData'] == '':
            Nonempty = False
        if func == "set":
            init_para['vaultData'] = str(input_para['data'])
            success = True
        elif Nonempty and func == "add" and input_para['data'] >= 0:
            print(init_para['vaultData'])
            init_para['vaultData'] = str(
                int(init_para['vaultData']) + input_para['data'])
            success = True
        elif Nonempty and func == "minus" and int(
                init_para['vaultData']) >= input_para['data']:
            init_para['vaultData'] = str(
                int(init_para['vaultData']) - input_para['data'])
            success = True
        elif Nonempty and func == "get":
            init_para['vaultData'] = init_para['vaultData']
            success = True
    if success:
        cur.execute("UPDATE paramater SET data='" +
                    str(init_para['vaultData']) +
                    "' WHERE para_name='vaultData' and user_id='" +
                    str(user_id) + "'")
    cur.execute("INSERT INTO transcation VALUES ('" + str(user_id) + "','" +
                str(contract_id) + "','" + str(op_time) + "','" + operation +
                "','" + str(success) + "')")
    if func == "get":
        return {'success': True, 'ret': init_para['vaultData'] }
    else:
        return {success: True}


server = socket.socket()

server.bind(("", 6900))  

server.listen(5)  # Listen to port

print("Listen to port..")

while True:
    conn, addr = server.accept()  # Wait for connection

    print("conn:", conn, "\naddr:", addr)  

    while True:
        data = conn.recv(4096)  
        if not data:  
            print("Disconnect")
            break
        
        decodedData = data.decode("utf-8")
        data = json.loads(decodedData)
        
        print
        user_id, sol_file, func_name, para, contract_id, signature = \
            data['input']['userId'], \
            data['input']['contractId'], \
            data['input']['transactionType'], \
            data['input']['params'], \
            data['input']['transactionId'], \
            data['signature']

        print("Receive command：", data, type(data))
        sol_file = "test.sol"

        db_con = sqlite3.connect("transcation.db")
        cur = db_con.cursor()

        init_para, func_list = split_function(sol_file)
        # analysis function input para
        f = open("./" + sol_file.strip(".sol") + '.json')
        compiled_sol = json.load(f)
        func_input = compiled_sol["contracts"]["SimpleStorage.sol"]["Vault"][
            "abi"]

        input_para = {}
        for func in func_input:
            if func['name'] == func_name:
                for inpu in range(len(func['inputs'])):
                    input_para[func['inputs'][inpu]['name']] = para[inpu]

        result = try_transaction(user_id, func_name, input_para, func_list, init_para,
                        contract_id,
                        data['input'], data['signature'])
        db_con.commit()
        cur.close()
        conn.sendall(bytes(json.dumps(result).encode("utf-8")))
server.close()
