import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function VictimDashboard() {
  const nav = useNavigate();
  const { logout, role } = useAuth();

  return (
    <div style={{ maxWidth: 560, margin: "48px auto", padding: 16 }}>
      <h2>Victim Dashboard</h2>
      <p>✅ Logged in as: <b>{role}</b></p>

      <button style={primaryBtn} onClick={() => nav('/victim/new-request')}>
      Request Help
      </button>

      <button
        style={secondaryBtn}
        onClick={async () => {
          await logout();
          nav("/");
        }}
      >
        Logout
      </button>
    </div>
  );
}

const primaryBtn = {
  width: "100%",
  padding: "12px 12px",
  borderRadius: 10,
  border: "1px solid #ddd",
  marginTop: 16,
  background: "black",
  color: "white",
  cursor: "pointer",
};

const secondaryBtn = {
  width: "100%",
  padding: "12px 12px",
  borderRadius: 10,
  border: "1px solid #ddd",
  marginTop: 10,
  background: "#f4f4f4",
  cursor: "pointer",
};