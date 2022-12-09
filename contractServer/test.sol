pragma solidity ^0.6.6;
contract Vault {
    uint vaultData;
    function set(uint data) public{
        vaultData = data;
    }
    
    function add(uint data) public{
        vaultData = vaultData + data;
    }
    
    function minus(uint data) public{
        vaultData = vaultData - data;
    }

    function get() public view returns (uint) {
        return vaultData;
    }
}