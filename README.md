# KrakenDB 🐙

> A custom-built relational database storage and execution engine written entirely in pure C#. 

## About KrakenDB

It does not use text files. Instead, it implements its own custom binary file system (KFS), directly managing disk I/O, byte serialization, and memory allocation to achieve hyper-fast, random-access data retrieval. 

It also features a custom-built SQL Compiler (Lexer/Parser) to interpret raw SQL commands over TCP.

## Architecture

The codebase is strictly separated into specialized engines:

```text
src/
└── KrakenDB.Server/
    ├── Storage/          # The Physical Engine (DiskManager, Page anatomy)
    ├── Network/          # The Network Engine (KrakenHost TCP Listener, Models)
    ├── Parsing/          # The SQL Compiler (Lexer, Tokenizer, Strategy Parsers)
    ├── Execution/        # The Query Engine (Command Pattern Executors)
    └── Program.cs        # Server Startup
```
# How it Works (The Lifecycle)

- **Network Layer:** `KrakenHost` listens on port 5432 for incoming TCP connections.

- **Parsing:** Receives a raw SQL string (e.g., `INSERT INTO Usuarios VALUES ('Matheus') `). A custom Lexer tokenizes the string, and a Strategy-based Parser converts it into an executable `KrakenCommand`.

- **Execution & Storage:** - The `QueryEngine` routes the command to the appropriate Executor.
    
    - The engine queries the `DiskManager` to seek the correct physical page using an O(1) mathematical offset (PageId * 8192).

    - If the page has space, it serializes the data into bytes and writes it to the disk.

    - If the page is full, it calculates the End of File, allocates a new 8KB block, updates the old page's pointer, and saves both.

 - **Response:** A JSON result is dispatched back to the client.


# Getting Started

To run the database server:

- Clone the repository and navigate to the project root

```bash
dotnet run --project src/KrakenDB.Server
```

- To test the database without an ORM, you can send raw SQL queries via nc (Netcat) from another terminal:

```bash
# Insert a record
echo "INSERT INTO Usuarios VALUES ('Matheus')" | nc localhost 5432

# Read records
echo "SELECT * FROM Usuarios" | nc localhost 5432
```
