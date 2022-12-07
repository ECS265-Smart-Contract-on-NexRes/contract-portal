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
    signature = base64.b64decode(signature)  # base64解码
    data = msg.encode("utf-8")
    rsa_key = RSA.importKey(key)
    sign = PKCS1_v1_5.new(rsa_key)
    sha_data = SHA256.new(data)  # 与签名时使用的算法相同
    result = sign.verify(sha_data, signature)
    return result


def split_function(filepath):  # filepath:输入需要处理的文件路径
    f = open(filepath, 'r')
    lines = f.readlines()
    f.close()
    flag = -1  # 作为二维数组下标使用
    startinit = 0
    init = {}
    function_list = {}
    for line in lines:
        text = line.strip()  # strip是trim掉字符串两边的空格。
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
    check_para = json.dumps(check_para)
    print(pub_key, signature, check_para)
    if verifying(pub_key, signature, check_para) and func in func_list:
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


server = socket.socket()

server.bind(("", 6900))  # 绑定监听端口

server.listen(5)  # 监听

print("监听开始..")

while True:
    conn, addr = server.accept()  # 等待连接

    print("conn:", conn, "\naddr:", addr)  # conn连接实例

    while True:
        data = conn.recv(4096)  # 接收
        if not data:  # 客户端已断开
            print("客户端断开连接")
            break

        data = json.loads(data.decode("utf-8"))
        print
        user_id, sol_file, func_name, para, contract_id, signature = data[
            0], data[1], data[2], data[3], data[4], data[5]
        print("收到的命令：", data, type(data))

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

        try_transaction(user_id, func_name, input_para, func_list, init_para,
                        contract_id,
                        [data[0], data[1], data[2], data[3], data[4]], data[5])
        db_con.commit()
        cur.close()
server.close()
