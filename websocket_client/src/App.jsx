import React, { useEffect, useRef } from "react";

function App() {
  const socketRef = useRef(null);

  useEffect(() => {
    socketRef.current = new WebSocket("ws://10.120.5.124:3000");

    return () => {
      socketRef.current.close();
    };
  }, []);

  const send = (color) => {
    socketRef.current.send(color);
  };

  return (
    <div>
      <button onClick={() => send("red")}>Red</button>
      <button onClick={() => send("green")}>Green</button>
      <button onClick={() => send("blue")}>Blue</button>
    </div>
  );
}

export default App;
