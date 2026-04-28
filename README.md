# KrakenDB 🐙

> A custom-built relational database storage and execution engine written entirely in pure C#. 

## About KrakenDB

While most modern backend developers rely on high-level abstractions like SQL Server or PostgreSQL, KrakenDB was built from the ground up to demonstrate a deep understanding of low-level systems engineering. 

It does not use text files. Instead, it implements its own custom binary file system (KFS), directly managing disk I/O, byte serialization, and memory allocation to achieve hyper-fast, random-access data retrieval.

## Architecture

The codebase is strictly separated into two main engines:

```text
src/
└── KrakenDB.Server/
    ├── Storage/          # The Physical Engine (DiskManager, Page anatomy)
    ├── Network/          # The Network Engine (KrakenHost TCP Listener, JSON Models)
    └── Program.cs        # The Execution Engine (Binds Network and Storage)
```
# How it Works (The Lifecycle)

- Network Layer: KrakenHost listens on port 5432 for incoming TCP connections.

- **Parsing:** Receives a JSON payload (e.g., `{"Action": "INSERT", "Table": "Users", "Data": "Matheus"}`).

- **Execution & Storage:** - The engine queries the DiskManager to seek the correct physical page using an O(1) mathematical offset (PageId * 8192). 
    - If the page has space, it serializes the data into bytes and writes it to the disk.
    - If the page is full, it calculates the End of File, allocates a new 8KB block, updates the old page's pointer, and saves both.

 - **Response:** A JSON result is dispatched back to the client.


# Getting Started

To run the database server:

- Clone the repository and navigate to the project root

```bash
dotnet run --project src/KrakenDB.Server
```

- To test the database without an ORM, you can send a raw JSON packet via nc (Netcat) from another terminal:

```bash
# Insert a record
echo '{"Action":"INSERT","Table":"Usuarios","Data":"Matheus"}' | nc localhost 5432

# Read records
echo '{"Action":"SELECT","Table":"Usuarios"}' | nc localhost 5432
```
