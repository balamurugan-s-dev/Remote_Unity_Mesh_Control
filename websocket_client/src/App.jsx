import React, { useEffect, useRef, useState } from "react";

function App() {
  const socketRef = useRef(null);
  const [xRotation, setXRotation] = useState(0);
  const [yRotation, setYRotation] = useState(0);
  const [zRotation, setZRotation] = useState(0);

  useEffect(() => {
    socketRef.current = new WebSocket("ws://10.120.5.124:3000");

    return () => {
      socketRef.current.close();
    };
  }, []);

  const SendCommand = (data) => {
    if (socketRef.current.readyState === WebSocket.OPEN) {
      socketRef.current.send(JSON.stringify(data));
    }
  }

  const handleXRotation = (x) => {
    const val = parseFloat(x.target.value);
    setXRotation(val);
    SendCommand({
      target: "PlayerCube",
      type: "transform",
      property: "rotation",
      axis: "x",
      axisValue: val
    });
  }

  const handleYRotation = (y) => {
    const val = parseFloat(y.target.value);
    setYRotation(val);
    SendCommand({
      target: "PlayerCube",
      type: "transform",
      property: "rotation",
      axis: "y",
      axisValue: val
    });
  }

  const handleZRotation = (z) => {
    const val = parseFloat(z.target.value);
    setZRotation(val);
    SendCommand({
      target: "PlayerCube",
      type: "transform",
      property: "rotation",
      axis: "z",
      axisValue: val
    });
  }

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'center',
      alignItems: 'center',
      height: '100vh',
    }}>
      <div style={{display: 'flex', flexDirection: 'column', gap: '10px'}}>
        <div style={{marginBottom: '20px'}}><h2>Rotation Controls:</h2></div>

        <div style={{display: 'flex', flexDirection: 'column', width: "300px",gap: '5px'}}>
          <label htmlFor="">Cube Rotation X: {xRotation}</label>
          <input
            type="range"
            min="0"
            max="360"
            value={xRotation}
            style={{ width: "300" }}
            onChange={handleXRotation}
          />
        </div>

        <div style={{display: 'flex', flexDirection: 'column', width: "300px",gap: '5px'}}>
          <label htmlFor="">Cube Rotation Y: {yRotation}</label>
          <input
            type="range"
            min="0"
            max="360"
            value={yRotation}
            style={{ width: "300" }}
            onChange={handleYRotation}
          />
        </div>

        <div style={{display: 'flex', flexDirection: 'column', width: "300px",gap: '5px'}}>
          <label htmlFor="">Cube Rotation Z: {zRotation}</label>
          <input
            type="range"
            min="0"
            max="360"
            value={zRotation}
            style={{ width: "300" }}
            onChange={handleZRotation}
          />
        </div>
      </div>
    </div>
  );
}

export default App;
