# Riptide — A BitTorrent Client Built from Scratch

<img width="728" height="400" alt="Riptide Terminal Output" src="https://via.placeholder.com/728x400.png?text=Riptide+Screenshot+Here" />

## About The Project
Riptide is a fully functional BitTorrent client built entirely from scratch in C#. 

This project was developed as a deep dive into the technical intricacies of decentralized peer-to-peer networking. Rather than relying on external torrenting libraries, Riptide implements the core BitTorrent specification from the ground up. It handles everything from parsing custom data serializations to managing raw TCP sockets, cryptographic hashing, and concurrent file I/O.

## Core Features
* **Custom BEncoding Engine:** Includes a bespoke recursive descent parser that decodes the BitTorrent protocol's native serialization format into strictly typed C# data structures.
* **Torrent File Management:** Automatically extracts metadata, calculates the 20-byte SHA1 InfoHash, and slices piece hashes for data verification.
* **Tracker Communication:** Connects to HTTP/UDP trackers, safely URL-encodes binary hashes, and parses compact peer lists to discover available seeders and leechers in the swarm.
* **The Peer Protocol:** Manages active TCP connections and handshakes, handling the constant stream of binary state messages (Choke, Unchoke, Interested, Have, Request, Piece).
* **Piece Assembly & File I/O:** Safely downloads multiple file chunks concurrently, verifies their integrity against the original SHA1 hashes, and writes the completed blocks to disk.
* **Azureus-Style Identification:** Generates a compliant, randomized 20-byte Peer ID (using the `-RT0001-` prefix) to uniquely identify the client to the network.

## Architecture
```text
riptide/
    Bencode.cs          # Recursive descent parser for BEncoded data
    Torrent.cs          # File blueprint, metadata extraction, and SHA1 hashing
    ClientConfig.cs     # Static configuration and Azureus-style Peer ID generation
    Tracker.cs          # HTTP client for announcing to trackers and parsing peer IPs
    Peer.cs             # TCP socket management and peer-to-peer message protocol
    ClientEngine.cs     # Connection pooling, concurrent piece requests, and File I/O
```

## How It Works
1. **Initialization:** The client reads a `.torrent` file and passes it to `Bencode.cs`, which translates the raw bytes into a readable dictionary.
2. **Metadata & Hashing:** `Torrent.cs` extracts the file size, piece length, and the raw binary hashes. It then hashes the original `info` dictionary using SHA1 to create the torrent's unique ID.
3. **Announcing:** Riptide URL-encodes its InfoHash and Peer ID, sending an HTTP GET request to the tracker. The tracker responds with a list of IP addresses of other users downloading the same file.
4. **The Swarm:** Riptide opens TCP sockets to those IP addresses, performs the BitTorrent handshake, and begins exchanging "Interested" and "Request" messages to download missing pieces. 
5. **Verification:** As raw bytes stream in, they are hashed and checked against the original torrent metadata. Valid pieces are saved to the hard drive.

## Setup & Installation
Requires the .NET SDK to be installed.

1. Clone the repository:
   ```bash
   git clone [https://github.com/your-username/riptide.git](https://github.com/your-username/riptide.git)
   ```
2. Navigate to the project directory:
   ```bash
   cd riptide
   ```
3. Run the client, passing a `.torrent` file as an argument:
   ```bash
   dotnet run -- "path/to/debian-iso.torrent"
   ```

## Tech Stack
* **C# (.NET)**
* **System.Security.Cryptography** (SHA1 Hash Generation)
* **System.Net.Sockets** (TCP Peer Connections)
* **System.Net.Http** (Tracker Announce Requests)
* **System.IO** (Binary Stream Reading & Disk Writing)
