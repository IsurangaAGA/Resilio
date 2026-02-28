import React, { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import styles from "./VerifyCodePage.module.css";
import AppHeader from "../components/AppHeader/AppHeader";

export default function OtpVerifyPage({ initialResendSeconds = 45 }) {
  const nav = useNavigate();
  const { role, identifier, verifyOtp } = useAuth();

  const [firstName, setFirstName] = useState("");
  const [fullName, setFullName] = useState("");

  const [digits, setDigits] = useState(Array(6).fill(""));
  const inputsRef = useRef([]);

  const [secondsLeft, setSecondsLeft] = useState(initialResendSeconds);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const code = useMemo(() => digits.join(""), [digits]);
  const isComplete = useMemo(() => digits.every((d) => d !== ""), [digits]);

  useEffect(() => {
    if (secondsLeft <= 0) return;
    const id = setInterval(() => setSecondsLeft((s) => s - 1), 1000);
    return () => clearInterval(id);
  }, [secondsLeft]);

  useEffect(() => {
    inputsRef.current?.[0]?.focus?.();
  }, []);

  const focusIndex = (i) => {
    const el = inputsRef.current?.[i];
    if (el) {
      el.focus();
      el.select?.();
    }
  };

  const setDigitAt = (index, value) => {
    setDigits((prev) => {
      const next = [...prev];
      next[index] = value;
      return next;
    });
  };

  const handleChange = (index, e) => {
    setError(null);

    const raw = e.target.value;
    const onlyDigits = raw.replace(/\D/g, "");

    if (onlyDigits.length > 1) {
      const spread = onlyDigits.slice(0, 6 - index).split("");
      setDigits((prev) => {
        const next = [...prev];
        for (let i = 0; i < spread.length; i++) next[index + i] = spread[i];
        return next;
      });
      const nextIndex = Math.min(index + onlyDigits.length, 5);
      focusIndex(nextIndex);
      return;
    }

    const val = onlyDigits.slice(-1);
    setDigitAt(index, val);
    if (val && index < 5) focusIndex(index + 1);
  };

  const handleKeyDown = (index, e) => {
    if (e.key === "Backspace") {
      if (digits[index]) {
        setDigitAt(index, "");
        return;
      }
      if (index > 0) {
        setDigitAt(index - 1, "");
        focusIndex(index - 1);
      }
    }

    if (e.key === "ArrowLeft" && index > 0) focusIndex(index - 1);
    if (e.key === "ArrowRight" && index < 5) focusIndex(index + 1);
    if (e.key === "Enter" && !loading) handleVerify();
  };

  const handlePaste = (index, e) => {
    setError(null);

    const pasted = (e.clipboardData?.getData("text") || "").replace(/\D/g, "");
    if (!pasted) return;
    e.preventDefault();

    const spread = pasted.slice(0, 6 - index).split("");
    setDigits((prev) => {
      const next = [...prev];
      for (let i = 0; i < spread.length; i++) next[index + i] = spread[i];
      return next;
    });

    const nextIndex = Math.min(index + spread.length - 1, 5);
    focusIndex(nextIndex);
  };

  // ✅ Old page behavior: button works anytime; shows error if OTP incomplete
  const handleVerify = async () => {
    if (loading) return;

    setError(null);

    if (!isComplete) {
      setError("Please enter the 6-digit code.");
      return;
    }

    setLoading(true);
    try {
      await verifyOtp(
        code,
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

  const handleResend = () => {
    if (secondsLeft > 0 || loading) return;

    setSecondsLeft(initialResendSeconds);
    setDigits(Array(6).fill(""));
    setError(null);
    focusIndex(0);

    // same behavior as your old flow (start again)
    nav("/auth/start");
  };

  const formatSeconds = (s) => String(Math.max(0, s)).padStart(2, "0");

  return (
    <>
      <AppHeader />
    <div className={styles.verifyPage}>
      <div className={styles.card}>
        <h1 className={styles.title}>VERIFY YOUR CODE</h1>

        <p className={styles.subtitle}>
          A 6-digit verification code has been sent to your registered contact.
          <br />
          <span className={styles.phone}>(e.g., {identifier})</span>
        </p>

        {role === "Victim" && (
          <div style={{ width: "100%", marginTop: 12 }}>
            <label style={{ display: "block", marginBottom: 6 }}>
              First name / Nickname
            </label>
            <input
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              disabled={loading}
            />
          </div>
        )}

        {role === "Volunteer" && (
          <div style={{ width: "100%", marginTop: 12 }}>
            <label style={{ display: "block", marginBottom: 6 }}>Full name</label>
            <input
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              disabled={loading}
            />
          </div>
        )}

        <div className={styles.otpRow} aria-label="6-digit verification code">
          {digits.map((d, i) => (
            <input
              key={i}
              ref={(el) => (inputsRef.current[i] = el)}
              className={`${styles.otpBox} ${i < 3 ? styles.otpBoxActive : ""}`}
              inputMode="numeric"
              pattern="[0-9]*"
              maxLength={1}
              value={d}
              onChange={(e) => handleChange(i, e)}
              onKeyDown={(e) => handleKeyDown(i, e)}
              onPaste={(e) => handlePaste(i, e)}
              aria-label={`Digit ${i + 1}`}
              disabled={loading}
            />
          ))}
        </div>

        {error && <p style={{ color: "crimson", marginTop: 12 }}>{error}</p>}

        <button
          type="button"
          className={styles.verifyBtn}
          onClick={handleVerify}
          disabled={loading}
        >
          {loading ? "VERIFYING..." : "VERIFY"}
        </button>

        <div className={styles.bottomRow}>
          <button
            type="button"
            className={styles.linkBtn}
            onClick={() => nav("/auth/start")}
            disabled={loading}
          >
            Go back
          </button>

          <div className={styles.resendWrap}>
            <div className={styles.resendText}>Didn't receive the code?</div>
            <button
              type="button"
              className={`${styles.resendBtn} ${
                secondsLeft > 0 ? styles.resendDisabled : ""
              }`}
              onClick={handleResend}
              disabled={secondsLeft > 0 || loading}
            >
              Resend in{" "}
              <span className={styles.resendTime}>
                0:{formatSeconds(secondsLeft)}
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    </>
  );
}