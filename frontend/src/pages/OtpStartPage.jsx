import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import styles from "./SecureAccessPage.module.css";
import helpImg from "../assets/help.png"; 
import AppHeader from "../components/AppHeader/AppHeader";


export default function SecureAccessPage() {
  const nav = useNavigate();
  const { role, startOtp } = useAuth();

  // UI state (same as your new design)
  const [contactMethod, setContactMethod] = useState("phone"); // "phone" | "email"
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [country, setCountry] = useState("LK");

  // Functional state (from old page)
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Map UI -> backend channel + identifier
  const channel = contactMethod === "email" ? "EMAIL" : "SMS";
  const identifier = contactMethod === "email" ? email : phone;

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await startOtp(identifier, channel);
      nav("/auth/verify");
    } catch (err) {
      setError(err?.message || "Failed to send code.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <AppHeader />
    <div className={styles.page}>
      <div className={styles.shell}>
        <div className={styles.layout}>
          {/* LEFT PANEL (unchanged UI) */}
          <section className={styles.left}>
            <div className={styles.leftCard}>
              <div className={styles.leftFigure}>
                <img
                  src={helpImg}
                  alt="Disaster response support"
                  className={styles.leftImage}
                />
              </div>

              <p className={styles.leftCaption}>
                Stay connected during emergencies—verify your access to request help,
                coordinate responders, and receive timely disaster alerts.
              </p>
            </div>
          </section>

          {/* RIGHT PANEL (unchanged UI) */}
          <section className={styles.right}>
            <div className={styles.card}>
              <div className={styles.cardTop}>
                <div className={styles.logo} aria-hidden="true">
                  <div className={styles.logoShield}>
                    <div className={styles.logoHands} />
                    <div className={styles.logoHeart} />
                  </div>
                </div>

                {/* Keep your title/subtitle styles, but show role-based heading like old page */}
                <h2 className={styles.cardTitle}>
                  {role === "Victim" ? "Welcome" : " Welcome to Volunteer Access"}
                </h2>

                <p className={styles.cardSubtitle}>
                  Enter your phone number or email to receive a 6-digit
                  <br />
                  access code (OTP)
                </p>
              </div>

              <div className={styles.divider} />

              <form className={styles.form} onSubmit={handleSubmit}>
                <div className={styles.sectionLabel}>Contact Method</div>

                <div className={styles.segment} role="tablist" aria-label="Contact method">
                  <button
                    type="button"
                    className={`${styles.segmentBtn} ${
                      contactMethod === "phone" ? styles.segmentActive : ""
                    }`}
                    onClick={() => {
                      setContactMethod("phone");
                      setError(null);
                    }}
                    role="tab"
                    aria-selected={contactMethod === "phone"}
                  >
                    Phone Number
                  </button>

                  <button
                    type="button"
                    className={`${styles.segmentBtn} ${
                      contactMethod === "email" ? styles.segmentActive : ""
                    }`}
                    onClick={() => {
                      setContactMethod("email");
                      setError(null);
                    }}
                    role="tab"
                    aria-selected={contactMethod === "email"}
                  >
                    Email
                  </button>
                </div>

                {/* Conditional field: ONLY ONE renders (unchanged UI) */}
                {contactMethod === "email" ? (
                  <div className={styles.field}>
                    <label className={styles.label}>Email Address</label>
                    <div className={styles.inputGroup}>
                      <select
                        className={styles.country}
                        value={country}
                        onChange={(e) => setCountry(e.target.value)}
                        aria-label="Country"
                      >
                        <option value="US">🇺🇸</option>
                        <option value="LK">🇱🇰</option>
                        <option value="IN">🇮🇳</option>
                        <option value="GB">🇬🇧</option>
                      </select>

                      <input
                        className={styles.input}
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="name@example.com"
                        required
                      />

                      <span className={styles.trailingIcon} aria-hidden="true">
                        i
                      </span>
                    </div>
                  </div>
                ) : (
                  <div className={styles.field}>
                    <label className={styles.label}>Phone Number</label>
                    <div className={styles.inputGroup}>
                      <select
                        className={styles.country}
                        value={country}
                        onChange={(e) => setCountry(e.target.value)}
                        aria-label="Country"
                      >
                        <option value="US">🇺🇸</option>
                        <option value="LK">🇱🇰</option>
                        <option value="IN">🇮🇳</option>
                        <option value="GB">🇬🇧</option>
                      </select>

                      <input
                        className={styles.input}
                        type="tel"
                        value={phone}
                        onChange={(e) => setPhone(e.target.value)}
                        placeholder="(94) 123 569 756"
                        required
                      />

                      <span className={styles.trailingIcon} aria-hidden="true">
                        i
                      </span>
                    </div>
                  </div>
                )}

                {/* Error message (no new styles added) */}
                {error && (
                  <p style={{ color: "crimson", marginTop: 6, marginBottom: 0 }}>
                    {error}
                  </p>
                )}

                <button className={styles.primaryBtn} type="submit" disabled={loading}>
                  {loading ? "SENDING..." : "SEND ACCESS CODE"}
                </button>

                <p className={styles.disclaimer}>
                  By proceeding, you agree to receive a single-use code for authentication
                  purposes. Standard rates may apply.
                </p>

                {/* Back button functionality from old page (no CSS changes) */}
                <button
                  type="button"
                  onClick={() => nav("/")}
                  style={{
                    width: "100%",
                    padding: "12px 12px",
                    borderRadius: 10,
                    border: "1px solid #ddd",
                    marginTop: 10,
                    background: "#f4f4f4",
                    cursor: "pointer",
                  }}
                >
                  Back
                </button>
              </form>
            </div>
          </section>
        </div>
      </div>
    </div>

  </>
  );
}