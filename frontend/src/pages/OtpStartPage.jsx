import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function OtpStartPage() {
  const nav = useNavigate();
  const { role, startOtp } = useAuth();

  const [identifier, setIdentifier] = useState("");
  const [channel, setChannel] = useState("EMAIL");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const onSend = async () => {
    setError(null);
    setLoading(true);
    try {
      await startOtp(identifier, channel);
      nav("/auth/verify");
    } catch (e) {
      setError(e?.message || "Failed to send code.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: 420, margin: "48px auto", padding: 16 }}>
      <h2>{role === "Victim" ? "Quick Access" : "Volunteer Access"}</h2>
      <p style={{ opacity: 0.85 }}>Enter phone or email. We’ll send a verification code.</p>

      <label style={{ display: "block", marginTop: 12 }}>Phone or Email</label>
      <input
        value={identifier}
        onChange={(e) => setIdentifier(e.target.value)}
        placeholder="e.g. test@example.com"
        style={inputStyle}
      />

      <label style={{ display: "block", marginTop: 12 }}>Channel</label>
      <select value={channel} onChange={(e) => setChannel(e.target.value)} style={inputStyle}>
        <option value="EMAIL">EMAIL</option>
        <option value="SMS">SMS</option>
      </select>

      {error && <p style={{ color: "crimson", marginTop: 12 }}>{error}</p>}

      <button disabled={loading} onClick={onSend} style={primaryBtn}>
        {loading ? "Sending..." : "Send Code"}
      </button>

      <button onClick={() => nav("/")} style={secondaryBtn}>
        Back
      </button>
    </div>
  );
}

const inputStyle = {
  width: "100%",
  padding: "12px 12px",
  borderRadius: 8,
  border: "1px solid #ccc",
  marginTop: 6,
};

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