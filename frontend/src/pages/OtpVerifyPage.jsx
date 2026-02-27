import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function OtpVerifyPage() {
  const nav = useNavigate();
  const { role, identifier, verifyOtp } = useAuth();

  const [otp, setOtp] = useState("");
  const [firstName, setFirstName] = useState("");
  const [fullName, setFullName] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const onVerify = async () => {
    setError(null);
    setLoading(true);
    try {
      await verifyOtp(
        otp,
        role === "Victim" ? firstName : null,
        role === "Volunteer" ? fullName : null
      );

      if (role === "Victim") nav("/victim");
      else nav("/volunteer");
    } catch (e) {
      setError(e?.message || "Verification failed.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: 420, margin: "48px auto", padding: 16 }}>
      <h2>Verify Code</h2>
      <p style={{ opacity: 0.85 }}>
        We sent a code to: <b>{identifier}</b>
      </p>

      {role === "Victim" && (
        <>
          <label style={{ display: "block", marginTop: 12 }}>First name / Nickname</label>
          <input value={firstName} onChange={(e) => setFirstName(e.target.value)} style={inputStyle} />
        </>
      )}

      {role === "Volunteer" && (
        <>
          <label style={{ display: "block", marginTop: 12 }}>Full name</label>
          <input value={fullName} onChange={(e) => setFullName(e.target.value)} style={inputStyle} />
        </>
      )}

      <label style={{ display: "block", marginTop: 12 }}>OTP Code</label>
      <input
        value={otp}
        onChange={(e) => setOtp(e.target.value)}
        placeholder="6-digit code"
        inputMode="numeric"
        style={inputStyle}
      />

      {error && <p style={{ color: "crimson", marginTop: 12 }}>{error}</p>}

      <button disabled={loading} onClick={onVerify} style={primaryBtn}>
        {loading ? "Verifying..." : "Verify"}
      </button>

      <button onClick={() => nav("/auth/start")} style={secondaryBtn}>
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