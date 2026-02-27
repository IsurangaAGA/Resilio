import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function LandingPage() {
  const nav = useNavigate();
  const { setRole } = useAuth();

  return (
    <div style={{ maxWidth: 420, margin: "48px auto", padding: 16 }}>
      <h1>Resilio</h1>
      <p style={{ opacity: 0.85 }}>
        Choose the option that matches you. We’ll keep it fast and simple.
      </p>

      <button
        style={btnStyle("green")}
        onClick={() => {
          setRole("Victim");
          nav("/auth/start");
        }}
      >
        🟢 I Need Help
      </button>

      <button
        style={btnStyle("dodgerblue")}
        onClick={() => {
          setRole("Volunteer");
          nav("/auth/start");
        }}
      >
        🔵 I Want to Help
      </button>
    </div>
  );
}

function btnStyle(color) {
  return {
    width: "100%",
    padding: "14px 16px",
    marginTop: 12,
    borderRadius: 10,
    border: "1px solid #ddd",
    background: color,
    color: "white",
    fontSize: 16,
    cursor: "pointer",
  };
}