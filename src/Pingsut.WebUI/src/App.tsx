import React, { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export default function App() {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [status, setStatus] = useState("Disconnected");

    // Step 2: Build
    useEffect(() => {
        const conn = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5153/nonTransitiveHub")
            .withAutomaticReconnect()
            .build();
        setConnection(conn);
    }, []);

    // Step 3: Start & Listen
    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    setStatus("Connected!");

                    connection.on("RoomUpdated", (data) => {
                        alert(`Room updated! Data: ${JSON.stringify(data)}`);
                    });
                })
                .catch(e => setStatus(`Failed: ${e}`));
        }
    }, [connection]);

    // Step 4: Talk
    const handleCreateRoom = async () => {
        if (connection) {
            try {
                // The C# NonTransitiveCommand requires Actions and Rules.
                const command = {
                    actions: [],
                    rules: []
                };
                const newRoomId = await connection.invoke("CreateRoom", command);
                alert(`Created Room: ${newRoomId}`);
            } catch (err) {
                alert(`Error creating room: ${err}`);
                console.error(err);
            }
        } else {
            alert("Connection not ready yet");
        }
    };

    // Step 5: Render
    return (
        <div style={{ padding: '2rem', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h2>SignalR Status: {status}</h2>

            <button
                onClick={handleCreateRoom}
                style={{ padding: '10px', background: 'blue', color: 'white', border: 'none' }}
            >
                Test Create Room
            </button>
        </div>
    );
}
