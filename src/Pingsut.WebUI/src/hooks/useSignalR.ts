import { useEffect, useState } from "react";
import { connection } from "../lib/signalr";

export function useSignalR() {
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const start = async () => {
      try {
        if (connection.state === "Disconnected") {
          await connection.start();
          setIsConnected(true);
        }
      } catch (error) {
        console.error(error);
      }
    };

    start();

    return () => {
      connection.stop();
    };
  }, []);

  return {
    connection,
    isConnected,
  };
}
