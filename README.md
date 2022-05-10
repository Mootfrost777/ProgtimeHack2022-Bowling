# ProgtimeHack2022_Bowling
Bowling game writeen in ProgTime 2022 hackathon/ 

# Server setup
Install dotnet: <br/>
Windows:  https://dotnet.microsoft.com/en-us/download <br/>
Linux: https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu <br/>

Compile and run server: <br/>
```
cd Bowling_Server
dotnet run
```

In the console that opens, enter server IP (``0.0.0.0`` if you want to listen from all available IP) and the port(ex. 7632) on which the game server will be launched.

The server must have as least one static IP.

# Game
Compile the game for the desired operating system and send it to users. At startup, enter the IP and port on which the server is running. <br/>
If there are enough players(2), the game will start **immediately**.
