//landing page css

import React from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import styles from "./ReliefHubHome.module.css";
import AppHeader from "../components/AppHeader/AppHeader";

// Put your image at: src/assets/resilio-help.png
import heroBg from "../assets/resilio-help.png";

export default function ReliefHubHome() {
  const nav = useNavigate();
  const { setRole } = useAuth();

  return (
     <>
    <AppHeader />
    <div className={styles.page}>

      {/* Hero */}
      <section
        id="home"
        className={styles.hero}
        style={{ backgroundImage: `url(${heroBg})` }}
      >
        <div className={styles.heroOverlay} />

        <div className={styles.heroContent}>
          <div className={styles.heroCard}>
            <h1 className={styles.heroTitle}>
              CONNECT. RESPOND. REBUILD.
              <br />
              You are Safe Here!
            </h1>

            <div className={styles.heroButtons}>
              <button
                className={`${styles.btn} ${styles.btnPrimary}`}
                onClick={() => {
                  setRole("Victim");
                  nav("/auth/start");
                }}
              >
                I NEED HELP
              </button>

              <button
                className={`${styles.btn} ${styles.btnAccent}`}
                onClick={() => {
                  setRole("Volunteer");
                  nav("/auth/start");
                }}
              >
                I WANT TO HELP
              </button>
            </div>
          </div>
        </div>

        <div className={styles.waveWrap} aria-hidden="true">
          <svg
            className={styles.wave}
            viewBox="0 0 1440 140"
            preserveAspectRatio="none"
          >
            <path
              d="M0,64
                 C180,112 360,112 540,78
                 C720,44 900,18 1080,48
                 C1260,78 1350,98 1440,82
                 L1440,140 L0,140 Z"
              fill="#f4f2fb"
            />
          </svg>
        </div>
      </section>

      {/* How it works */}
      <section className={styles.how} aria-labelledby="how-title">
        <div className={styles.container}>
          <h2 id="how-title" className={styles.sectionTitle}>
            How ReliefHub Works
          </h2>

          <div className={styles.steps}>
            <div className={styles.stepCard}>
              <div className={styles.stepIcon} aria-hidden="true">
                {/* phone icon */}
                <svg width="34" height="34" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M8 2h8a2 2 0 0 1 2 2v16a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2Z"
                    stroke="currentColor"
                    strokeWidth="1.6"
                  />
                  <path
                    d="M10 19h4"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                  />
                </svg>
              </div>
              <h3 className={styles.stepTitle}>SECURE OTP ACCESS</h3>
              <p className={styles.stepText}>
                Fast, confidential verification via your phone or email.
              </p>
            </div>

            <div className={styles.stepCard}>
              <div className={styles.stepIcon} aria-hidden="true">
                {/* handshake icon */}
                <svg width="34" height="34" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M7 12l2-2a3 3 0 0 1 4 0l1 1"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                  />
                  <path
                    d="M3 12l4-4 5 5"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                  />
                  <path
                    d="M21 12l-4-4-4 4"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                  />
                  <path
                    d="M10 14l2 2a3 3 0 0 0 4 0l1-1"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                  />
                </svg>
              </div>
              <h3 className={styles.stepTitle}>
                VERIFIED VOLUNTEERS &amp; PROFILES
              </h3>
              <p className={styles.stepText}>
                Profiles are checked to ensure skilled assistance and safety.
              </p>
            </div>

            <div className={styles.stepCard}>
              <div className={styles.stepIcon} aria-hidden="true">
                {/* rocket icon */}
                <svg width="34" height="34" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M14 4c3 0 6 3 6 6-4 3-7 4-10 4l-2 6-2-2 2-2"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                  <path
                    d="M10 14L4 16l2 2"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                  <path
                    d="M14 4c-4 1-7 4-8 8l4 2"
                    stroke="currentColor"
                    strokeWidth="1.6"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </div>
              <h3 className={styles.stepTitle}>EFFICIENT MATCHING</h3>
              <p className={styles.stepText}>
                Quickly connect with relevant help or offer your unique skills
                where needed.
              </p>
            </div>
          </div>
        </div>
      </section>
    </div>
   </>  
  );
}