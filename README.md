# SNMP Agent

## Overview
This is a work in progress C# implementation of a Simple Network Management Protocol agent. The main target of this project is to recreate most of the agent's [functions](https://www.manageengine.com/network-monitoring/what-is-snmp.html).
 
## Description of the parts implemented so far
#### MIB-2 file parser
This part is responsible for reading the database structures and type definitions described using text files in ASN.1 and recreating them using .NET objects and types. 
#### MIB tree
All IDs are then recreated in a form of a tree, while the data types are stored in a list. Navigating the tree is possible with methods implemented in the ```MIBTree``` class. Each node of the tree contains fields defining the structure of it's leafs and it's stored values:
```csharp
private TreeNode currentNode;
private Dictionary<int, MIBTree> idDict;
```
#### BER encoder and decoder
A part responsible for encoding and decoding data so that it could be sent in the same form as SNMP through the network. The supported types are: 
* OBJECT IDENTIFIER
* NULL
* INTEGER
* BOOLEAN
* STRING
* SEQUENCE

#### PDU encoder and decoder
The code written for this part is responsible for preparing the final form in which data is sent and received through the network. It uses the BER encoder to handle the types and adds information about the type of request that is being handled.
